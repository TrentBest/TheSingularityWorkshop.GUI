# TheSingularityWorkshop.GUI

A platform-neutral GUI engineering layer with recursive builders and platform adapters.

The repository exists to separate GUI intent from GUI manifestation.

A consuming application should be able to describe an interface once and allow a platform adapter to materialize it for WPF, Blazor, Unity UI Toolkit, or another supported technology.

## Repository structure

- src/Core — the neutral recursive GUI model and platform-independent primitives.
- src/Blazor — Blazor builders and the first Core-to-web manifestation.
- src/WPF — reusable WPF builders and desktop manifestation infrastructure.
- tests/Core.Tests — platform-independent contract tests.
- docs — architecture, model, adapter, and testing contracts.
- ROADMAP.md — engineering work required to make the neutral model durable.

## The engineering boundary

The important abstraction is not a shared collection of buttons and panels.

It is a semantic GUI model:

application intent -> recursive GUI model -> platform adapter -> native GUI

Core therefore must not depend on HTML, DOM, CSS, JavaScript, Blazor RenderFragment, WPF DependencyObject, XAML, Unity UI Toolkit types, or platform event delegates.

Platform adapters own those details.

## Recursive builders

The model is intentionally recursive. A builder can contain children, and each child can itself contain children.

For example:

Experience -> Panel -> Panel -> Button -> Text

This permits small builders to compose larger interfaces without creating a single application-specific GUI builder.

## Neutrality is a contract

Neutral does not mean copying the lowest common denominator between platforms.

A Core concept belongs in the model only when it has a defined, platform-independent meaning. Platform-specific details are translated by adapters.

This distinction is the central engineering problem for this repository.

See:

- docs/ARCHITECTURE.md
- docs/GUI_MODEL.md
- docs/PLATFORM_ADAPTERS.md
- docs/TESTING.md
- ROADMAP.md

## Current integrations

The WebPage repository is the first migration target for the Blazor layer.

RevitFamilyManagerBuilders remains unchanged. Reusable WPF infrastructure is being migrated here for forward-moving applications rather than modifying that existing repository.

## Status

The repository is in active contract-design phase. The current recursive tree is intentionally small. The next work is to replace ambiguous stringly-typed behavior with explicit semantic contracts, capability negotiation, shared adapter conformance tests, and deterministic GUI artifacts.
