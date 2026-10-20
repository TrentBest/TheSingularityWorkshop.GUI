using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: SourceUmlGenerator <input.cs> <structure.svg> [--verify] [--uml <class-diagram.svg>]");
    return 2;
}

var input = Path.GetFullPath(args[0]);
var output = Path.GetFullPath(args[1]);
var verify = args.Skip(2).Contains("--verify", StringComparer.Ordinal);
var umlOption = Array.IndexOf(args, "--uml");
if (umlOption >= 0 && (umlOption + 1 >= args.Length || args[umlOption + 1].StartsWith("--", StringComparison.Ordinal)))
{
    Console.Error.WriteLine("--uml requires an output SVG path.");
    return 2;
}
var umlOutput = umlOption >= 0 ? Path.GetFullPath(args[umlOption + 1]) : null;
if (!File.Exists(input))
{
    Console.Error.WriteLine($"Input source file does not exist: {input}");
    return 2;
}

var sourceIdentity = args[0].Replace('\\', '/');
var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(input), path: sourceIdentity);
var root = await tree.GetRootAsync();
var diagnostics = tree.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
if (diagnostics.Length > 0)
{
    foreach (var diagnostic in diagnostics)
        Console.Error.WriteLine(diagnostic);
    return 1;
}

var cards = new List<DataCard>();
foreach (var declaration in root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>()
             .OrderBy(n => n.SpanStart))
{
    // Nested types are projected by their owner, not as duplicate root cards.
    if (declaration.Ancestors().OfType<BaseTypeDeclarationSyntax>().Any())
        continue;
    cards.Add(ProjectType(declaration));
}

foreach (var declaration in root.DescendantNodes().OfType<DelegateDeclarationSyntax>().OrderBy(n => n.SpanStart))
{
    if (!declaration.Ancestors().OfType<BaseTypeDeclarationSyntax>().Any())
        cards.Add(new DataCard(StableId(declaration), "delegate", declaration.Identifier.ValueText,
            NamespaceOf(declaration), isExpandedByDefault: false));
}

var sourceName = Path.GetFileName(input);
var sourceCard = new DataCard("source:" + sourceName, "source-file", sourceName,
    subtitle: "C# source-derived structure", children: cards, canRequestChildInsertion: false);
var svg = new SvgDataCardRenderer().Render(sourceCard, width: 1100);
Directory.CreateDirectory(Path.GetDirectoryName(output)!);
File.WriteAllText(output, svg);

var document = XDocument.Parse(svg);
XNamespace ns = "http://www.w3.org/2000/svg";
var renderedIds = document.Descendants(ns + "g")
    .Select(element => (string?)element.Attribute("data-card-id"))
    .Where(id => id is not null).ToArray();
if (renderedIds.Length != renderedIds.Distinct(StringComparer.Ordinal).Count())
{
    Console.Error.WriteLine("Generated diagram contains duplicate stable IDs.");
    return 1;
}
if (verify && (cards.Count == 0 || document.Root?.Element(ns + "title") is null ||
               document.Root.Element(ns + "desc") is null))
{
    Console.Error.WriteLine("Verification requires at least one type and accessible SVG title/description.");
    return 1;
}

if (umlOutput is not null)
{
    var declaration = root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>()
        .Where(node => !node.Ancestors().OfType<BaseTypeDeclarationSyntax>().Any())
        .OrderBy(node => node.SpanStart).FirstOrDefault();
    if (declaration is null)
    {
        Console.Error.WriteLine("UML output requires at least one top-level type declaration.");
        return 1;
    }

    var uml = ProjectUmlClass(declaration);
    var classSvg = new SvgUmlClassDiagramRenderer().Render(uml, width: 1100);
    Directory.CreateDirectory(Path.GetDirectoryName(umlOutput)!);
    File.WriteAllText(umlOutput, classSvg);
    var classDocument = XDocument.Parse(classSvg);
    if (classDocument.Root?.Element(ns + "title") is null || classDocument.Root.Element(ns + "desc") is null)
    {
        Console.Error.WriteLine("Generated UML SVG must have accessible title and description.");
        return 1;
    }
    Console.WriteLine($"Generated UML class diagram for {uml.Name} -> {umlOutput}");
}

Console.WriteLine($"Generated {renderedIds.Length} source-derived cards from {input} -> {output}");
return 0;

