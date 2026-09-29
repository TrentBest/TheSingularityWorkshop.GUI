# Testing Strategy

GUI engineering has two separate correctness problems:

1. model correctness — did we describe the intended GUI correctly?
2. manifestation correctness — did a platform render that intent correctly?

They should not be tested as one problem.

## Current verification surface

| Layer | Build | Tests | Coverage | Platform |
|---|---|---|---|---|
| GUI.Core | Yes | Yes | Yes | Ubuntu |
| GUI.Blazor | Yes | Yes | Test lane present | Ubuntu |
| GUI.WPF | Yes | **Not yet** | Not yet | Windows |
| Future Unity adapter | No | No | No | Unity |

The absence of WPF tests is deliberate and documented; tests should establish real behavioral contracts rather than fabricated placeholders.

## Core tests

Core tests must run without a UI platform.

They cover the semantic model, including:

- recursive composition;
- identity;
- snapshot behavior;
- property isolation;
- deterministic child ordering;
- invalid input;
- the default builder vocabulary.

Core is the foundation for every adapter.

## Blazor tests

Blazor tests verify the current browser manifestation surface and its relationship to the semantic model.

They belong in the Blazor layer because browser rendering details do not belong in Core.

## WPF tests

The repository has a Windows build and packaging lane for WPF.

A dedicated `tests/WPF.Tests` project is **not yet included**. The intended first test layer is:

- semantic renderer mapping;
- Hub rendering;
- canonical kind coverage;
- common property translation;
- native builder construction;
- routed tree-view expansion/collapse behavior;
- window/dialog creation contracts where they can be tested without requiring an interactive desktop session.

Once those tests exist, the Windows lane should run them before packaging.

## Adapter contract tests

Each adapter should eventually consume a shared set of semantic cases.

For example:

```text
Panel(root) -> Button(save) -> Text(Save)
```

must preserve:

- hierarchy;
- identity;
- semantic kind;
- text;
- child ordering.

The native representation may differ.

## CI and packaging

The package workflow is split into independently releasable lanes:

```text
build-core   -> publish_nuget_core
build-blazor -> publish_nuget_blazor
build-wpf    -> publish_nuget_wpf
```

Each publish job is manually gated by the workflow's `publish=true` input.

A successful build/test/pack run creates an artifact. It does not silently publish to NuGet.

