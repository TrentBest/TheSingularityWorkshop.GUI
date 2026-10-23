# ButtonGuiBuilder

**Purpose.** `ButtonGuiBuilder` is a fluent builder for a button element in the Blazor builder API. It specializes `ElementBuilder` with button-specific defaults and fluent return types.

## UML class card

![ButtonGuiBuilder UML relationship diagram](../../uml/source-files/Blazor/TheSingularityWorkshop-Gui-ButtonGuiBuilder-7d34e4ca8a.svg)

See the [focused UML and use cases](../../uml/ButtonGuiBuilderUML.md).

## Contract and behavior

- Construction is internal and performed through the owning builder API; callers should use the public factory rather than instantiate this type directly.
- The base element is `button`; the builder initializes its HTML `type` attribute to `button`, avoiding implicit form-submit behavior by default.
- `Label(text)` sets button text.
- `PositionAt(x, y)` forwards normalized coordinates to the inherited positioning API.
- `Attribute`, `AriaLabel`, `Title`, and `Style` forward to the inherited builder and return `ButtonGuiBuilder` for fluent chaining.
- Three `OnClick` overloads accept `Action`, `Func<Task>`, or `Action<MouseEventArgs>`.

## Example

Use the public `WorkshopGui` factory; the concrete builder's constructor is not public:

```csharp
var save = WorkshopGui.Button(receiver)
    .Label("Save")
    .AriaLabel("Save changes")
    .OnClick(() => SaveAsync());
```

The receiver is supplied by the surrounding Blazor composition context. This example shows the fluent API shape, not a standalone application or persistence implementation.

## Failure and security boundaries

The builder does not authorize the operation represented by a button or persist data. A hidden or disabled UI control is not an authorization boundary. HTML attributes and styles should be selected by the host application's policy.

## Verification

See [Blazor builder event tests](../../../tests/Blazor.Tests/WorkshopGuiBuilderEventTests.cs), [builder implementation](../../../src/Blazor/ButtonGuiBuilder.cs), and the [package workflow](../../../.github/workflows/package.yml).
