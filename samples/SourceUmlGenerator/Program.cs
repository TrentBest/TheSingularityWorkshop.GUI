using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

if (args.Length > 0 && string.Equals(args[0], "--project-model-json", StringComparison.Ordinal))
    return await RunProjectModelMode(args.Skip(1).ToArray());

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: SourceUmlGenerator <input.cs> <structure.svg> [--verify] [--uml <class-diagram.svg> --type <name-or-namespace.name>] [--model-json <model.json> --repository <owner/repo> --revision <sha> --project <project.csproj>] | --project-model-json <model.json> --project <project.csproj> --repository <owner/repo> --revision <sha> [--configuration Release --inventory-markdown <inventory.md> --type-pages-dir <directory>]");
    return 2;
}

var input = Path.GetFullPath(args[0]);
var output = Path.GetFullPath(args[1]);
var verify = args.Skip(2).Contains("--verify", StringComparer.Ordinal);
var umlOption = Array.IndexOf(args, "--uml");
var typeOption = Array.IndexOf(args, "--type");
if (typeOption >= 0 && (typeOption + 1 >= args.Length || args[typeOption + 1].StartsWith("--", StringComparison.Ordinal)))
{
    Console.Error.WriteLine("--type requires a type name or fully qualified namespace.type name.");
    return 2;
}
var requestedType = typeOption >= 0 ? args[typeOption + 1] : null;
if (requestedType is not null && umlOption < 0)
{
    Console.Error.WriteLine("--type is only valid with --uml.");
    return 2;
}
if (umlOption >= 0 && (umlOption + 1 >= args.Length || args[umlOption + 1].StartsWith("--", StringComparison.Ordinal)))
{
    Console.Error.WriteLine("--uml requires an output SVG path.");
    return 2;
}
var umlOutput = umlOption >= 0 ? Path.GetFullPath(args[umlOption + 1]) : null;
var modelJsonPath = GetOption(args, "--model-json");
var modelRepository = GetOption(args, "--repository");
var modelRevision = GetOption(args, "--revision");
var modelProject = GetOption(args, "--project");
if (modelJsonPath is not null &&
    (string.IsNullOrWhiteSpace(modelRepository) || string.IsNullOrWhiteSpace(modelRevision) || string.IsNullOrWhiteSpace(modelProject)))
{
    Console.Error.WriteLine("--model-json requires --repository, --revision, and --project provenance values.");
    return 2;
}
if (modelJsonPath is null && new[] { modelRepository, modelRevision, modelProject }.Any(value => value is not null))
{
    Console.Error.WriteLine("--repository, --revision, and --project are only valid with --model-json.");
    return 2;
}
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

