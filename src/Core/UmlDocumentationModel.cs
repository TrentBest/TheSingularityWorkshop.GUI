namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Versioned, renderer-neutral documentation facts produced by build-time source analysis.
/// This model is not a Roslyn syntax tree, compiled reflection snapshot, or rendered diagram.
/// </summary>
public sealed record UmlDocumentationModel(
    string SchemaVersion,
    UmlModelProvenance Provenance,
    IReadOnlyList<UmlDocumentedType> Types,
    IReadOnlyList<UmlDocumentedRelationship> Relationships);

/// <summary>Identifies the exact source/build context from which a documentation model was produced.</summary>
public sealed record UmlModelProvenance(
    string Repository,
    string SourceRevision,
    string ProjectPath,
    string? AssemblyIdentity,
    string? TargetFramework,
    string Configuration,
    string AnalysisMode,
    string? BuiltAssemblySha256);

/// <summary>Stable metadata identity for a declared type; source positions are deliberately separate.</summary>
public sealed record UmlTypeIdentity(
    string AssemblyIdentity,
    string Namespace,
    IReadOnlyList<string> ContainingTypes,
    string MetadataName,
    int GenericArity,
    string Kind);

/// <summary>A declared type and all source declaration spans that contribute to it.</summary>
public sealed record UmlDocumentedType(
    string Id,
    UmlTypeIdentity Identity,
    IReadOnlyList<UmlSourceDeclaration> SourceDeclarations,
    IReadOnlyList<UmlDocumentedMember> Members);

/// <summary>Stable member identity uses the canonical signature, not the source line number.</summary>
public sealed record UmlDocumentedMember(
    string Id,
    string Kind,
    string MetadataName,
    string Signature,
    IReadOnlyList<UmlSourceDeclaration> SourceDeclarations);

/// <summary>A repository-relative source path and exact syntax span.</summary>
public sealed record UmlSourceDeclaration(string Path, UmlSourceSpan Span);

/// <summary>Zero-based character offset and length plus one-based human-readable line/column coordinates.</summary>
public sealed record UmlSourceSpan(
    int Start,
    int Length,
    int StartLine,
    int StartColumn,
    int EndLine,
    int EndColumn);

/// <summary>Semantic relationship kinds supported by the version 1 documentation contract.</summary>
public enum UmlRelationshipKind
{
    Inheritance,
    InterfaceRealization,
    Association,
    Dependency
}

/// <summary>Whether a relationship target was resolved by compiler symbols.</summary>
public enum UmlRelationshipResolution
{
    Resolved,
    External,
    Unresolved
}

/// <summary>
/// A directed relationship from the declaring/using type to its target. Unresolved targets are
/// represented explicitly and must never be rendered as verified semantic connectors.
/// </summary>
public sealed record UmlDocumentedRelationship(
    string Id,
    UmlRelationshipKind Kind,
    string SourceTypeId,
    string? TargetTypeId,
    string TargetDisplayName,
    UmlRelationshipResolution Resolution,
    string? TargetAssemblyIdentity,
    string Direction,
    string Label,
    string? Multiplicity,
    UmlOwnershipEvidence? OwnershipEvidence,
    UmlRelationshipProvenance Provenance);

/// <summary>
/// Optional evidence for ownership semantics. Field/property references alone are insufficient
/// evidence for aggregation, composition, or ownership.
/// </summary>
public sealed record UmlOwnershipEvidence(
    string EvidenceKind,
    string EvidenceReference,
    string ReviewState,
    string? Reviewer);

/// <summary>Identifies the source fact or compiler symbol that established a relationship.</summary>
public sealed record UmlRelationshipProvenance(
    string Kind,
    string? SourcePath,
    UmlSourceSpan? Span,
    string? SymbolDisplay);

/// <summary>
/// Human-authored use-case intent. Keep this separate from source-derived UML facts because
/// actor goals and business intent cannot be reliably inferred from C# declarations.
/// </summary>
public sealed record UmlUseCaseDocument(
    string SchemaVersion,
    IReadOnlyList<UmlUseCase> UseCases);

/// <summary>A reviewable actor goal with explicit evidence and ownership.</summary>
public sealed record UmlUseCase(
    string Id,
    string Title,
    UmlUseCaseActor Actor,
    string Goal,
    string Trigger,
    IReadOnlyList<string> Preconditions,
    IReadOnlyList<UmlUseCaseStep> MainFlow,
    IReadOnlyList<UmlUseCaseAlternateFlow> AlternateFlows,
    IReadOnlyList<string> Postconditions,
    IReadOnlyList<string> RelatedTypeIds,
    IReadOnlyList<UmlUseCaseEvidence> Evidence,
    UmlUseCaseReview Review,
    UmlUseCaseOwnership Ownership);

