using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TheSingularityWorkshop.Workshop.Gui;

namespace SourceUmlGenerator;

/// <summary>
/// Projects a successful project compilation into the renderer-neutral UML v1 model.
/// Relationships in this builder are based on compiler symbols, never on matching name strings.
/// </summary>
internal static class ProjectUmlModelBuilder
{
    public static UmlDocumentationModel Build(
        Compilation compilation,
        string projectPath,
        string repository,
        string revision,
        string configuration,
        string? targetFramework)
    {
        ArgumentNullException.ThrowIfNull(compilation);
        var assembly = compilation.Assembly;
        var assemblyIdentity = assembly.Identity.ToString();
        var symbols = EnumerateTypes(assembly.GlobalNamespace)
            .Where(type => type.Locations.Any(location => location.IsInSource))
            .OrderBy(type => type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), StringComparer.Ordinal)
            .ToArray();
        var ids = new Dictionary<INamedTypeSymbol, string>(SymbolEqualityComparer.Default);
        foreach (var type in symbols)
            ids.Add(type, "type:" + assemblyIdentity + ":" +
                type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

        var documentedTypes = symbols.Select(type =>
        {
            var declarations = type.DeclaringSyntaxReferences
                .Select(reference => SourceDeclaration(reference.GetSyntax(), projectPath))
                .OrderBy(source => source.Path, StringComparer.Ordinal)
                .ThenBy(source => source.Span.Start)
                .ToArray();
            var members = type.GetMembers()
                .Where(member => !member.IsImplicitlyDeclared && member is not INamedTypeSymbol)
                .SelectMany(member => MemberRecords(member, ids[type], projectPath))
                .OrderBy(member => member.Id, StringComparer.Ordinal)
                .ToArray();

            var containingTypes = new Stack<string>();
            for (var containing = type.ContainingType; containing is not null; containing = containing.ContainingType)
                containingTypes.Push(containing.MetadataName);

            return new UmlDocumentedType(
                ids[type],
                new UmlTypeIdentity(
                    assemblyIdentity,
                    type.ContainingNamespace.IsGlobalNamespace ? string.Empty : type.ContainingNamespace.ToDisplayString(),
                    containingTypes.ToArray(),
                    type.MetadataName,
                    type.Arity,
                    KindOf(type)),
                declarations,
                members);
        }).ToArray();

        var relationships = new List<UmlDocumentedRelationship>();
        foreach (var type in symbols)
        {
            var sourceId = ids[type];
            if (type.TypeKind == TypeKind.Class && type.BaseType is { SpecialType: not SpecialType.System_Object } baseType)
            {
                relationships.Add(CreateRelationship(
                    sourceId, baseType, UmlRelationshipKind.Inheritance, "inherits", type, ids, projectPath));
            }

            foreach (var implementedInterface in type.Interfaces)
            {
                relationships.Add(CreateRelationship(
                    sourceId, implementedInterface, UmlRelationshipKind.InterfaceRealization,
                    "implements", type, ids, projectPath));
            }

            foreach (var member in type.GetMembers().Where(item =>
                         !item.IsImplicitlyDeclared && item is not INamedTypeSymbol))
            {
                switch (member)
                {
                    case IFieldSymbol field when !field.IsConst:
                        AddMemberTypeRelationships(relationships, sourceId, member, field.Type,
                            UmlRelationshipKind.Association, "field type", ids, projectPath);
                        break;
                    case IPropertySymbol property:
                        AddMemberTypeRelationships(relationships, sourceId, member, property.Type,
                            UmlRelationshipKind.Association, "property type", ids, projectPath);
                        foreach (var parameter in property.Parameters)
                            AddMemberTypeRelationships(relationships, sourceId, member, parameter.Type,
                                UmlRelationshipKind.Dependency, "index parameter type " + parameter.Name, ids, projectPath);
                        break;
                    case IEventSymbol eventSymbol:
                        AddMemberTypeRelationships(relationships, sourceId, member, eventSymbol.Type,
                            UmlRelationshipKind.Association, "event type", ids, projectPath);
                        break;
                    case IMethodSymbol method when method.MethodKind is not
                        (MethodKind.PropertyGet or MethodKind.PropertySet or MethodKind.EventAdd or MethodKind.EventRemove):
                        if (!method.ReturnsVoid)
                            AddMemberTypeRelationships(relationships, sourceId, member, method.ReturnType,
                                UmlRelationshipKind.Dependency, "return type", ids, projectPath);
                        foreach (var parameter in method.Parameters)
                            AddMemberTypeRelationships(relationships, sourceId, member, parameter.Type,
                                UmlRelationshipKind.Dependency, "parameter type " + parameter.Name, ids, projectPath);
                        break;
                }
            }
        }

        return new UmlDocumentationModel(
            UmlDocumentationModelJson.CurrentSchemaVersion,
            new UmlModelProvenance(
                repository,
                revision,
                Normalize(Path.GetRelativePath(Directory.GetCurrentDirectory(), Path.GetFullPath(projectPath))),
                assemblyIdentity,
                targetFramework,
                configuration,
                "project-semantic",
                null),
            documentedTypes,
            relationships.OrderBy(edge => edge.SourceTypeId, StringComparer.Ordinal)
                .ThenBy(edge => edge.Kind)
                .ThenBy(edge => edge.TargetDisplayName, StringComparer.Ordinal)
                .ToArray());
    }

