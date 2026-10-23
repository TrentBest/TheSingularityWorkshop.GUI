# GuiBuilders

**Author:** Trent Best  
**Fully qualified name:** `TheSingularityWorkshop.Workshop.Gui.GuiBuilders`  
**Kind:** static class  
**Assembly:** `TheSingularityWorkshop.GUI.Core`  
**Source:** [`GuiBuilders.cs`](../../../src/Core/GuiBuilders.cs)  
**Tests:** [Core tests](../../../tests/Core.Tests)

## Purpose and boundary

`GuiBuilders` is a convenience factory for the five canonical semantic node kinds currently exposed by Core: Panel, Column, Text, Button, and Warning. It keeps common creation calls short while returning the same platform-neutral `GuiBuilder` used for richer descriptions. It does not create native controls or attach interaction behavior.

## UML class diagram

![Compiler-derived class neighborhood for GuiBuilders](../../uml/source-files/Core/TheSingularityWorkshop-Workshop-Gui-GuiBuilders-66c84fc863.svg)

The class is static and has no instance state. Each factory delegates to `GuiBuilder.Create`; Text, Button, and Warning additionally set the supplied text.

## Contract

- `Panel(id)` creates a `Panel` semantic node.
- `Column(id)` creates a `Column` semantic node.
- `Text(id, text)` creates a `Text` node and assigns its text.
- `Button(id, text)` creates a `Button` semantic node and assigns its text. The factory does not itself implement click handling.
- `Warning(id, text)` creates a `Warning` node and assigns its text.

The factory does not independently validate identifiers or establish renderer-specific layout. The returned builder remains configurable before the caller builds a `GuiNode`.

## Use case

Use the short factory for common semantic structures and switch to the fluent builder for additional properties:

```csharp
var button = GuiBuilders.Button("save", "Save")
    .Property("role", "primary")
    .Build();
```

This creates a semantic node; a renderer or host must decide how it is presented and how any action is connected.

## Related types

- [`GuiBuilder`](GuiBuilder.md) — fluent semantic node builder.
- [`GuiNode`](GuiNode.md) — immutable-ish semantic tree node model.
- [Class documentation standard](../../CLASS_DOCUMENTATION_STANDARD.md)
