# GUI Benchmarking

The GUI should eventually be measurable for the same reason the other Workshop primitives are measurable: a useful abstraction should make its costs visible.

The benchmark project is **not** intended to produce a single number that declares the GUI "fast" or "slow."

It exists to establish controlled baselines for the semantic layer and its adapters.

## Why the WebPage comes first

The first benchmark should follow a representative working slice of the WebPage rather than being designed around whichever synthetic operation happens to look favorable.

The intended progression is:

```
GUI semantic model
       |
       +----> WebPage proving ground
       |
       +----> benchmark baseline
       |
       +----> adapter conformance
       |
       +----> richer spatial workloads
```

The WebPage answers whether a real application can use the architecture. The benchmark then isolates the costs of the work that application actually performs.

The integration is tracked in [WebPage issue #52](https://github.com/TrentBest/WebPage/issues/52). Benchmark project work is tracked in [GUI issue #2](https://github.com/TrentBest/TheSingularityWorkshop.GUI/issues/2).

## What to measure

The benchmark should separate at least five kinds of work.

### 1. Semantic construction

Measure creation of representative GUI trees:

- small control tree;
- nested panels;
- large recursive trees;
- mixed content;
- repeated builder patterns;
- representative multi-panel layouts.

Record:

- elapsed time;
- allocations;
- resulting node count;
- tree depth.

### 2. Semantic traversal and observation

Measure operations that inspect an existing GUI artifact:

- recursive traversal;
- stable-identity lookup;
- property lookup;
- child enumeration;
- view/viewport lookup;
- selection/focus changes;
- replacement of a representation while preserving subject identity.

The benchmark should distinguish cold construction from repeated observation/update work.

### 3. Observation and multi-view composition

Use the spatial model as a first-class workload.

For example:

```
1 subject   -> 1 view   -> 1 viewport
12 subjects -> 12 views -> 12 viewports
100 subjects -> 100 views -> 100 viewports
1000 subjects -> 1000 views -> 1000 viewports
```

Also test:

```
1 subject -> many independent observations
many subjects -> many independent observations
```

The flagship example is a dozen targeted panels showing controlled observations of selected units. The benchmark should vary the number of views rather than treating twelve as a magic number.

### 4. Adapter manifestation

Measure the cost of turning semantic intent into a platform manifestation.

The first useful adapter is Blazor.

Later benchmarks may compare:

- Blazor;
- WPF;
- Unity;
- another materially different adapter.

The comparison must state exactly what is included. Core semantic cost, adapter construction, browser layout, native layout, rendering, GPU work, and I/O must not be silently attributed to one another.

### 5. Serialization and reconstruction

When deterministic GUI artifacts exist, benchmark:

- serialize;
- deserialize;
- reconstruct;
- validate identity;
- validate semantic equivalence.

This belongs later in the benchmark project, not in the first baseline.

## Spatial workload progression

The spatial suite should grow with the semantic contracts:

1. 2D composition;
2. nested coordinate spaces;
3. transform composition;
4. viewport changes;
5. 2D camera pan/zoom;
6. multi-view selection;
7. later 3D camera state;
8. later projection;
9. later hit testing;
10. linked and synchronized views;
11. visibility/occlusion intent;
12. large subject counts and level-of-detail hints.

These workloads should become executable as the corresponding spatial contracts stabilize.

## Required controls

The benchmark project should include control cases where a fair comparison exists:

1. direct/native construction;
2. semantic construction through Core;
3. repeated observation of an already-built artifact;
4. adapter manifestation.

The point is not to manufacture a winner.

The point is to identify where the abstraction costs time or memory and where it provides composability, portability, or representational leverage.

## Benchmark hygiene

Every published result should identify:

- target framework and runtime version;
- operating environment;
- build configuration;
- exact workload size;
- warm-up policy;
- iteration policy;
- allocation measurement method;
- whether an adapter is involved;
- whether rendering is included;
- whether I/O is included;
- whether the measurement is construction, steady-state update, or teardown.

The eventual benchmark project should use a standard statistical benchmark harness rather than handwritten stopwatch loops.

Avoid benchmarks whose only purpose is to produce a large headline number. A useful result tells another engineer what work was actually performed.

## Relationship to the WebPage

The WebPage and the benchmark answer different questions.

The WebPage asks:

> Can a real application use the GUI architecture to produce a useful experience?

The benchmark asks:

> What does the architecture cost while doing so?

The documentation should therefore present them as two forms of evidence rather than one replacing the other.

## What the benchmark cannot prove

A benchmark can establish the cost of a defined operation under a defined workload.

It cannot prove that the architecture is universally faster, nor can it reduce usability, portability, semantic clarity, adapter isolation, or developer productivity to a single metric.

Those questions belong to the working application, contract tests, and architectural evidence alongside the measurements.

## Planned benchmark shape

The eventual project should make workloads readable as named scenarios, for example:

- `ConstructSmallGui`
- `ConstructWidePanelSet`
- `TraverseDeepTree`
- `LookupStableIdentity`
- `ComposeTwelveViews`
- `UpdateSelectedView`
- `ReplaceRepresentationPreserveIdentity`
- `ManifestBlazorRepresentativeSlice`

The names are illustrative until the actual API is stable.

## Status

**Specified:** methodology and workload families.

**Proposed:** executable benchmark project and baseline measurements.

The benchmark becomes authoritative only after the WebPage has a representative working slice consuming the GUI repository.
