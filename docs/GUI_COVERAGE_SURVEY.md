# GUI Coverage Survey — Standing on the Shoulders of Existing Systems

This document records what existing GUI systems teach us about the size of the representation domain.

The purpose is not to copy a toolkit. It is to identify the semantic territory that a sufficiently general GUI layer must be able to represent.

## 1. The important limitation

We cannot literally inspect or re-weight the training data or internal model weights of an AI system to determine which GUI concepts occur most frequently.

We can, however, use a useful engineering proxy:

- long-lived GUI systems
- widely deployed operating-system interfaces
- web standards
- mature cross-platform frameworks
- accessibility standards
- graphics and scene systems
- mobile UI systems
- immersive interaction standards
- terminal and text interfaces
- historical GUI architectures

If the same concept repeatedly appears across independent lineages, that is strong evidence that the concept belongs in the semantic vocabulary rather than in one platform adapter.

## 2. The coverage ladder

The GUI domain is substantially larger than the conventional widget set.

| Domain | Representative systems | Semantic territory |
|---|---|---|
| Text terminal | terminals, shells, TTYs | text streams, cursor, selection, input, viewport, color, control sequences |
| Desktop shell | Windows, macOS, Linux desktops | windows, workspaces, menus, launchers, taskbars/docks, notifications, system surfaces |
| Native desktop | Win32, WinForms, WPF, WinUI, Cocoa/AppKit, GTK, Qt | controls, containers, focus, commands, layout, input, data binding, drawing |
| Web | HTML, DOM, CSS, SVG, Canvas | document tree, semantics, layout, styling, graphics, events, media, accessibility |
| Data application | IDEs, spreadsheets, browsers, databases | trees, tables, grids, editors, panes, tabs, docking, selection, filtering, search |
| Authoring/CAD | CAD, BIM, graphics editors, diagramming systems | coordinates, transforms, layers, snapping, selection, geometry, viewports, camera state |
| Media | image/video/audio applications | raster/vector media, timelines, playback, transport, metadata, effects |
| Games | 2D/3D engines and game UIs | scenes, cameras, sprites, meshes, animation, particles, spatial interaction |
| Mobile/touch | iOS, Android | touch, gestures, adaptive layout, system navigation, accessibility, localization |
| Immersive | OpenXR and VR/AR systems | poses, spaces, gaze, hands, controllers, haptics, spatial composition |
| Assistive technology | screen readers, switch access, magnification, braille | semantic roles, actions, state, relationships, focus, alternate presentation |
| Deliberately unstructured | static/noise, telemetry, raw imagery | arbitrary visual/audio fields, degraded signals, continuous data |

The neutral GUI model therefore cannot be designed as a glorified collection of Button, Panel, and TextBox. Those are manifestations of deeper concepts.

## 3. Recurring semantic primitives

### Structure

Existing systems repeatedly use hierarchies or graphs of addressable things:

- DOM trees
- WPF visual/logical trees
- Win32/UI Automation trees
- GTK accessible trees
- Qt model/view hierarchies
- scene graphs
- document structures

A tree is useful, but the semantic model should not assume every relationship is strictly parent/child.

### Identity

Elements need stable identity independent of their current visual manifestation.

Identity matters for interaction, accessibility, automation, state observation, incremental updates, persistence, testing, and multiple simultaneous views.

### Semantics

A representation needs to say what something is for, not merely how it looks.

A button-like object has an action. A tree item has hierarchy. A slider has a value/range. A document has content. A map has spatial meaning.

Accessibility systems make this especially explicit through roles, states, relations, and actions.

### Geometry and space

The GUI domain contains multiple spatial models:

- screen coordinates
- logical coordinates
- document coordinates
- scene coordinates
- normalized coordinates
- world coordinates
- 3D positions
- transforms
- rotation
- scale
- camera/view transforms
- depth
- clipping
- hit regions

Qt Graphics View explicitly separates scene coordinates from view coordinates and supports transformations, zooming, rotation, and large collections of graphical items. citeturn1search1

### Presentation

A general representation needs to describe text and typography, color and gradients, images and media, vector geometry, raster media, icons, borders and effects, animation and transitions, audio-linked presentation, and arbitrary visual fields.

WPF demonstrates how far a mature desktop GUI extends beyond widgets: layout, data binding, graphics, animation, documents, media, text, and typography all belong to the framework. citeturn0search10

### Interaction

Physical devices should map into semantic interaction.

Examples include activate, select, cancel, navigate, focus, point, drag, resize, rotate, zoom, text input, gesture, gaze, pose, voice, and haptic response.

OpenXR is a useful precedent: interaction profiles describe semantic input paths such as select, menu, grip pose, aim pose, and haptics while allowing runtimes to map them onto physical hardware. citeturn0search14

That is close to the desired physical device → semantic interaction → digital reality boundary.

### Data/view separation

Qt's model/view architecture is a strong precedent.

A view obtains data from a model, and the presentation does not have to resemble the underlying data structure. Multiple views can observe the same model. citeturn1search2

That directly supports the Land of Idealism:

> one digital reality, many possible observers.

A diagnostic observer, desktop observer, web observer, spatial observer, and VR observer should not require different underlying realities.

## 4. The operating-system-sized requirement

The phrase "serve up a complete Windows operating system" changes the requirement in an important way.

