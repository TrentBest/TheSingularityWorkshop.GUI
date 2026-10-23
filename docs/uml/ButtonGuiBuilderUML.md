# ButtonGuiBuilder — UML and use cases

**Purpose:** document the fluent Blazor-specific builder for button elements.

## UML class diagram

![Compiler-derived class relationship diagram for ButtonGuiBuilder](source-files/Blazor/TheSingularityWorkshop-Gui-ButtonGuiBuilder-7d34e4ca8a.svg)

The focal type inherits from `ElementBuilder`. Its constructor is internal, so the intended entry point is the public `WorkshopGui` factory. The diagram records source structure; it does not assert a business workflow or authorization relationship.

## Use cases

### Build a labelled action button

1. Obtain the builder through `WorkshopGui.Button(receiver)`.
2. Set a label and, where appropriate, an accessible label.
3. Attach a synchronous or asynchronous click handler.
4. Include the built element in the host's composition.

**Expected result:** a button element with `type="button"` by default, configured text/attributes, and the registered callback.

### Preserve fluent chaining

Use the specialized `Attribute`, `AriaLabel`, `Title`, `Style`, and `OnClick` methods when composing a button. They return the specialized builder, keeping button configuration chainable.

## Explicit limits

- The builder does not authorize the operation represented by the button.
- Handler execution and UI lifecycle belong to the Blazor host.
- This is a Blazor-facing builder API, not a Core semantic-tree builder.
- See the [authored API contract](../api/Blazor/ButtonGuiBuilder.md), [implementation](../../src/Blazor/ButtonGuiBuilder.cs), and [event tests](../../tests/Blazor.Tests/WorkshopGuiBuilderEventTests.cs).