if (modelJsonPath is not null)
{
    var model = SourceUmlGenerator.SyntaxOnlyUmlModelBuilder.Build(
        root,
        sourceIdentity,
        modelRepository!,
        modelRevision!,
        modelProject!,
        Environment.GetEnvironmentVariable("CONFIGURATION") ?? "Release");
    var json = UmlDocumentationModelJson.Serialize(model);
    var modelPath = Path.GetFullPath(modelJsonPath);
    Directory.CreateDirectory(Path.GetDirectoryName(modelPath)!);
    File.WriteAllText(modelPath, json + Environment.NewLine);
    Console.WriteLine($"Generated syntax-only UML model with {model.Types.Count} types and {model.Relationships.Count} asserted relationships -> {modelPath}");
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
    var declarations = root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>()
        .OrderBy(node => node.SpanStart).ToArray();
    var matches = requestedType is null ? Array.Empty<BaseTypeDeclarationSyntax>() : declarations.Where(node =>
        string.Equals(node.Identifier.ValueText, requestedType, StringComparison.Ordinal) ||
        string.Equals(DeclaredTypeName(node), requestedType, StringComparison.Ordinal)).ToArray();
    if (matches.Length > 1)
    {
        Console.Error.WriteLine($"Type selection '{requestedType}' is ambiguous; use a fully qualified namespace.type name.");
        return 2;
    }
    var declaration = requestedType is null
        ? declarations.FirstOrDefault(node => !node.Ancestors().OfType<BaseTypeDeclarationSyntax>().Any())
        : matches.SingleOrDefault();
    if (declaration is null)
    {
        Console.Error.WriteLine(requestedType is null
            ? "UML output requires at least one top-level type declaration."
            : $"No type matched '{requestedType}'. Use the declared type name or fully qualified namespace.type name.");
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

static string DeclaredTypeName(BaseTypeDeclarationSyntax node)
{
    var enclosingTypes = node.Ancestors().OfType<BaseTypeDeclarationSyntax>()
        .Reverse().Select(type => type.Identifier.ValueText).ToArray();
    var typePath = string.Join(".", enclosingTypes.Append(node.Identifier.ValueText));
    var ns = NamespaceOf(node);
    return ns == "(global namespace)" ? typePath : ns + "." + typePath;
}

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

static string? GetOption(string[] arguments, string name)
{
    var index = Array.IndexOf(arguments, name);
    if (index < 0)
        return null;
    if (index + 1 >= arguments.Length || arguments[index + 1].StartsWith("--", StringComparison.Ordinal))
        throw new ArgumentException($"{name} requires a value.");
    return arguments[index + 1];
}


static async Task<int> RunProjectModelMode(string[] arguments)
{
    string? projectPath;
    string? repository;
    string? revision;
    string? configuration;
    string? output;
    string? diagramSvg;
    string? diagramType;
    string? inventoryMarkdown;
    string? typePagesDirectory;
    string? reflectionAssemblyPath;
    try
    {
        projectPath = GetOption(arguments, "--project");
        repository = GetOption(arguments, "--repository");
        revision = GetOption(arguments, "--revision");
        configuration = GetOption(arguments, "--configuration") ?? "Release";
        diagramSvg = GetOption(arguments, "--diagram-svg");
        diagramType = GetOption(arguments, "--diagram-type");
        inventoryMarkdown = GetOption(arguments, "--inventory-markdown");
        typePagesDirectory = GetOption(arguments, "--type-pages-dir");
        reflectionAssemblyPath = GetOption(arguments, "--reflection-assembly");
        output = arguments.Length > 0 && !arguments[0].StartsWith("--", StringComparison.Ordinal)
            ? arguments[0]
            : null;
    }
    catch (ArgumentException exception)
    {
        Console.Error.WriteLine(exception.Message);
        return 2;
    }

    if (string.IsNullOrWhiteSpace(output) ||
        string.IsNullOrWhiteSpace(projectPath) ||
        string.IsNullOrWhiteSpace(repository) ||
        string.IsNullOrWhiteSpace(revision))
    {
        Console.Error.WriteLine("--project-model-json requires an output path, --project, --repository, and --revision.");
        return 2;
    }
    if ((diagramSvg is null) != (diagramType is null))
    {
        Console.Error.WriteLine("--diagram-svg and --diagram-type must be supplied together.");
        return 2;
    }

    projectPath = Path.GetFullPath(projectPath);
    if (!File.Exists(projectPath))
    {
        Console.Error.WriteLine($"Project file does not exist: {projectPath}");
        return 2;
    }

    try
    {
        MSBuildLocator.RegisterDefaults();
    }
    catch (InvalidOperationException exception)
    {
        Console.Error.WriteLine($"Could not register MSBuild: {exception.Message}");
        return 1;
    }

    using var workspace = MSBuildWorkspace.Create(new Dictionary<string, string>
    {
        ["Configuration"] = configuration
    });
    workspace.WorkspaceFailed += (_, eventArgs) =>
        Console.Error.WriteLine($"MSBuild workspace {eventArgs.Diagnostic.Kind}: {eventArgs.Diagnostic.Message}");

    Microsoft.CodeAnalysis.Project project;
    try
    {
        project = await workspace.OpenProjectAsync(projectPath);
    }
    catch (Exception exception)
    {
        Console.Error.WriteLine($"Could not load project '{projectPath}': {exception.Message}");
        return 1;
    }

    var compilation = await project.GetCompilationAsync();
    if (compilation is null)
    {
        Console.Error.WriteLine($"MSBuild workspace did not produce a compilation for '{projectPath}'.");
        return 1;
    }

    var errors = compilation.GetDiagnostics()
        .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
        .ToArray();
    if (errors.Length > 0)
    {
        foreach (var error in errors)
            Console.Error.WriteLine(error);
        Console.Error.WriteLine($"Semantic UML generation refused to emit a model because compilation has {errors.Length} error(s).");
        return 1;
    }

    string? targetFramework = null;
    try
    {
        var projectXml = XDocument.Load(projectPath);
        targetFramework = projectXml.Descendants()
            .FirstOrDefault(element => element.Name.LocalName == "TargetFramework")?.Value;
        targetFramework ??= projectXml.Descendants()
            .FirstOrDefault(element => element.Name.LocalName == "TargetFrameworks")?.Value
            .Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault();
    }
    catch (System.Xml.XmlException)
    {
        // MSBuild already loaded the project; framework provenance can remain unspecified.
    }

    var model = SourceUmlGenerator.ProjectUmlModelBuilder.Build(
        compilation, projectPath, repository, revision, configuration, targetFramework);

    if (reflectionAssemblyPath is not null)
    {
        var fullAssemblyPath = Path.GetFullPath(reflectionAssemblyPath);
        if (!File.Exists(fullAssemblyPath))
        {
            Console.Error.WriteLine($"Reflection assembly does not exist: {fullAssemblyPath}");
            return 2;
        }

        try
        {
            var reflectionAssembly = System.Reflection.Assembly.LoadFrom(fullAssemblyPath);
            var reflectedNames = reflectionAssembly.GetTypes()
                .Where(type => !type.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false)
                    && !type.Name.Contains("<", StringComparison.Ordinal))
                .Select(type => type.FullName?.Replace('+', '.') ?? type.Name)
                .ToHashSet(StringComparer.Ordinal);
            var sourceNames = model.Types.Select(ModelMetadataQualifiedTypeName)
                .ToHashSet(StringComparer.Ordinal);
            var missingFromAssembly = sourceNames.Except(reflectedNames, StringComparer.Ordinal).OrderBy(name => name, StringComparer.Ordinal).ToArray();
            var missingFromModel = reflectedNames.Except(sourceNames, StringComparer.Ordinal).OrderBy(name => name, StringComparer.Ordinal).ToArray();
            if (missingFromAssembly.Length > 0 || missingFromModel.Length > 0)
            {
                foreach (var name in missingFromAssembly)
                    Console.Error.WriteLine($"Source model type missing from exact-build assembly: {name}");
                foreach (var name in missingFromModel)
                    Console.Error.WriteLine($"Compiled source type missing from semantic model: {name}");
                return 1;
            }

            Console.WriteLine($"Exact-build reflection check passed: {sourceNames.Count} source types match {reflectionAssembly.GetName().Name}.");
        }
        catch (Exception exception) when (exception is System.Reflection.ReflectionTypeLoadException
                                           or FileLoadException
                                           or BadImageFormatException)
        {
            Console.Error.WriteLine($"Could not reconcile reflection assembly '{fullAssemblyPath}': {exception.Message}");
            if (exception is System.Reflection.ReflectionTypeLoadException loadException)
                foreach (var loaderException in loadException.LoaderExceptions.Where(item => item is not null))
                    Console.Error.WriteLine($"  {loaderException!.Message}");
            return 1;
        }
    }

    UmlDocumentedType? diagramFocus = null;
    if (diagramType is not null)
    {
        var matches = model.Types.Where(type =>
                string.Equals(ModelQualifiedTypeName(type), diagramType, StringComparison.Ordinal) ||
                string.Equals(type.Identity.MetadataName, diagramType, StringComparison.Ordinal))
            .ToArray();
        if (matches.Length != 1)
        {
            Console.Error.WriteLine(matches.Length == 0
                ? $"No compiled type matched diagram target '{diagramType}'."
                : $"Diagram target '{diagramType}' is ambiguous; use a fully qualified namespace.type name.");
            return matches.Length == 0 ? 1 : 2;
        }
        diagramFocus = matches[0];
    }

    var json = UmlDocumentationModelJson.Serialize(model);
    var outputPath = Path.GetFullPath(output);
    Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
    File.WriteAllText(outputPath, json + Environment.NewLine);
    Console.WriteLine(
        $"Generated project-semantic UML model: {model.Types.Count} types, " +
        $"{model.Relationships.Count} compiler-symbol relationships -> {outputPath}");

    if (inventoryMarkdown is not null)
    {
        var inventoryPath = Path.GetFullPath(inventoryMarkdown);
        Directory.CreateDirectory(Path.GetDirectoryName(inventoryPath)!);
        File.WriteAllText(inventoryPath, RenderTypeInventory(model) + Environment.NewLine);
        Console.WriteLine($"Generated deterministic UML type inventory for {model.Types.Count} types -> {inventoryPath}");
    }

    if (typePagesDirectory is not null)
    {
        var pagesDirectory = Path.GetFullPath(typePagesDirectory);
        var pageCount = WriteTypePages(model, pagesDirectory);
        Console.WriteLine($"Generated {model.Types.Count} per-type UML pages plus index -> {pagesDirectory} ({pageCount} Markdown files)");
    }

    if (diagramSvg is not null && diagramFocus is not null)
    {
        var svg = new SvgUmlDocumentationGraphRenderer().Render(model, diagramFocus.Id);
        var diagramPath = Path.GetFullPath(diagramSvg);
        Directory.CreateDirectory(Path.GetDirectoryName(diagramPath)!);
        File.WriteAllText(diagramPath, svg + Environment.NewLine);
        var document = XDocument.Parse(svg);
        XNamespace svgNamespace = "http://www.w3.org/2000/svg";
        if (document.Root?.Element(svgNamespace + "title") is null ||
            document.Root.Element(svgNamespace + "desc") is null)
        {
            Console.Error.WriteLine("Generated semantic UML SVG must have accessible title and description.");
            return 1;
        }
        Console.WriteLine($"Generated focused semantic UML graph for {diagramType} -> {diagramPath}");
    }

    return 0;
}



static int WriteTypePages(UmlDocumentationModel model, string directory)
{
    Directory.CreateDirectory(directory);
    const string manifestName = ".source-uml-generated-files";
    var manifestPath = Path.Combine(directory, manifestName);
    var previousGeneratedFiles = File.Exists(manifestPath)
        ? File.ReadAllLines(manifestPath)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.Ordinal)
            .ToArray()
        : Array.Empty<string>();
    // Only prune files explicitly recorded by our previous manifest. Never enumerate
    // the directory and delete arbitrary Markdown that may be authored by a user.
    if (previousGeneratedFiles.Any(name =>
            Path.GetFileName(name) != name ||
            !name.EndsWith(".md", StringComparison.Ordinal) ||
            string.Equals(name, manifestName, StringComparison.Ordinal)))
        throw new InvalidDataException($"Generated UML file manifest '{manifestPath}' contains an unsafe path.");

    var types = model.Types.OrderBy(ModelQualifiedTypeName, StringComparer.Ordinal)
        .ThenBy(type => type.Id, StringComparer.Ordinal).ToArray();
    var byId = types.ToDictionary(type => type.Id, StringComparer.Ordinal);
    var currentGeneratedFiles = new HashSet<string>(StringComparer.Ordinal) { "index.md" };
    var index = new System.Text.StringBuilder();
    index.AppendLine("# Generated UML type pages");
    index.AppendLine();
    index.AppendLine("> Generated output. Do not edit by hand; regenerate from the source model.");
    index.AppendLine();
    index.AppendLine("- Repository: \u0060" + InventoryCell(model.Provenance.Repository) + "\u0060");
    index.AppendLine("- Revision: [\u0060" + InventoryCell(model.Provenance.SourceRevision) + "\u0060](https://github.com/" + model.Provenance.Repository + "/tree/" + model.Provenance.SourceRevision + ")");
    index.AppendLine("- Project: \u0060" + InventoryCell(model.Provenance.ProjectPath) + "\u0060");
    index.AppendLine("- Types: " + types.Length);
    index.AppendLine();
    index.AppendLine("| Type | Kind | Members | Relationships | Page |");
    index.AppendLine("|---|---|---:|---:|---|");

    foreach (var type in types)
    {
        var qualifiedName = ModelQualifiedTypeName(type);
        var safeName = new string(qualifiedName.Select(character =>
            char.IsLetterOrDigit(character) || character is '-' or '_' ? character : '-').ToArray()).Trim('-');
        var hash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(type.Id))).ToLowerInvariant()[..10];
        var fileName = safeName + "-" + hash + ".md";
        currentGeneratedFiles.Add(fileName);
        var pagePath = Path.Combine(directory, fileName);
        File.WriteAllText(pagePath, RenderTypePage(model, type, byId, fileName) + Environment.NewLine);
        var relationshipCount = model.Relationships.Count(edge => edge.SourceTypeId == type.Id || edge.TargetTypeId == type.Id);
        index.AppendLine("| \u0060" + InventoryCell(qualifiedName) + "\u0060 | \u0060" + InventoryCell(type.Identity.Kind) +
            "\u0060 | " + type.Members.Count + " | " + relationshipCount + " | [" + fileName + "](" + fileName + ") |");
    }

    File.WriteAllText(Path.Combine(directory, "index.md"), index.ToString() + Environment.NewLine);

    foreach (var staleFile in previousGeneratedFiles.Except(currentGeneratedFiles, StringComparer.Ordinal))
    {
        var stalePath = Path.Combine(directory, staleFile);
        if (File.Exists(stalePath))
        {
            File.Delete(stalePath);
            Console.WriteLine($"Pruned stale generated UML page: {stalePath}");
        }
    }

    File.WriteAllLines(manifestPath, currentGeneratedFiles.OrderBy(name => name, StringComparer.Ordinal));
    return currentGeneratedFiles.Count;
}

