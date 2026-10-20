using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

if (args.Length < 2 || args.Length > 3)
{
    Console.Error.WriteLine("Usage: SourceUmlGenerator <input.cs> <output.svg> [--verify]");
    return 2;
}

var input = Path.GetFullPath(args[0]);
var output = Path.GetFullPath(args[1]);
var verify = args.Length == 3 && args[2] == "--verify";
if (!File.Exists(input))
{
    Console.Error.WriteLine($"Input source file does not exist: {input}");
    return 2;
}

var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(input), path: input);
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
    // Only top-level declarations are projected as roots; nested types remain owned by their containing type.
    if (declaration.Ancestors().OfType<BaseTypeDeclarationSyntax>().Any())
        continue;
    cards.Add(ProjectType(declaration));
}

foreach (var declaration in root.DescendantNodes().OfType<DelegateDeclarationSyntax>().OrderBy(n => n.SpanStart))
{
    if (!declaration.Ancestors().OfType<BaseTypeDeclarationSyntax>().Any())
        cards.Add(new DataCard(StableId(declaration), "delegate", declaration.Identifier.ValueText,
            declaration.ToString()));
}

var namespaces = root.DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>()
    .OrderBy(n => n.SpanStart).ToArray();
var namespaceName = namespaces.LastOrDefault()?.Name.ToString() ?? "(global namespace)";
var namespaceCard = new DataCard("namespace:" + namespaceName, "namespace", namespaceName,
    subtitle: "Source-derived namespace", children: cards, canRequestChildInsertion: false);
var svg = new SvgDataCardRenderer().Render(namespaceCard, width: 1100);
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
    foreach (var member in declaration.Members.OrderBy(m => m.SpanStart))
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
        if (string.IsNullOrWhiteSpace(title)) continue;
        members.Add(new DataCard(StableId(member), memberKind, title, subtitle, isExpandedByDefault: false));
    }

    var nestedTypes = declaration.Members.OfType<BaseTypeDeclarationSyntax>()
        .Select(ProjectType).ToArray();
    members.AddRange(nestedTypes);

    var header = declaration.Modifiers.Count > 0
        ? string.Join(" ", declaration.Modifiers.Select(m => m.Text)) + " " + kind + " " + declaration.Identifier.ValueText
        : kind + " " + declaration.Identifier.ValueText;
    if (declaration is TypeDeclarationSyntax type && type.BaseList is not null)
        header += " : " + type.BaseList.Types;
    return new DataCard(StableId(declaration), kind, declaration.Identifier.ValueText,
        subtitle: header, children: members, canRequestChildInsertion: false);
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
