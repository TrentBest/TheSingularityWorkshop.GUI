using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TheSingularityWorkshop.Workshop.Gui;

namespace SourceUmlGenerator;

/// <summary>
/// Projects one parsed source file into the provisional v1 interchange model.
/// This mode is intentionally syntax-only: it emits no asserted semantic relationships.
/// </summary>
internal static class SyntaxOnlyUmlModelBuilder
{
    public static UmlDocumentationModel Build(
        SyntaxNode root,
        string sourcePath,
        string repository,
        string revision,
        string projectPath,
        string configuration)
    {
        var declarations = root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>()
            .OrderBy(node => node.SpanStart)
            .ToArray();
        var assemblyIdentity = "syntax-only";
        var typeIds = declarations.ToDictionary(node => node, node =>
            "type:" + assemblyIdentity + ":" + NamespaceOf(node) + ":" + DeclaredMetadataName(node));

        var types = declarations.Select(node =>
        {
            var identity = new UmlTypeIdentity(
                assemblyIdentity,
                NamespaceOf(node),
                node.Ancestors().OfType<BaseTypeDeclarationSyntax>().Reverse()
                    .Select(parent => parent.Identifier.ValueText).ToArray(),
                MetadataName(node),
                GenericArity(node),
                KindOf(node));
            var source = SourceDeclaration(node, sourcePath);
            var documentedMembers = new List<UmlDocumentedMember>();
            if (node is TypeDeclarationSyntax declaredType)
            {
                foreach (var member in declaredType.Members)
                    AddMember(documentedMembers, member, sourcePath, typeIds[node]);
            }
            else if (node is EnumDeclarationSyntax enumeration)
            {
                foreach (var member in enumeration.Members)
                    AddMember(documentedMembers, member, sourcePath, typeIds[node]);
            }

            return new UmlDocumentedType(
                typeIds[node],
                identity,
                [source],
                documentedMembers);
        }).ToArray();

        return new UmlDocumentationModel(
            UmlDocumentationModelJson.CurrentSchemaVersion,
            new UmlModelProvenance(
                repository,
                revision,
                Normalize(projectPath),
                assemblyIdentity,
                null,
                configuration,
                "syntax-only",
                null),
            types,
            []);
    }

    private static void AddMember(List<UmlDocumentedMember> output, MemberDeclarationSyntax member, string sourcePath, string ownerTypeId)
    {
        var name = member switch
        {
            FieldDeclarationSyntax field => string.Join(",", field.Declaration.Variables.Select(v => v.Identifier.ValueText)),
            EventFieldDeclarationSyntax evt => string.Join(",", evt.Declaration.Variables.Select(v => v.Identifier.ValueText)),
            PropertyDeclarationSyntax property => property.Identifier.ValueText,
            IndexerDeclarationSyntax => "this",
            MethodDeclarationSyntax method => method.Identifier.ValueText,
            ConstructorDeclarationSyntax ctor => ctor.Identifier.ValueText,
            DestructorDeclarationSyntax dtor => dtor.Identifier.ValueText,
            OperatorDeclarationSyntax op => "operator " + op.OperatorToken.Text,
            ConversionOperatorDeclarationSyntax conversion => conversion.ImplicitOrExplicitKeyword.Text + " operator",
            EventDeclarationSyntax evt => evt.Identifier.ValueText,
            EnumMemberDeclarationSyntax enumMember => enumMember.Identifier.ValueText,
            _ => null
        };
        if (name is null)
            return;

        var kind = member.Kind().ToString();
        var signature = member.ToString().Split('{')[0].Trim();
        var span = SourceDeclaration(member, sourcePath);
        var id = ownerTypeId + ":member:" + kind + ":" + name + ":" + signature;
        output.Add(new UmlDocumentedMember(id, kind, name, signature, [span]));
    }

    private static UmlSourceDeclaration SourceDeclaration(SyntaxNode node, string sourcePath)
    {
        var start = node.GetLocation().GetLineSpan().StartLinePosition;
        var end = node.GetLocation().GetLineSpan().EndLinePosition;
        return new UmlSourceDeclaration(Normalize(sourcePath), new UmlSourceSpan(
            node.SpanStart, node.Span.Length,
            start.Line + 1, start.Character + 1,
            end.Line + 1, end.Character + 1));
    }

    private static string DeclaredMetadataName(BaseTypeDeclarationSyntax node)
    {
        var containing = node.Ancestors().OfType<BaseTypeDeclarationSyntax>().Reverse()
            .Select(parent => MetadataName(parent));
        return string.Join("+", containing.Append(MetadataName(node)));
    }

    private static string MetadataName(BaseTypeDeclarationSyntax node) =>
        node.Identifier.ValueText + (GenericArity(node) == 0 ? string.Empty : "`" + GenericArity(node));

    private static int GenericArity(BaseTypeDeclarationSyntax node) =>
        node is TypeDeclarationSyntax type ? type.TypeParameterList?.Parameters.Count ?? 0 : 0;

    private static string KindOf(BaseTypeDeclarationSyntax node) => node switch
    {
        ClassDeclarationSyntax => "class",
        StructDeclarationSyntax => "struct",
        InterfaceDeclarationSyntax => "interface",
        EnumDeclarationSyntax => "enum",
        RecordDeclarationSyntax record when record.ClassOrStructKeyword.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.StructKeyword) => "record-struct",
        RecordDeclarationSyntax => "record",
        _ => "type"
    };

    private static string NamespaceOf(SyntaxNode node)
    {
        var names = node.Ancestors().OfType<BaseNamespaceDeclarationSyntax>()
            .Reverse().Select(n => n.Name.ToString()).ToArray();
        return names.Length == 0 ? string.Empty : string.Join(".", names);
    }

    private static string Normalize(string path) => path.Replace('\\', '/');
}