static string RenderTypePage(
    UmlDocumentationModel model,
    UmlDocumentedType type,
    IReadOnlyDictionary<string, UmlDocumentedType> byId,
    string fileName)
{
    var builder = new System.Text.StringBuilder();
    var qualifiedName = ModelQualifiedTypeName(type);
    builder.AppendLine("# " + qualifiedName);
    builder.AppendLine();
    builder.AppendLine("> Generated from compiler-backed source analysis. Do not edit by hand.");
    builder.AppendLine();
    builder.AppendLine("- Repository: \u0060" + InventoryCell(model.Provenance.Repository) + "\u0060");
    builder.AppendLine("- Revision: [\u0060" + InventoryCell(model.Provenance.SourceRevision) + "\u0060](https://github.com/" + model.Provenance.Repository + "/tree/" + model.Provenance.SourceRevision + ")");
    builder.AppendLine("- Project: \u0060" + InventoryCell(model.Provenance.ProjectPath) + "\u0060");
    builder.AppendLine("- Type ID: \u0060" + InventoryCell(type.Id) + "\u0060");
    builder.AppendLine("- Metadata name: \u0060" + InventoryCell(type.Identity.MetadataName) + "\u0060");
    builder.AppendLine("- Kind: \u0060" + InventoryCell(type.Identity.Kind) + "\u0060");
    builder.AppendLine("- Generic arity: " + type.Identity.GenericArity);
    builder.AppendLine();
    builder.AppendLine("## Source declarations");
    builder.AppendLine();
    foreach (var source in type.SourceDeclarations.OrderBy(item => item.Path, StringComparer.Ordinal).ThenBy(item => item.Span.Start))
        builder.AppendLine("- " + InventorySourceLink(model.Provenance, source));
    builder.AppendLine();
    builder.AppendLine("## Members");
    builder.AppendLine();
    if (type.Members.Count == 0)
        builder.AppendLine("_No explicit source members were recorded._");
    else
    {
        builder.AppendLine("| Kind | Signature | Source |");
        builder.AppendLine("|---|---|---|");
        foreach (var member in type.Members.OrderBy(item => item.Signature, StringComparer.Ordinal).ThenBy(item => item.Id, StringComparer.Ordinal))
        {
            var source = member.SourceDeclarations.OrderBy(item => item.Path, StringComparer.Ordinal).ThenBy(item => item.Span.Start).FirstOrDefault();
            var sourceText = source is null ? "—" : InventorySourceLink(model.Provenance, source);
            builder.AppendLine("| \u0060" + InventoryCell(member.Kind) + "\u0060 | \u0060" +
                InventoryCell(member.Signature) + "\u0060 | " + sourceText + " |");
        }
    }

    builder.AppendLine();
    builder.AppendLine("## Relationships");
    builder.AppendLine();
    var relationships = model.Relationships.Where(edge => edge.SourceTypeId == type.Id || edge.TargetTypeId == type.Id)
        .OrderBy(edge => edge.Kind).ThenBy(edge => edge.SourceTypeId, StringComparer.Ordinal)
        .ThenBy(edge => edge.TargetDisplayName, StringComparer.Ordinal).ThenBy(edge => edge.Id, StringComparer.Ordinal).ToArray();
    if (relationships.Length == 0)
        builder.AppendLine("_No relationships were recorded by this analysis mode._");
    else
    {
        builder.AppendLine("| Direction | Kind | Related type | Resolution | Evidence |");
        builder.AppendLine("|---|---|---|---|---|");
        foreach (var edge in relationships)
        {
            var outgoing = edge.SourceTypeId == type.Id;
            var otherId = outgoing ? edge.TargetTypeId : edge.SourceTypeId;
            var relatedName = otherId is not null && byId.TryGetValue(otherId, out var relatedType)
                ? ModelQualifiedTypeName(relatedType) : edge.TargetDisplayName;
            var evidence = edge.Provenance.SourcePath is null
                ? "\u0060" + InventoryCell(edge.Provenance.Kind) + "\u0060"
                : edge.Provenance.Span is null
                    ? "\u0060" + InventoryCell(edge.Provenance.SourcePath) + "\u0060"
                    : InventorySourceLink(model.Provenance, new UmlSourceDeclaration(edge.Provenance.SourcePath, edge.Provenance.Span));
            builder.AppendLine("| " + (outgoing ? "outgoing" : "incoming") + " | \u0060" +
                InventoryCell(edge.Kind.ToString()) + "\u0060 | \u0060" + InventoryCell(relatedName) +
                "\u0060 | \u0060" + InventoryCell(edge.Resolution.ToString()) + "\u0060 | " + evidence + " |");
        }
    }

    builder.AppendLine();
    builder.AppendLine("[Back to the generated type index](index.md)");
    return builder.ToString();
}

