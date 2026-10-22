# GuiBuilder

**Author:** Trent Best

## Purpose

`GuiBuilder` is the fluent, platform-neutral builder for an immutable `GuiNode` tree. It expresses presentation structure and stable interaction intent without depending on WPF, Blazor, AnyApp, or a domain package.

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