The GUI layer does not necessarily need to implement Windows.

It needs to be capable of representing the surfaces through which an operating system becomes observable and interactive.

That includes:

- desktop/workspace
- top-level windows
- window chrome
- menus and context menus
- taskbars/docks and launchers
- notifications
- dialogs
- file pickers
- settings surfaces
- system status
- clipboard
- drag/drop
- keyboard focus
- text input and IME/composition
- cursors
- accessibility
- application lifecycle surfaces
- multiple windows and views
- display topology
- scaling and DPI
- screenshots/capture
- system-level automation

Windows UI Automation is particularly instructive. Microsoft exposes a semantic automation tree rooted at the Windows desktop, with application windows and descendant controls, and distinguishes raw, control, and content views. citeturn0search7turn0search0

That means an operating-system GUI can itself be understood as a large semantic representation tree, not merely a pile of pixels.

## 5. Accessibility is not a side feature

Existing systems repeatedly teach the same lesson:

> the semantic representation of a GUI is also useful when pixels are not the primary representation.

Windows UI Automation exposes roles, relationships, navigation, properties, and actions to clients. citeturn0search0turn0search9

GTK exposes roles, states, properties, and relations through an accessibility tree and maps that semantic description onto platform accessibility APIs. citeturn1search0turn1search9

Qt likewise exposes a semantic accessibility tree and accessibility events independently of the visual implementation. citeturn1search10

This suggests an architectural rule:

> Accessibility semantics should be generated from the same neutral GUI representation, not bolted onto a platform renderer afterward.

The same semantic tree can potentially feed visual rendering, screen readers, automation, testing, voice interaction, alternative displays, simplified views, and diagnostic inspection.

## 6. Language must be outside semantic identity

"Klingon friendly" is not merely a translation feature.

The architecture should be language-independent.

That means:

- element identity must not depend on displayed text
- semantic kinds must not depend on English names
- commands/actions must have stable identifiers
- visible labels must be localizable resources
- descriptions must be localizable
- accessibility names must be localizable
- layout must tolerate expansion and contraction
- scripts must not be assumed to be Latin
- bidirectional text must be possible
- text direction must be representable
- fonts and glyph fallback must be externalized
- locale-sensitive formatting must be externalized
- pluralization must be representable
- input methods must be externalized
- cultural formatting must not leak into domain identity

Apple's SwiftUI documentation explicitly treats localization as a property of view presentation and provides localization hints for translators. citeturn1search13

Android Compose similarly expects content descriptions to be localized resources rather than hard-coded visual semantics. citeturn1search4turn1search7

The architectural consequence is:

    Semantic identity
        |
        +-- stable machine identifier
        +-- semantic role
        +-- behavior/action identifiers
        +-- localized presentation resources
        +-- accessibility presentation
        +-- platform manifestation

The machine-readable GUI should therefore be able to contain no English at all.

English can remain the repository's development language without becoming a runtime architectural assumption.

## 7. A GUI is more than visual

The research points toward at least four simultaneous representations:

    DIGITAL REALITY
          |
    +-----+-----+
    |     |     |
    Visual Semantic Interaction
    |     |     |
    pixels roles   input/actions
    /3D   /data
    |     |     |
    +-----+-----+
          |
    Accessibility
          |
    alternate presentation

The visual manifestation is only one consumer.

This is one of the strongest reasons not to make Core a widget library.

## 8. The digital-shadow principle

A useful working architectural term is:

**Digital Shadow of a GUI System**

A GUI system in the outside world becomes a target representation domain inside the ecosystem.

Examples:

- Windows desktop shadow
- web browser shadow
- terminal shadow
- CAD shadow
- spreadsheet shadow
- mobile UI shadow
- VR interaction shadow
- diagnostic console shadow

The shadow does not have to reproduce every implementation detail.

It must reproduce the observable semantic surface necessary for an experience to represent and interact with that system.

This gives us a useful boundary:

> Implementation compatibility is optional; representational compatibility is fundamental.

A Windows shadow does not need to become Win32.

A browser shadow does not need to become Chromium.

A VR shadow does not need to become OpenXR internally.

The GUI layer needs to be able to describe the things those systems expose.

## 9. Low-computation mode is a feature

If a user spends significant time focused on an operating-system-like interface, the renderer may be doing comparatively little domain computation.

That suggests separating:

- domain computation cost
- GUI representation cost
- rendering cost
- input translation cost

A large GUI should not automatically imply a large simulation.

The GUI can be a high-detail observer over a low-computation state.

This is another reason the GUI must remain extrinsic.

## 10. What this changes in the roadmap

Before implementing a large collection of widgets, the repository should establish contracts for:

1. semantic identity
2. hierarchy and relationships
3. roles and capabilities
4. presentation/media
5. geometry and coordinate spaces
6. interaction semantics
7. state/data observation
8. accessibility semantics
9. localization/internationalization
10. viewport/window/surface composition
11. lifecycle
12. capability negotiation

Only after those exist should platform adapters become the primary implementation work.

## 11. The practical target

The target is not:

> "support every GUI framework."

The target is:

> **Any GUI system that can be observed, represented, or interacted with should have a place in the semantic model.**

The adapter determines how much of that representation a particular platform can manifest.

That makes the repository a semantic GUI substrate, rather than another GUI toolkit.
