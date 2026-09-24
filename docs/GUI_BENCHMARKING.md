# GUI Benchmarking

## Purpose

The GUI should eventually be measurable for the same reason the other Workshop primitives are measurable: a useful abstraction should make its costs visible.

The benchmark project is **not** intended to produce a single number that declares the GUI "fast" or "slow."

It exists to establish controlled baselines for the semantic layer and its adapters.

The first benchmark should answer:

> What does the semantic GUI abstraction actually cost as the workload becomes more expressive?

## What to measure

The benchmark should separate at least five kinds of work.

### 1. Semantic construction

Measure creation of representative GUI trees:

- small control tree;
- nested panels;
- large recursive trees;
- mixed content;
- repeated builder patterns.

Record:

- elapsed time;
- allocations;
- resulting node count;
- tree depth.

### 2. Semantic traversal and observation

Measure operations that inspect an existing GUI artifact:

- recursive traversal;
- identity lookup;
- property lookup;
- child enumeration;
- view/viewport lookup;
- selection/focus changes.

The benchmark should distinguish cold construction from repeated observation.

### 3. Multi-view composition

Use the spatial model as a first-class workload.

For example:

```
1 subject  -> 1 view  -> 1 viewport
12 subjects -> 12 views -> 12 viewports
100 subjects -> 100 views -> 100 viewports
1000 subjects -> 1000 views -> 1000 viewports
```

Where appropriate, include the stronger case:

```
1 subject -> many independent observations
many subjects -> many independent observations
```

The benchmark should verify that the semantic model remains about relationships and identity rather than accidentally duplicating domain state.

### 4. Adapter manifestation

Measure the cost of turning semantic intent into a platform manifestation.

The first useful adapter is Blazor.

Later benchmarks may compare:

- Blazor;
- WPF;
- Unity;
- another materially different adapter.

The comparison must be explicit about what is being measured. Adapter rendering cost, browser cost, layout cost, and native platform cost must not be silently attributed to Core.

### 5. Serialization and reconstruction

When deterministic GUI artifacts exist, benchmark:

- serialize;
- deserialize;
- reconstruct;
- validate identity;
- validate semantic equivalence.

This belongs later in the benchmark project, not in the first baseline.

## Workload design

Each benchmark should publish:

- exact input size;
- semantic workload;
- platform;
- runtime version;
- benchmark harness version;
- warm-up policy;
- iteration policy;
- allocation measurement;
- whether an adapter is involved.

Avoid benchmarks whose only purpose is to produce a large headline number.

A useful result should tell another engineer what work was actually performed.

## Required controls

The benchmark project should include control cases:

1. direct/native construction where a fair comparison exists;
2. semantic construction through Core;
3. adapter manifestation;
4. repeated observation of an already-built artifact.

The point is not to manufacture a winner.

The point is to identify where the abstraction costs time or memory and where it buys composability.

## Spatial benchmark cases

The spatial suite should eventually cover:

- 2D layout composition;
- nested coordinate spaces;
- transform composition;
- viewport changes;
- 2D camera pan/zoom;
- 3D camera state;
- projection;
- hit testing;
- multi-view selection;
- linked cameras;
- synchronized views;
- visibility/occlusion intent;
- large subject counts;
- level-of-detail hints.

These cases become especially useful once the semantic contracts are executable.

## Relationship to the WebPage

The WebPage and the benchmark answer different questions.

The WebPage asks:

> Can a real application use the GUI architecture to produce a useful experience?

The benchmark asks:

> What does the architecture cost while doing so?

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

A successful benchmark does not replace the working application.

A working application does not replace measurement.

Both are evidence.

## Status

**Proposed.**

The benchmark project should be created after the WebPage has a representative working slice built on the GUI repository. That keeps the first benchmark tied to real workloads rather than synthetic abstractions chosen before the architecture has been exercised.