static DataCard ProjectType(BaseTypeDeclarationSyntax declaration)
{
    var kind = declaration switch
    {
        ClassDeclarationSyntax => "class",
        StructDeclarationSyntax => "struct",
        InterfaceDeclarationSyntax => "interface",
        EnumDeclarationSyntax => "enum",
        RecordDeclarationSyntax record when record.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword) => "record struct",
        RecordDeclarationSyntax => "record",
        _ => "type"
    };
    var members = new List<DataCard>();
    foreach (var member in MembersOf(declaration).OrderBy(m => m.SpanStart))
    {
        var (memberKind, title, subtitle) = member switch
        {
            FieldDeclarationSyntax field => ("field", string.Join(", ", field.Declaration.Variables.Select(v => v.Identifier.ValueText)),
                field.Declaration.Type + " " + field.Modifiers + (field.Declaration.Variables.FirstOrDefault()?.Initializer is { } init ? " = " + init.Value : "")),
            PropertyDeclarationSyntax property => ("property", property.Identifier.ValueText, property.Type + " " + property.Modifiers),
            MethodDeclarationSyntax method => ("method", method.Identifier.ValueText, method.ReturnType + " " + method.Modifiers + " " + method.ParameterList),
            ConstructorDeclarationSyntax ctor => ("constructor", ctor.Identifier.ValueText, ctor.Modifiers + " " + ctor.ParameterList),
            EventDeclarationSyntax evt => ("event", evt.Identifier.ValueText, evt.Type + " " + evt.Modifiers),
            EventFieldDeclarationSyntax evt => ("event", string.Join(", ", evt.Declaration.Variables.Select(v => v.Identifier.ValueText)), evt.Declaration.Type.ToString()),
            EnumMemberDeclarationSyntax enumMember => ("enum-member", enumMember.Identifier.ValueText, enumMember.EqualsValue?.Value.ToString()),
            BaseTypeDeclarationSyntax nested => ("nested-" + nested.Kind().ToString().ToLowerInvariant(), nested.Identifier.ValueText, nested.ToString().Split('{')[0].Trim()),
            _ => ("member", member.ToString().Split('(', '{', ';')[0].Trim(), member.Kind().ToString())
        };
        if (member is BaseTypeDeclarationSyntax)
            continue; // Nested types are added below as semantic children.
        if (string.IsNullOrWhiteSpace(title)) continue;
        members.Add(new DataCard(StableId(member), memberKind, title, subtitle, isExpandedByDefault: false));
    }

    var nestedTypes = (declaration is TypeDeclarationSyntax owner
            ? owner.Members.OfType<BaseTypeDeclarationSyntax>()
            : Enumerable.Empty<BaseTypeDeclarationSyntax>())
        .Select(ProjectType).ToArray();
    members.AddRange(nestedTypes);

    var modifiers = declaration switch
    {
        TypeDeclarationSyntax type => string.Join(" ", type.Modifiers.Select(m => m.Text)),
        EnumDeclarationSyntax enumeration => string.Join(" ", enumeration.Modifiers.Select(m => m.Text)),
        _ => string.Empty
    };
    var header = (string.IsNullOrWhiteSpace(modifiers) ? "" : modifiers + " ") + kind + " " + declaration.Identifier.ValueText;
    if (declaration is TypeDeclarationSyntax typed && typed.BaseList is not null)
        header += " : " + typed.BaseList.Types;
    return new DataCard(StableId(declaration), kind, declaration.Identifier.ValueText,
        subtitle: NamespaceOf(declaration) + " · " + header, children: members, canRequestChildInsertion: false);
}

