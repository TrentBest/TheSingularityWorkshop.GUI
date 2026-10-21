using System.Text.Json;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.GUI.Core.Tests;

public sealed class UmlDocumentationModelTests
{
    [Fact]
    public void Json_contract_preserves_provenance_stable_identity_spans_and_typed_relationships()
    {
        var span = new UmlSourceSpan(10, 12, 2, 3, 2, 15);
        var type = new UmlDocumentedType(
            "type:example:Example`0",
            new UmlTypeIdentity("Example, Version=1.0.0.0", "Example", [], "Example", 0, "class"),
            [new UmlSourceDeclaration("src/Example.cs", span)],
            [new UmlDocumentedMember(
                "member:type:example:Example`0:M:Run()",
                "method",
                "Run",
                "void Run()",
                [new UmlSourceDeclaration("src/Example.cs", span)])]);
        var relationship = new UmlDocumentedRelationship(
            "edge:example:implements:contract",
            UmlRelationshipKind.InterfaceRealization,
            type.Id,
            null,
            "IContract",
            UmlRelationshipResolution.Unresolved,
            null,
            "source-to-target",
            "implements",
            null,
            null,
            new UmlRelationshipProvenance("syntax-only", "src/Example.cs", span, null));
        var model = new UmlDocumentationModel(
            UmlDocumentationModelJson.CurrentSchemaVersion,
            new UmlModelProvenance("owner/repo", "abc123", "src/Example.csproj", null, "net8.0", "Release", "syntax-only", null),
            [type],
            [relationship]);

        var json = UmlDocumentationModelJson.Serialize(model);
        using var document = JsonDocument.Parse(json);
        Assert.Equal("1.0", document.RootElement.GetProperty("schemaVersion").GetString());
        Assert.Equal("interface-realization",
            document.RootElement.GetProperty("relationships")[0].GetProperty("kind").GetString());
        Assert.Equal("unresolved",
            document.RootElement.GetProperty("relationships")[0].GetProperty("resolution").GetString());

        var roundTrip = UmlDocumentationModelJson.Deserialize(json);
        Assert.Equal("abc123", roundTrip.Provenance.SourceRevision);
        Assert.Equal(type.Id, roundTrip.Types[0].Id);
        Assert.Equal(span, roundTrip.Types[0].SourceDeclarations[0].Span);
        Assert.Equal(UmlRelationshipKind.InterfaceRealization, roundTrip.Relationships[0].Kind);
        Assert.Equal(UmlRelationshipResolution.Unresolved, roundTrip.Relationships[0].Resolution);
    }

    [Fact]
    public void Json_contract_rejects_unsupported_schema_versions()
    {
        var model = new UmlDocumentationModel(
            "2.0",
            new UmlModelProvenance("owner/repo", "abc123", "src/Example.csproj", null, "net8.0", "Release", "syntax-only", null),
            [],
            []);

        Assert.Throws<ArgumentException>(() => UmlDocumentationModelJson.Serialize(model));
        Assert.Throws<JsonException>(() => UmlDocumentationModelJson.Deserialize(
            """{"schemaVersion":"2.0","provenance":{},"types":[],"relationships":[]}"""));
    }

    [Fact]
    public void Json_contract_rejects_unknown_properties_instead_of_silently_ignoring_them()
    {
        const string json = """{"schemaVersion":"1.0","provenance":{},"types":[],"relationships":[],"unexpected":true}""";

        Assert.Throws<JsonException>(() => UmlDocumentationModelJson.Deserialize(json));
    }

    [Fact]
    public void Json_contract_rejects_invalid_source_spans_and_resolved_edges_to_missing_types()
    {
        var invalidSpanModel = new UmlDocumentationModel(
            UmlDocumentationModelJson.CurrentSchemaVersion,
            new UmlModelProvenance("owner/repo", "abc123", "src/Example.csproj", null, "net8.0", "Release", "syntax-only", null),
            [new UmlDocumentedType(
                "type:example:Example",
                new UmlTypeIdentity("Example, Version=1.0.0.0", "Example", [], "Example", 0, "class"),
                [new UmlSourceDeclaration("src/Example.cs", new UmlSourceSpan(-1, 10, 1, 1, 1, 11))],
                [])],
            []);

        Assert.Throws<ArgumentException>(() => UmlDocumentationModelJson.Serialize(invalidSpanModel));

        var missingTargetModel = new UmlDocumentationModel(
            UmlDocumentationModelJson.CurrentSchemaVersion,
            new UmlModelProvenance("owner/repo", "abc123", "src/Example.csproj", null, "net8.0", "Release", "roslyn-compilation", null),
            [new UmlDocumentedType(
                "type:example:Example",
                new UmlTypeIdentity("Example, Version=1.0.0.0", "Example", [], "Example", 0, "class"),
                [new UmlSourceDeclaration("src/Example.cs", new UmlSourceSpan(0, 10, 1, 1, 1, 11))],
                [])],
            [new UmlDocumentedRelationship(
                "edge:example:inherits:missing",
                UmlRelationshipKind.Inheritance,
                "type:example:Example",
                "type:external:Missing",
                "Missing",
                UmlRelationshipResolution.Resolved,
                null,
                "source-to-target",
                "inherits",
                null,
                null,
                new UmlRelationshipProvenance("compiler-symbol", "src/Example.cs", null, "Missing"))]);

        Assert.Throws<ArgumentException>(() => UmlDocumentationModelJson.Serialize(missingTargetModel));
    }
}
