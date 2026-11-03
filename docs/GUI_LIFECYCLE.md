![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# GUI Lifecycle

The GUI is ephemeral.

It may be created, updated, reconfigured, replaced, suspended, resumed, or destroyed without changing the identity of the underlying experience.

## Conceptual lifecycle

```
manifest
   |
   v
experience composition
   |
   v
runtime state
   |
   v
GUI observation
   |
   v
GUI intent
   |
   v
platform manifestation
   |
   +--> input
   |
   +--> state change
   |
   +--> GUI update
   |
   +--> platform replacement
   |
   v
dispose
```

The same experience may have multiple GUI manifestations over its lifetime.

## No permanent GUI identity

The experience should not need to become a WPF window, a Blazor component, a Unity panel, or a VR canvas in order to exist.

Those are manifestations.

This allows:

- headless execution
- diagnostics without operational controls
- a desktop interface
- a browser interface
- an immersive interface
- multiple simultaneous observers

without changing the underlying experience.

## Multiple observers

The model should eventually permit multiple GUI observers of one runtime experience.

For example:

```
                 +--> diagnostic GUI
runtime state ---+--> operational GUI
                 +--> spatial GUI
                 +--> remote GUI
                 +--> VR GUI
```

These observers may present different representations of the same state.

They should not require the runtime domain to duplicate itself for every observer.

## State and rendering

A GUI renderer should not become the authoritative owner of domain state merely because it displays that state.

The domain remains authoritative.

The GUI observes and requests.

This is one of the central protections provided by the extrinsic model.

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center">
  <a href="https://github.com/TrentBest/TheSingularityWorkshop.GUI" aria-label="Visit The Singularity Workshop GUI repository">
    <img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" />
  </a><br />
  <strong>The Singularity Workshop · GUI</strong><br />
  <em>Making software meaning visible to people.</em><br />
  <a href="https://github.com/TrentBest/TheSingularityWorkshop.GUI">Repository</a> · <a href="https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/docs/START_HERE.md">Explore GUI</a> · <a href="https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/docs/GALLERY.md">Visual Gallery</a> · <a href="https://github.com/TrentBest/TheSingularityWorkshop.GUI/issues">Issues and ideas</a>
</p>
