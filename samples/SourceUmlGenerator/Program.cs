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

var sourceIdentity = args[0].Replace('\\\\', '/');
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