static UmlClassDiagram ProjectUmlClass(BaseTypeDeclarationSyntax declaration)
{
    var members = MembersOf(declaration).ToArray();
    var fields = new List<UmlMember>();
    var properties = new List<UmlMember>();
    var operations = new List<UmlMember>();

    foreach (var field in members.OfType<FieldDeclarationSyntax>())
        foreach (var variable in field.Declaration.Variables)
            fields.Add(new UmlMember(variable.Identifier.ValueText,
                field.Declaration.Type + " " + variable.Identifier.ValueText,
                VisibilityOf(field.Modifiers), field.Modifiers.Any(SyntaxKind.StaticKeyword)));

    foreach (var property in members.OfType<PropertyDeclarationSyntax>())
        properties.Add(new UmlMember(property.Identifier.ValueText,
            property.Type + " " + property.Identifier.ValueText +
            (property.ExpressionBody is not null ? " => " + property.ExpressionBody.Expression :
             property.AccessorList is not null ? " { " + string.Join("; ", property.AccessorList.Accessors.Select(a => a.Modifiers + a.Keyword.Text)) + " }" : ""),
            VisibilityOf(property.Modifiers), property.Modifiers.Any(SyntaxKind.StaticKeyword)));

    foreach (var method in members.OfType<MethodDeclarationSyntax>())
        operations.Add(new UmlMember(method.Identifier.ValueText,
            method.ReturnType + " " + method.Identifier.ValueText + method.ParameterList,
            VisibilityOf(method.Modifiers), method.Modifiers.Any(SyntaxKind.StaticKeyword)));

    foreach (var constructor in members.OfType<ConstructorDeclarationSyntax>())
        operations.Add(new UmlMember(constructor.Identifier.ValueText,
            constructor.Identifier.ValueText + "(" + string.Join(",\n    ", constructor.ParameterList.Parameters.Select(parameter =>
                (parameter.Type?.ToString() ?? "var") + " " + parameter.Identifier.ValueText +
                (parameter.Default is null ? "" : " = " + parameter.Default.Value))) + ")",
            VisibilityOf(constructor.Modifiers), constructor.Modifiers.Any(SyntaxKind.StaticKeyword)));

    foreach (var evt in members.OfType<EventDeclarationSyntax>())
        operations.Add(new UmlMember(evt.Identifier.ValueText, "event " + evt.Type + " " + evt.Identifier.ValueText,
            VisibilityOf(evt.Modifiers), evt.Modifiers.Any(SyntaxKind.StaticKeyword)));

    foreach (var evt in members.OfType<EventFieldDeclarationSyntax>())
        foreach (var variable in evt.Declaration.Variables)
            fields.Add(new UmlMember(variable.Identifier.ValueText, "event " + evt.Declaration.Type + " " + variable.Identifier.ValueText,
                VisibilityOf(evt.Modifiers), evt.Modifiers.Any(SyntaxKind.StaticKeyword)));

    foreach (var member in members.OfType<EnumMemberDeclarationSyntax>())
        fields.Add(new UmlMember(member.Identifier.ValueText,
            member.Identifier.ValueText + (member.EqualsValue is null ? "" : " = " + member.EqualsValue.Value),
            "public"));

    var kind = declaration switch
    {
        InterfaceDeclarationSyntax => "interface",
        StructDeclarationSyntax => "struct",
        EnumDeclarationSyntax => "enumeration",
        RecordDeclarationSyntax record when record.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword) => "record struct",
        RecordDeclarationSyntax => "record",
        _ => "class"
    };
    if (declaration is TypeDeclarationSyntax decoratedType)
    {
        var modifiers = decoratedType.Modifiers
            .Where(token => token.IsKind(SyntaxKind.AbstractKeyword) ||
                            token.IsKind(SyntaxKind.SealedKeyword) ||
                            token.IsKind(SyntaxKind.StaticKeyword) ||
                            token.IsKind(SyntaxKind.ReadOnlyKeyword) ||
                            token.IsKind(SyntaxKind.RefKeyword))
            .Select(token => token.Text);
        var prefix = string.Join(" ", modifiers);
        if (!string.IsNullOrWhiteSpace(prefix))
            kind = prefix + " " + kind;
    }
    var relationships = declaration is TypeDeclarationSyntax type && type.BaseList is not null
        ? type.BaseList.Types.Select(t => t.ToString()).ToArray()
        : Array.Empty<string>();
    return new UmlClassDiagram(declaration.Identifier.ValueText, kind, NamespaceOf(declaration),
        fields, properties, operations, relationships,
        $"Source-derived UML class view for {declaration.Identifier.ValueText}. Fields, properties, and operations are parsed from syntax. Type references are not yet semantically resolved, and relationships are not yet drawn as UML connectors.");
}

static string VisibilityOf(SyntaxTokenList modifiers)
{
    if (modifiers.Any(SyntaxKind.PublicKeyword)) return "public";
    if (modifiers.Any(SyntaxKind.ProtectedKeyword)) return "protected";
    if (modifiers.Any(SyntaxKind.PrivateKeyword)) return "private";
    if (modifiers.Any(SyntaxKind.InternalKeyword)) return "internal";
    return "package";
}

static IEnumerable<MemberDeclarationSyntax> MembersOf(BaseTypeDeclarationSyntax declaration) =>
    declaration switch
    {
        TypeDeclarationSyntax type => type.Members,
        EnumDeclarationSyntax enumeration => enumeration.Members,
        _ => Enumerable.Empty<MemberDeclarationSyntax>()
    };

static string NamespaceOf(SyntaxNode node)
{
    var names = node.Ancestors().OfType<BaseNamespaceDeclarationSyntax>()
        .Reverse().Select(n => n.Name.ToString()).ToArray();
    return names.Length == 0 ? "(global namespace)" : string.Join(".", names);
}

static string StableId(SyntaxNode node)
{
    var identifier = node switch
    {
        BaseTypeDeclarationSyntax type => type.Identifier.ValueText,
        DelegateDeclarationSyntax del => del.Identifier.ValueText,
        MethodDeclarationSyntax method => method.Identifier.ValueText + "-" + method.ParameterList,
        PropertyDeclarationSyntax property => property.Identifier.ValueText,
        _ => node.Kind() + "-" + node.SpanStart
    };
    var path = node.SyntaxTree.FilePath.Replace('\\', '/');
    return path + ":" + node.Kind() + ":" + identifier + ":" + node.SpanStart;
}
