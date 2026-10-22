# GuiBuilder UML

**Author:** Trent Best

## Class diagram

```mermaid
classDiagram
    class GuiBuilder {
        +Create(kind, id) GuiBuilder
        +Text(text) GuiBuilder
        +Image(source) GuiBuilder
        +Property(name, value) GuiBuilder
        +WithOnClick(commandId) GuiBuilder
        +Child(kind, id, configure) GuiBuilder
        +Build() GuiNode
    }
    class GuiNode {
        +Kind string
        +Id string
        +Text string
        +Source string
        +Properties IReadOnlyDictionary
        +Children IReadOnlyList
    }
    class HostCommandDispatcher {
        <<host responsibility>>
        +Dispatch(commandId)
    }
    GuiBuilder ..> GuiNode : builds immutable snapshot
    GuiNode ..> HostCommandDispatcher : command identity is consumed by adapter
```

## Use cases

- A builder author creates a button and assigns a stable command ID.
- The built node preserves that ID as semantic metadata.
- A platform adapter may route activation to a host-provided dispatcher.
- The host validates the ID against an explicit command map before invoking domain behavior.

The dispatcher shown here is a conceptual host responsibility, not a type declared by GUI.Core.
