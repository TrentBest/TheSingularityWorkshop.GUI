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
