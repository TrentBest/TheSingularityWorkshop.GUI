# GUI.SVG Guide

> **Status:** implementation guide for the current SVG adapter contract. This document describes the source and tests on `development`; it does not assert that every example or feature has been verified in every browser.

GUI.SVG converts a platform-neutral `GuiNode` snapshot into a standalone SVG XML document. It is an adapter and serializer, not an animation clock, an FSM_API execution loop, or a GPU renderer.

## Start with executable behavior

- [SVG adapter contract and examples](SVG_ADAPTER.md) — supported node kinds, coordinate mapping, animation-as-data, accessibility, URL policy, and trust boundary.
- [SVG adapter gallery](assets/svg-adapter-gallery.svg) — checked-in generated snapshot; consult the package workflow for the verification status of a particular commit.
- [GUI architecture](ARCHITECTURE.md) — ownership boundary between Core semantics and platform manifestations.

## Current contract

The implementation in `src/SVG/SvgGuiRenderer.cs` validates positive output dimensions and unique node IDs, emits a root SVG title, and emits a description when the root node supplies a non-empty `description` property. The root's `aria-labelledby` references those generated IDs. It validates the tree depth against a limit of 256 using iterative prevalidation before recursive serialization.

The adapter supports a defined SVG-oriented vocabulary for shapes, text, definitions, links, filters, and declarative animation elements. See the source and [SVG adapter contract](SVG_ADAPTER.md) for the current list; a node kind not explicitly handled is not evidence of a fully implemented specialized behavior.

## Animation and runtime boundaries

Animation nodes describe SVG animation instructions. They do not cause this library to advance time, run FSM_API, or coordinate runtime state. The target SVG viewer determines which SVG features it executes. A static generated SVG is evidence of serialized output, not proof of a live application or GPU-backed rendering path.

## Security boundary

Well-formed XML is not the same as safe SVG. The renderer serializes supplied reference values and does not impose a URL-scheme/origin allow-list. Treat scene data as trusted unless the application applies a separate policy. Applications handling untrusted input must validate destinations and consider external-resource, navigation, style, and viewer behavior.

## Evidence discipline

When documenting an SVG artifact, label it accurately as one of:

- **Generated output** — emitted by a named sample or generator from a stated source revision.
- **Source-derived diagram** — projected from parsed/compiled source; identify whether the analysis is syntax-only or compiler-semantic.
- **Hand-authored concept** — illustration of an idea, not proof of implemented behavior.
- **Runtime capture** — captured from an actually running host; name the host and verification evidence.

Do not infer successful CI, package publication, runtime behavior, or cross-viewer compatibility from the existence of an SVG file.