public sealed record UmlUseCaseActor(string Name, string Kind);

public sealed record UmlUseCaseStep(int Step, string Text, IReadOnlyList<string> EvidenceIds);

public sealed record UmlUseCaseAlternateFlow(
    string Id,
    string Condition,
    IReadOnlyList<UmlUseCaseStep> Steps,
    IReadOnlyList<string> EvidenceIds);

public sealed record UmlUseCaseEvidence(string Id, string Kind, string Uri, string Claim);

public sealed record UmlUseCaseReview(string State, string? Reviewer, string? ReviewedRevision);

public sealed record UmlUseCaseOwnership(string Owner, string Origin, IReadOnlyList<string> Assumptions);


/// <summary>Canonical JSON serialization settings for the versioned documentation interchange.</summary>
public static class UmlDocumentationModelJson
{
    public const string CurrentSchemaVersion = "1.0";

    private static readonly System.Text.Json.JsonSerializerOptions Options = CreateOptions();

    /// <summary>Serializes a model using camel-case JSON and string-valued kebab-case relationship enums.</summary>
    public static string Serialize(UmlDocumentationModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        if (!string.Equals(model.SchemaVersion, CurrentSchemaVersion, StringComparison.Ordinal))
            throw new ArgumentException($"Unsupported UML documentation schema version: {model.SchemaVersion}", nameof(model));
        ValidateModel(model);

        return System.Text.Json.JsonSerializer.Serialize(model, Options);
    }

    /// <summary>Deserializes only supported schema versions and rejects unknown JSON properties.</summary>
    public static UmlDocumentationModel Deserialize(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        var model = System.Text.Json.JsonSerializer.Deserialize<UmlDocumentationModel>(json, Options)
            ?? throw new System.Text.Json.JsonException("The UML documentation model cannot be null.");
        if (!string.Equals(model.SchemaVersion, CurrentSchemaVersion, StringComparison.Ordinal))
            throw new System.Text.Json.JsonException($"Unsupported UML documentation schema version: {model.SchemaVersion}");
        try
        {
            ValidateModel(model);
        }
        catch (ArgumentException exception)
        {
            throw new System.Text.Json.JsonException("The UML documentation model violates schema v1 invariants.", exception);
        }
        return model;
    }

    private static void ValidateModel(UmlDocumentationModel model)
    {
        Require(model.Provenance is not null, "Model provenance is required.");
        RequireText(model.Provenance.Repository, "Repository provenance is required.");
        RequireText(model.Provenance.SourceRevision, "Source revision provenance is required.");
        RequireText(model.Provenance.ProjectPath, "Project path provenance is required.");
        RequireText(model.Provenance.Configuration, "Build configuration provenance is required.");
        RequireText(model.Provenance.AnalysisMode, "Analysis mode provenance is required.");
        Require(model.Types is not null && model.Relationships is not null, "Type and relationship collections are required.");

        var typeIds = new HashSet<string>(StringComparer.Ordinal);
        var memberIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var type in model.Types)
        {
            Require(type is not null, "Type entries cannot be null.");
            RequireText(type.Id, "Type IDs are required.");
            Require(typeIds.Add(type.Id), $"Duplicate type ID: {type.Id}");
            Require(type.Identity is not null, $"Type identity is required: {type.Id}");
            RequireText(type.Identity.AssemblyIdentity, $"Assembly identity is required: {type.Id}");
            RequireText(type.Identity.MetadataName, $"Metadata name is required: {type.Id}");
            RequireText(type.Identity.Kind, $"Type kind is required: {type.Id}");
            Require(type.Identity.GenericArity >= 0, $"Generic arity cannot be negative: {type.Id}");
            Require(type.SourceDeclarations is not null && type.SourceDeclarations.Count > 0,
                $"At least one source declaration is required: {type.Id}");
            foreach (var source in type.SourceDeclarations)
                ValidateSource(source);
            Require(type.Members is not null, $"Member collection is required: {type.Id}");
            foreach (var member in type.Members)
            {
                Require(member is not null, $"Member entries cannot be null: {type.Id}");
                RequireText(member.Id, "Member IDs are required.");
                Require(memberIds.Add(member.Id), $"Duplicate member ID: {member.Id}");
                RequireText(member.Kind, $"Member kind is required: {member.Id}");
                RequireText(member.MetadataName, $"Member metadata name is required: {member.Id}");
                RequireText(member.Signature, $"Member signature is required: {member.Id}");
                Require(member.SourceDeclarations is not null, $"Member source declarations are required: {member.Id}");
                foreach (var source in member.SourceDeclarations)
                    ValidateSource(source);
            }
        }

