<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# GuiBuilder

**Author:** Trent Best

## Purpose

`GuiBuilder` is the fluent, platform-neutral builder for an immutable `GuiNode` tree. It expresses presentation structure and stable interaction intent without depending on WPF, Blazor, AnyApp, or a domain package.

## Layout composition

Use semantic `Row` and `Column` nodes to describe horizontal and vertical composition. The WPF adapter renders `Row` as a horizontal `StackPanel`; the optional `orientation` property accepts WPF-compatible `Horizontal` or `Vertical` values when a host needs to override the default.

```csharp
var toolbar = GuiBuilder.Create("Row", "scene-toolbar")
    .Child("Button", "save", button => button.Text("Save"))
    .Child("Button", "cancel", button => button.Text("Cancel"))
    .Build();
```

Layout kinds describe structure only. Spacing, sizing, and theme properties remain renderer-adapter contracts; GUI.Core does not reference platform UI types.

## Presentation state

Use the string-valued `isEnabled` property to express whether a rendered control should appear and behave as enabled in the current view:

```csharp
var unavailable = GuiBuilder.Create("Button", "publish")
    .Text("Publish")
    .Property("isEnabled", "false")
    .Build();
```

The WPF renderer maps valid `true`/`false` values to the native control's enabled state. This is presentation state, not authorization: a host must still validate every requested operation at its execution boundary.

## Interaction contract

Use `WithOnClick(commandId)` to declare the command identity associated with activating a node:

```csharp
var button = GuiBuilders.Button("create-scene", "Create Scene")
    .WithOnClick("storybuilder.scene.create")
    .Build();
```

The builder records the identity as the `onClickCommand` string property. It does not execute the command, discover methods by reflection, or grant permission to perform the operation. The consuming host must explicitly map recognized command IDs to allowed operations.

The WPF adapter can bind the declaration through `WpfGuiRenderer.Render(node, commandDispatcher)`. Rendering without a dispatcher intentionally does not connect semantic command declarations to host operations.

## Use cases

- Build reusable semantic GUI trees independent of platform.
- Declare a stable action identity that survives the renderer boundary.
- Let an application host decide which commands are recognized and authorized.
- Keep domain state and execution authority outside GUI.Core.

## UML

See [GuiBuilder UML](../../docs/uml/GuiBuilderUML.md) for the builder's role and use cases.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