    private static UmlDocumentedRelationship CreateRelationship(
        string sourceId,
        INamedTypeSymbol target,
        UmlRelationshipKind kind,
        string label,
        INamedTypeSymbol source,
        IReadOnlyDictionary<INamedTypeSymbol, string> ids,
        string projectPath)
    {
        var targetName = target.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var targetIsInModel = ids.TryGetValue(target, out var targetId);
        var syntax = source.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
        var location = syntax is null ? null : SourceDeclaration(syntax, projectPath);
        var span = location?.Span;
        var resolution = targetIsInModel
            ? UmlRelationshipResolution.Resolved
            : UmlRelationshipResolution.External;

        return new UmlDocumentedRelationship(
            sourceId + ":relationship:" + kind + ":" + targetName,
            kind,
            sourceId,
            targetIsInModel ? targetId : null,
            targetName,
            resolution,
            targetIsInModel ? null : target.ContainingAssembly?.Identity.ToString() ?? "unknown",
            "source-to-target",
            label,
            null,
            null,
            new UmlRelationshipProvenance(
                "roslyn-symbol",
                location?.Path,
                span,
                target.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)));
    }

    private static void AddMemberTypeRelationships(
        List<UmlDocumentedRelationship> output,
        string sourceId,
        ISymbol member,
        ITypeSymbol referencedType,
        UmlRelationshipKind kind,
        string label,
        IReadOnlyDictionary<INamedTypeSymbol, string> ids,
        string projectPath)
    {
        var seenTargets = new HashSet<string>(StringComparer.Ordinal);
        var signature = member.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        var reference = member.DeclaringSyntaxReferences.FirstOrDefault();
        var location = reference is null ? null : SourceDeclaration(reference.GetSyntax(), projectPath);

        foreach (var target in EnumerateReferencedTypes(referencedType))
        {
            if (target.SpecialType != SpecialType.None)
                continue;
            var namespaceName = target.ContainingNamespace.IsGlobalNamespace
                ? string.Empty
                : target.ContainingNamespace.ToDisplayString();
            // Framework collection/async wrappers are not useful domain associations; still
            // inspect their generic arguments so application-owned types are retained.
            if (namespaceName == "System" || namespaceName.StartsWith("System.", StringComparison.Ordinal))
                continue;

            var targetName = target.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            if (!seenTargets.Add(targetName))
                continue;

            var targetIsInModel = ids.TryGetValue(target, out var targetId);
            output.Add(new UmlDocumentedRelationship(
                sourceId + ":relationship:" + kind + ":" + label + ":" + signature + ":" + targetName,
                kind,
                sourceId,
                targetIsInModel ? targetId : null,
                targetName,
                targetIsInModel ? UmlRelationshipResolution.Resolved : UmlRelationshipResolution.External,
                targetIsInModel ? null : target.ContainingAssembly?.Identity.ToString() ?? "unknown",
                "source-to-target",
                label,
                null,
                null,
                new UmlRelationshipProvenance(
                    "roslyn-member-symbol",
                    location?.Path,
                    location?.Span,
                    member.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat))));
        }
    }

    private static IEnumerable<INamedTypeSymbol> EnumerateReferencedTypes(ITypeSymbol type)
    {
        switch (type)
        {
            case IArrayTypeSymbol array:
                foreach (var target in EnumerateReferencedTypes(array.ElementType))
                    yield return target;
                yield break;
            case IPointerTypeSymbol pointer:
                foreach (var target in EnumerateReferencedTypes(pointer.PointedAtType))
                    yield return target;
                yield break;
            case INamedTypeSymbol named:
                yield return named.OriginalDefinition;
                foreach (var argument in named.TypeArguments)
                {
                    foreach (var target in EnumerateReferencedTypes(argument))
                        yield return target;
                }
                yield break;
        }
    }

    private static IEnumerable<UmlDocumentedMember> MemberRecords(
        ISymbol member,
        string ownerId,
        string projectPath)
    {
        var references = member.DeclaringSyntaxReferences;
        if (references.Length == 0)
            yield break;
        var signature = member.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        var sources = references.Select(reference => SourceDeclaration(reference.GetSyntax(), projectPath))
            .OrderBy(source => source.Path, StringComparer.Ordinal)
            .ThenBy(source => source.Span.Start)
            .ToArray();
        yield return new UmlDocumentedMember(
            ownerId + ":member:" + member.Kind + ":" + signature,
            member.Kind.ToString(),
            member.MetadataName,
            signature,
            sources);
    }

    private static UmlSourceDeclaration SourceDeclaration(SyntaxNode node, string projectPath)
    {
        var lineSpan = node.GetLocation().GetLineSpan();
        var path = lineSpan.Path;
        if (string.IsNullOrWhiteSpace(path))
            path = node.SyntaxTree.FilePath;
        var fullPath = Path.IsPathRooted(path) ? path : Path.GetFullPath(path);
        var relativePath = Normalize(Path.GetRelativePath(Directory.GetCurrentDirectory(), fullPath));
        var start = lineSpan.StartLinePosition;
        var end = lineSpan.EndLinePosition;
        return new UmlSourceDeclaration(relativePath, new UmlSourceSpan(
            node.SpanStart, node.Span.Length,
            start.Line + 1, start.Character + 1,
            end.Line + 1, end.Character + 1));
    }

    private static IEnumerable<INamedTypeSymbol> EnumerateTypes(INamespaceSymbol root)
    {
        foreach (var member in root.GetMembers())
        {
            if (member is INamespaceSymbol childNamespace)
            {
                foreach (var type in EnumerateTypes(childNamespace))
                    yield return type;
            }
            else if (member is INamedTypeSymbol type)
            {
                foreach (var nested in EnumerateTypeAndNested(type))
                    yield return nested;
            }
        }
    }

    private static IEnumerable<INamedTypeSymbol> EnumerateTypeAndNested(INamedTypeSymbol type)
    {
        yield return type;
        foreach (var nested in type.GetTypeMembers())
        {
            foreach (var item in EnumerateTypeAndNested(nested))
                yield return item;
        }
    }

    private static string KindOf(INamedTypeSymbol type) => type.TypeKind switch
    {
        TypeKind.Class => type.IsRecord ? "record" : "class",
        TypeKind.Struct => type.IsRecord ? "record-struct" : "struct",
        TypeKind.Interface => "interface",
        TypeKind.Enum => "enum",
        TypeKind.Delegate => "delegate",
        _ => type.TypeKind.ToString().ToLowerInvariant()
    };

    private static string Normalize(string path) => path.Replace('\\', '/');
}
