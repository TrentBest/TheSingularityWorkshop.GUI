# GUI Architecture

## Purpose

The GUI repository owns the engineering boundary between GUI intent and platform manifestation.

A consuming application should be able to describe a user interface without choosing WPF, Blazor, Unity UI Toolkit, or another presentation technology at the point where the domain experience is authored.

The central question is not how to make a WPF button and a Blazor button look similar. It is:

> What is the smallest, stable representation of a GUI that can be interpreted correctly by multiple platforms?

## Layers

### 1. Core model

src/Core contains platform-neutral primitives.

The current model is a recursive tree:

- a node has a kind and stable identity;
- a node may have text or a source;
- a node may have semantic properties;
- a node may contain zero or more child nodes;
- builders construct trees recursively;
- a built tree is a snapshot and does not retain mutable builder state.

Core must not reference WPF, Blazor, HTML, CSS, JavaScript, XAML, Unity, or another presentation framework.

### 2. Platform adapters

Platform projects translate the neutral model into native constructs.

A platform adapter owns:

- native element creation;
- platform-specific layout;
- event wiring;
- accessibility APIs;
- rendering lifecycle;
- resource management;
- platform capability differences.

A platform adapter must not force platform-native concepts into Core merely because the target platform exposes them.

### 3. Domain builders

A consuming application may provide builders such as FsmForgeGuiBuilder or SpatialWorldGuiBuilder.

These builders express domain intent. They belong in the consuming repository because the GUI repository should not know what a Forge, workshop, laboratory, or AEC office means.

## Recursive construction

Recursion is the fundamental composition mechanism.

A builder can create a node and configure children, and each child can recursively create more children. This means a GUI can be assembled from small builders without requiring a monolithic page builder.

Conceptually:

Experience -> Panel -> Panel -> Button -> Text

The same structural intent can then be materialized by different adapters.

## Stable boundary

The following concepts belong in Core only when they have platform-independent semantics:

- identity;
- hierarchy;
- textual content;
- semantic properties;
- resource/source references;
- coordinate intent;
- eventually: layout intent, interaction intent, state binding, accessibility intent, and lifecycle intent.

The following do not belong in Core:

- RenderFragment;
- DependencyObject;
- DOM nodes;
- XAML;
- CSS declarations;
- JavaScript callbacks;
- Unity-specific UI objects.

## Design principle

Core should be boring.

The sophistication belongs in the contracts and in the adapters, not in a hidden dependency on one rendering technology.
