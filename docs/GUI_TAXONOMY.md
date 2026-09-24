# GUI Taxonomy

The term "GUI" is broad. Treating every interface as the same kind of GUI creates a poor abstraction.

This repository therefore distinguishes **purpose** from **platform**.

A platform answers:

> Where does this interface manifest?

A GUI purpose answers:

> Why is this interface being presented?

These are independent dimensions.

## Diagnostic GUI

A diagnostic GUI exists primarily to expose information about a system.

Examples include:

- logs
- state inspectors
- telemetry
- memory views
- performance measurements
- object inspectors
- dependency graphs
- state-machine visualizations
- error reports
- debug overlays
- raw data views

Diagnostic GUI should be able to expose information without requiring the represented system to adopt the diagnostic representation as part of its domain model.

## Operational GUI

An operational GUI exists primarily to allow a person or device to operate a system.

Examples include:

- menus
- toolbars
- command palettes
- controls
- configuration panels
- workflow controls
- navigation
- editors
- administration surfaces

Operational GUI is where the interface becomes an active control boundary.

## Informational GUI

An informational GUI communicates information without necessarily exposing an operation.

Examples include:

- labels
- status displays
- indicators
- notices
- legends
- documentation
- read-only reports

## Spatial GUI

A spatial GUI represents relationships in space.

Examples include:

- maps
- floor plans
- diagrams
- node graphs
- dashboards arranged spatially
- CAD-like views
- world-space interfaces

Spatial GUI is important because its concepts cannot be reduced to a vertical stack of controls.

## Immersive GUI

An immersive GUI places the interface into a spatial or simulated environment.

Examples include:

- VR interfaces
- AR interfaces
- world-space controls
- cockpit interfaces
- diegetic interfaces
- spatial dashboards

Immersive GUI is not a separate semantic universe. It is another manifestation context for the same underlying GUI intent.

## Generative / expressive GUI

A GUI may also be used to represent something that has little or no conventional "control" structure.

The representation domain includes:

- images
- meshes
- diagrams
- geometry
- particles
- animation
- video
- audio-driven visualization
- procedural graphics
- noise
- static patterns
- intentionally empty space

A bad television channel displaying static is still a valid visual state.

The GUI abstraction must therefore not silently equate "GUI" with "buttons, labels, and panels."

## Composite GUI

Real interfaces frequently combine these purposes.

For example:

```
Workshop
  |
  +-- operational toolbar
  +-- diagnostic FSM inspector
  +-- spatial world view
  +-- informational status panel
  +-- immersive 3D viewport
```

The model should permit these representations to coexist without forcing them into one primitive category.

## Purpose is not implementation

These categories are semantic.

They do not imply:

- WPF
- Blazor
- HTML
- XAML
- Unity
- VR
- desktop
- mobile

A diagnostic GUI may be rendered in any of those environments.

An operational GUI may be rendered in any of those environments.

The semantic model and the manifestation platform remain separate.

## The important consequence

A platform adapter should answer:

> How do I manifest this semantic GUI here?

It should not answer:

> What kind of GUI is this?

That distinction belongs to the GUI model.
