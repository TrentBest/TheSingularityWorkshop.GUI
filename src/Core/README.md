# TheSingularityWorkshop.GUI.Core

The platform-neutral model and recursive builder primitives for The Singularity Workshop GUI.

**Package:** `TheSingularityWorkshop.GUI.Core`  
**Current project target:** `.NET 8.0`

## What this package provides

- `GuiNode`: an immutable recursive description with kind, stable string identity, optional text/source, a read-only property map, and ordered children.
- `GuiBuilder`: a fluent way to construct a tree and produce a `GuiNode` snapshot.
- Tree lookup through `Find` and `TryFind`.

Core does not render controls, own a UI lifecycle, or execute platform callbacks. Use an adapter package to turn a model into a platform-specific representation.

## Minimal example

```csharp
using TheSingularityWorkshop.Workshop.Gui;

var saveButton = GuiBuilder.Create("Button", "save")
    .Text("Save")
    .Property("class", "primary")
    .Build();

Console.WriteLine(saveButton.Kind); // Button
Console.WriteLine(saveButton.Id);   // save
Console.WriteLine(saveButton.Text); // Save
```

Node kinds and string properties are intentionally simple at this stage. Their interpretation belongs to an adapter's documented contract; arbitrary property names are not guaranteed to have identical meaning across adapters.

## Current boundaries

- A node requires a non-empty kind and ID.
- Child order is significant.
- Properties and child collections are copied into read-only snapshots when a node is constructed.
- Core does not promise serialization, data binding, lifecycle execution, event dispatch, or cross-adapter property parity.

## Documentation

- [GUI model contract](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/docs/GUI_MODEL.md)
- [Architecture](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/docs/ARCHITECTURE.md)
- [Platform adapter guide](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/docs/PLATFORM_ADAPTERS.md)
- [Release readiness checklist](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/docs/RELEASE_READINESS.md)
