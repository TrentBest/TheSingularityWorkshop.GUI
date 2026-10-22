# Evidence and Boundaries in Semantic GUI Systems

## Thesis

A semantic GUI abstraction earns credibility not by the breadth of its vocabulary, but by the correspondence between each promised meaning and the behavior its adapters actually preserve.

This chapter makes the evidence boundary explicit: an architecture can be a useful direction while some of its consequences remain unproven.

## Four distinct evidence classes

1. **Source structure** establishes declared types, signatures, dependencies, and code-level relationships.
2. **Executable tests** establish only the behaviors their assertions exercise, under the tested inputs and runtime.
3. **Consumer evidence** establishes that a particular package artifact can be restored and used in a specified environment.
4. **Theory** proposes why those mechanisms matter and predicts what should remain stable as platforms change.

These classes complement one another; none can silently substitute for another. A generated class diagram does not prove runtime behavior. A passing unit test does not prove package installation. A successful package consumer does not prove untested properties or full adapter parity. A compelling architectural argument does not prove that the implementation has achieved it.

## The semantic-boundary proposition

The repository's current concrete test is narrow and useful: Core describes a recursive semantic tree, while Blazor, SVG, and WPF provide distinct manifestation paths. That separation supports the proposition that interface meaning can be represented independently of a specific renderer in the covered model.

It does not establish that every conceivable interface can be represented, that all adapters preserve identical behavior, or that the same output is visually identical across platforms. Those are stronger claims requiring separate contracts and evidence.

## Compatibility is a matrix, not a slogan

Representational compatibility must be evaluated across at least four dimensions:

- **Semantic input:** which node kinds, properties, identities, and child structures are accepted?
- **Transformation:** how does each adapter map that input, and what is omitted or rejected?
- **Output behavior:** what can the output actually do—static display, native interaction, navigation, animation, or something else?
- **Consumer context:** which framework, operating system, package version, and host were verified?

For a particular capability, record supported, unsupported, or unverified for each adapter. Blank cells are not evidence of parity. An intentional adapter-specific difference can be a valid design; an undocumented difference is a compatibility risk.

## Failure behavior is part of the contract

Malformed input, unsupported properties, duplicate IDs, excessive nesting, unsafe external references, and resource exhaustion are not peripheral details. They determine whether a semantic description can be consumed predictably.

The safe architectural posture is to define boundary behavior and test it. In particular, serializing an external URI is not the same as authorizing navigation or resource access; the application or host remains responsible for its trust policy unless an explicit policy layer is implemented and verified.

## Consequences for theory and implementation

The theory should be updated when a repeatable observation changes the argument. Each substantial claim should link to:

- the implementation boundary that carries the claim;
- a test that asserts the relevant behavior;
- the package/adapter/runtime scope of that test;
- known exceptions and unverified cases.

This keeps theory falsifiable and prevents roadmap aspirations from being mistaken for delivered features.

## Repository evidence and limits

The current release gate and adapter audit record the repository's known verification status. See [Release Readiness](../RELEASE_READINESS.md), [GUI Surface Coverage Audit](../GUI_SURFACE_COVERAGE_AUDIT.md), and [Per-Type Documentation and UML Standard](../CLASS_DOCUMENTATION_STANDARD.md). The [semantic-tree-to-SVG use case](../USE_CASES/01-SEMANTIC-TREE-TO-SVG.md) supplies one concrete, intentionally narrow example.

This chapter describes an evidence discipline; it does not claim that the remaining release gates have passed.