        var relationshipIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var relationship in model.Relationships)
        {
            Require(relationship is not null, "Relationship entries cannot be null.");
            RequireText(relationship.Id, "Relationship IDs are required.");
            Require(relationshipIds.Add(relationship.Id), $"Duplicate relationship ID: {relationship.Id}");
            Require(Enum.IsDefined(relationship.Kind), $"Unknown relationship kind: {relationship.Kind}");
            Require(typeIds.Contains(relationship.SourceTypeId),
                $"Relationship source type is absent from the model: {relationship.SourceTypeId}");
            RequireText(relationship.TargetDisplayName, $"Target display name is required: {relationship.Id}");
            Require(Enum.IsDefined(relationship.Resolution), $"Unknown relationship resolution: {relationship.Resolution}");
            RequireText(relationship.Direction, $"Relationship direction is required: {relationship.Id}");
            Require(relationship.Direction == "source-to-target",
                $"Unsupported relationship direction: {relationship.Direction}");
            RequireText(relationship.Label, $"Relationship label is required: {relationship.Id}");
            Require(relationship.Provenance is not null, $"Relationship provenance is required: {relationship.Id}");
            RequireText(relationship.Provenance.Kind, $"Relationship provenance kind is required: {relationship.Id}");
            if (relationship.Provenance.SourcePath is not null)
                RequireText(relationship.Provenance.SourcePath, $"Relationship source path cannot be blank: {relationship.Id}");
            if (relationship.Provenance.Span is not null)
                ValidateSpan(relationship.Provenance.Span);
            if (relationship.Resolution == UmlRelationshipResolution.Resolved)
                Require(relationship.TargetTypeId is not null && typeIds.Contains(relationship.TargetTypeId),
                    $"Resolved target must identify a type in this model: {relationship.Id}");
            if (relationship.Resolution == UmlRelationshipResolution.Unresolved)
                Require(relationship.TargetTypeId is null,
                    $"Unresolved target must not claim a resolved type ID: {relationship.Id}");
            if (relationship.Resolution == UmlRelationshipResolution.External)
                RequireText(relationship.TargetAssemblyIdentity,
                    $"External target must identify its assembly: {relationship.Id}");
            if (relationship.OwnershipEvidence is not null)
            {
                RequireText(relationship.OwnershipEvidence.EvidenceKind, $"Ownership evidence kind is required: {relationship.Id}");
                RequireText(relationship.OwnershipEvidence.EvidenceReference, $"Ownership evidence reference is required: {relationship.Id}");
                RequireText(relationship.OwnershipEvidence.ReviewState, $"Ownership evidence review state is required: {relationship.Id}");
            }
        }
    }

    private static void ValidateSource(UmlSourceDeclaration source)
    {
        Require(source is not null, "Source declarations cannot be null.");
        RequireText(source.Path, "Source paths are required.");
        ValidateSpan(source.Span);
    }

    private static void ValidateSpan(UmlSourceSpan span)
    {
        Require(span is not null, "Source spans are required.");
        Require(span.Start >= 0 && span.Length >= 0, "Source offsets and lengths cannot be negative.");
        Require(span.StartLine >= 1 && span.EndLine >= span.StartLine, "Source line coordinates are invalid.");
        Require(span.StartColumn >= 1 && span.EndColumn >= 1, "Source column coordinates are invalid.");
        if (span.StartLine == span.EndLine)
            Require(span.EndColumn >= span.StartColumn, "Source columns are in reverse order.");
    }

    private static void RequireText(string? value, string message) =>
        Require(!string.IsNullOrWhiteSpace(value), message);

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new ArgumentException(message, "model");
    }

    private static System.Text.Json.JsonSerializerOptions CreateOptions()
    {
        var options = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow
        };
        options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter(
            System.Text.Json.JsonNamingPolicy.KebabCaseLower, allowIntegerValues: false));
        return options;
    }
}
