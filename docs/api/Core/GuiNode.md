# GuiNode

**Namespace:** `TheSingularityWorkshop.Workshop.Gui`  
**Kind:** sealed class  
**Source:** [`GuiNode.cs`](../../../src/Core/GuiNode.cs)

## Responsibility

`GuiNode` is an immutable node in the renderer-neutral GUI tree. It snapshots a node's identity, kind, optional text/resource reference, string-valued properties, and ordered children so renderer adapters can consume a stable model.

## UML class diagram

![UML class diagram for GuiNode](../../assets/uml/classes/Core/GuiNode.svg)

The diagram shows the members declared by the source and the ordered self-containment relationship. No base class or implemented interface is declared.

## Constructor and invariants

The constructor rejects blank `kind` or `id` values with `ArgumentException`. It copies the supplied property dictionary into a read-only dictionary using ordinal key comparison and snapshots the child sequence into a read-only ordered collection.

## Public contract

- `Kind` and `Id` identify the semantic node.
- `Text` and `Source` are optional values.
- `Properties` exposes a read-only snapshot of string-valued properties.
- `Children` exposes an ordered read-only child snapshot.
- `Find(id)` returns the first depth-first, left-to-right match and throws `InvalidOperationException` if no node is found.
- `TryFind(id, out node)` performs iterative depth-first, left-to-right traversal; it returns `false` and a null result when the identifier is absent.
- Both lookup methods reject null, empty, and whitespace identifiers.

## Relationships and boundaries

- A node may contain zero or more ordered child `GuiNode` instances.
- The SVG adapter type `SvgGuiRenderer` consumes the node tree and maps its node kinds and properties into SVG elements; its dedicated type page is still pending.
- The type does not render, schedule updates, execute event loops, or mutate its tree after construction.

## Verification

The source tree contains Core test suites, but a dedicated `GuiNodeTests.cs` mapping has not yet been confirmed in this inventory. Add or map focused tests for snapshot immutability, traversal order, missing IDs, invalid IDs, and deep trees before treating those behaviors as independently test-audited.

## Related types

- The related builder type is `GuiBuilder`; its dedicated type page is still pending.
- `SvgGuiRenderer`
- [Per-Type Documentation and UML Standard](../../CLASS_DOCUMENTATION_STANDARD.md)
