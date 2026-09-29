# GUI Documentation

This directory is the engineering documentation for The Singularity Workshop GUI.

The documentation is intentionally split into three levels.

## 1. What exists

These documents describe the current implementation and executable contracts.

- [Implementation Status](IMPLEMENTATION_STATUS.md)
- [GUI Model](GUI_MODEL.md)
- [Testing](TESTING.md)
- [WPF Guide](WPF_GUIDE.md)
- [Platform Adapters](PLATFORM_ADAPTERS.md)

If you need to know whether something is actually shipped, start with **Implementation Status**.

## 2. How the system is organized

- [Architecture](ARCHITECTURE.md)
- [Manifest Boundary](MANIFEST_BOUNDARY.md)
- [GUI Lifecycle](GUI_LIFECYCLE.md)
- [GUI Execution Model](GUI_EXECUTION_MODEL.md)
- [Nested Experiences](GUI_NESTED_EXPERIENCES.md)
- [Next Contract Surface](NEXT_CONTRACT_SURFACE.md)

These documents explain ownership and current architectural boundaries. They may describe contracts that are still being stabilized; check Implementation Status before treating them as public API guarantees.

## 3. Why the system is designed this way

The theory documents explore the larger engineering model:

- [Theory Guide](THEORY_GUIDE.md)
- [Ideal GUI](IDEAL_GUI.md)
- [GUI Expressiveness](GUI_EXPRESSIVENESS.md)
- [GUI Taxonomy](GUI_TAXONOMY.md)
- [GUI Spatial Domain](GUI_SPATIAL_DOMAIN.md)
- [GUI Spatial Contracts](GUI_SPATIAL_CONTRACTS.md)
- [Visual Reference](GUI_VISUAL_REFERENCE.md)
- [Coverage Survey](GUI_COVERAGE_SURVEY.md)
- [Benchmarking](GUI_BENCHMARKING.md)

The theory is deliberately allowed to run ahead of implementation. It describes the destination and the engineering questions; the implementation-status document describes the ground we have actually built.

## Repository entry points

- [Repository README](../README.md)
- [Roadmap](../ROADMAP.md)
- [Start Here](START_HERE.md)

## Documentation rule

When adding a new architectural capability, document it in three places as appropriate:

1. **implementation status** — what code actually exists;
2. **architecture/theory** — why the boundary exists;
3. **roadmap** — what remains before the capability becomes a stable contract.

That keeps the documentation ambitious without making it misleading.