static string InventoryCell(string value) => value.Replace("|", "\\|", StringComparison.Ordinal)
    .Replace("\r", " ", StringComparison.Ordinal)
    .Replace("\n", " ", StringComparison.Ordinal)
    .Replace("\u0060", "'", StringComparison.Ordinal);

static string InventorySourceLink(UmlModelProvenance provenance, UmlSourceDeclaration source)
{
    var path = source.Path.Replace(" ", "%20", StringComparison.Ordinal);
    var url = "https://github.com/" + provenance.Repository + "/blob/" +
        provenance.SourceRevision + "/" + path + "#L" + source.Span.StartLine + "-" + source.Span.EndLine;
    return "[\u0060" + InventoryCell(source.Path) + ":" + source.Span.StartLine + "-" +
        source.Span.EndLine + "\u0060](" + url + ")";
}

static string RenderTypeInventory(UmlDocumentationModel model)
{
    static string Cell(string value) => value.Replace("|", "\\|", StringComparison.Ordinal)
        .Replace("\r", " ", StringComparison.Ordinal)
        .Replace("\n", " ", StringComparison.Ordinal)
        .Replace("\u0060", "'", StringComparison.Ordinal);
    static string QualifiedName(UmlDocumentedType type) => ModelQualifiedTypeName(type);
    static string SourceLink(UmlModelProvenance provenance, UmlSourceDeclaration source)
    {
        var path = source.Path.Replace(" ", "%20", StringComparison.Ordinal);
        var url = "https://github.com/" + provenance.Repository + "/blob/" +
            provenance.SourceRevision + "/" + path + "#L" + source.Span.StartLine + "-" + source.Span.EndLine;
        return "[\u0060" + Cell(source.Path) + ":" + source.Span.StartLine + "-" +
            source.Span.EndLine + "\u0060](" + url + ")";
    }

    var types = model.Types.OrderBy(QualifiedName, StringComparer.Ordinal)
        .ThenBy(type => type.Id, StringComparer.Ordinal).ToArray();
    var byId = types.ToDictionary(type => type.Id, StringComparer.Ordinal);
    var builder = new System.Text.StringBuilder();
    builder.AppendLine("# Generated UML type inventory");
    builder.AppendLine();
    builder.AppendLine("> Generated output. The source and compiler-derived model are authoritative; do not edit this file by hand.");
    builder.AppendLine();
    builder.AppendLine("- Repository: \u0060" + Cell(model.Provenance.Repository) + "\u0060");
    builder.AppendLine("- Revision: [\u0060" + Cell(model.Provenance.SourceRevision) + "\u0060](https://github.com/" + model.Provenance.Repository + "/tree/" + model.Provenance.SourceRevision + ")");
    builder.AppendLine("- Project: \u0060" + Cell(model.Provenance.ProjectPath) + "\u0060");
    builder.AppendLine("- Assembly: \u0060" + Cell(model.Provenance.AssemblyIdentity ?? "unspecified") + "\u0060");
    builder.AppendLine("- Target framework: \u0060" + Cell(model.Provenance.TargetFramework ?? "unspecified") + "\u0060");
    builder.AppendLine("- Configuration: \u0060" + Cell(model.Provenance.Configuration) + "\u0060");
    builder.AppendLine("- Analysis mode: \u0060" + Cell(model.Provenance.AnalysisMode) + "\u0060");
    builder.AppendLine("- Schema: \u0060" + Cell(model.SchemaVersion) + "\u0060");
    builder.AppendLine("- Types: " + types.Length + "; relationships: " + model.Relationships.Count);
    builder.AppendLine();
    builder.AppendLine("## Index");
    builder.AppendLine();
    builder.AppendLine("| Type | Kind | Members | Source declarations | Relationships |");
    builder.AppendLine("|---|---|---:|---:|---:|");
    foreach (var type in types)
    {
        var outgoing = model.Relationships.Count(edge => edge.SourceTypeId == type.Id);
        var incoming = model.Relationships.Count(edge => edge.TargetTypeId == type.Id);
        builder.AppendLine("| [\u0060" + Cell(QualifiedName(type)) + "\u0060](#" + Anchor(QualifiedName(type)) + ") | \u0060" +
            Cell(type.Identity.Kind) + "\u0060 | " + type.Members.Count + " | " + type.SourceDeclarations.Count +
            " | " + outgoing + " out / " + incoming + " in |");
    }

    foreach (var type in types)
    {
        var name = QualifiedName(type);
        builder.AppendLine();
        builder.AppendLine("## " + name);
        builder.AppendLine();
        builder.AppendLine("- Kind: \u0060" + Cell(type.Identity.Kind) + "\u0060");
        builder.AppendLine("- Metadata name: \u0060" + Cell(type.Identity.MetadataName) + "\u0060");
        builder.AppendLine("- Generic arity: " + type.Identity.GenericArity);
        builder.AppendLine("- Type ID: \u0060" + Cell(type.Id) + "\u0060");
        builder.AppendLine("- Source declarations:");
        foreach (var source in type.SourceDeclarations.OrderBy(item => item.Path, StringComparer.Ordinal).ThenBy(item => item.Span.Start))
            builder.AppendLine("  - " + SourceLink(model.Provenance, source));
        builder.AppendLine();
        builder.AppendLine("### Members");
        builder.AppendLine();
        if (type.Members.Count == 0)
            builder.AppendLine("_No explicit source members were recorded._");
        else
        {
            builder.AppendLine("| Kind | Signature | Source |");
            builder.AppendLine("|---|---|---|");
            foreach (var member in type.Members.OrderBy(item => item.Signature, StringComparer.Ordinal).ThenBy(item => item.Id, StringComparer.Ordinal))
            {
                var source = member.SourceDeclarations.OrderBy(item => item.Path, StringComparer.Ordinal).ThenBy(item => item.Span.Start).FirstOrDefault();
                var sourceText = source is null ? "—" : SourceLink(model.Provenance, source);
                builder.AppendLine("| \u0060" + Cell(member.Kind) + "\u0060 | \u0060" + Cell(member.Signature) + "\u0060 | " + sourceText + " |");
            }
        }

        builder.AppendLine();
        builder.AppendLine("### Relationships");
        builder.AppendLine();
        var relationships = model.Relationships.Where(edge => edge.SourceTypeId == type.Id || edge.TargetTypeId == type.Id)
            .OrderBy(edge => edge.Kind).ThenBy(edge => edge.SourceTypeId, StringComparer.Ordinal)
            .ThenBy(edge => edge.TargetDisplayName, StringComparer.Ordinal).ThenBy(edge => edge.Id, StringComparer.Ordinal).ToArray();
        if (relationships.Length == 0)
            builder.AppendLine("_No relationships were recorded by this analysis mode._");
        else
        {
            builder.AppendLine("| Direction | Kind | Target/source | Resolution | Evidence |");
            builder.AppendLine("|---|---|---|---|---|");
            foreach (var edge in relationships)
            {
                var outgoing = edge.SourceTypeId == type.Id;
                var otherId = outgoing ? edge.TargetTypeId : edge.SourceTypeId;
                var otherName = otherId is not null && byId.TryGetValue(otherId, out var otherType)
                    ? QualifiedName(otherType) : edge.TargetDisplayName;
                var evidence = edge.Provenance.SourcePath is null
                    ? "\u0060" + Cell(edge.Provenance.Kind) + "\u0060"
                    : edge.Provenance.Span is null
                        ? "\u0060" + Cell(edge.Provenance.SourcePath) + "\u0060"
                        : SourceLink(model.Provenance, new UmlSourceDeclaration(edge.Provenance.SourcePath, edge.Provenance.Span));
                builder.AppendLine("| " + (outgoing ? "outgoing" : "incoming") + " | \u0060" +
                    Cell(edge.Kind.ToString()) + "\u0060 | \u0060" + Cell(otherName) + "\u0060 | \u0060" +
                    Cell(edge.Resolution.ToString()) + "\u0060 | " + evidence + " |");
            }
        }
    }

    return builder.ToString();
}

static string Anchor(string value) => value.ToLowerInvariant().Replace(".", "", StringComparison.Ordinal)
    .Replace(" ", "-", StringComparison.Ordinal).Replace("\u0060", "", StringComparison.Ordinal);

static string ModelQualifiedTypeName(UmlDocumentedType type)
{
    var parts = type.Identity.ContainingTypes
        .Select(StripMetadataArity)
        .Append(StripMetadataArity(type.Identity.MetadataName));
    var typePath = string.Join(".", parts);
    return string.IsNullOrWhiteSpace(type.Identity.Namespace)
        ? typePath
        : type.Identity.Namespace + "." + typePath;
}

static string ModelMetadataQualifiedTypeName(UmlDocumentedType type)
{
    var typePath = string.Join(".", type.Identity.ContainingTypes.Append(type.Identity.MetadataName));
    return string.IsNullOrWhiteSpace(type.Identity.Namespace)
        ? typePath
        : type.Identity.Namespace + "." + typePath;
}

static string StripMetadataArity(string name)
{
    var index = name.IndexOf('`');
    return index < 0 ? name : name[..index];
}
