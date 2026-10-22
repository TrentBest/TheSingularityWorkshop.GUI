# GuiNode — UML and Use Cases

**Author:** Trent Best  
**Target type:** `TheSingularityWorkshop.Workshop.Gui.GuiNode`  
**Kind:** sealed class  
**Source:** [`src/Core/GuiNode.cs`](../../src/Core/GuiNode.cs)  
**Type reference:** [GuiNode API documentation](../api/Core/GuiNode.md)  
**Diagram status:** manually maintained, reviewed against source at the time of this change

## Target-centered class diagram

![Target-centered UML class diagram for GuiNode](../assets/uml/classes/Core/GuiNode.svg)

The focal type is `GuiNode`. Its direct structural relationship is an ordered collection of child `GuiNode` instances. The constructor snapshots input properties and children; the public surface exposes immutable values and lookup operations. The class declares no base class or implemented interface.

## Use cases

### 1. Describe a renderer-neutral UI subtree

**Actor:** a Core consumer or builder  
**Goal:** represent a panel and its children without binding the model to a UI framework.

1. Construct each node with a nonblank kind and stable identifier.
2. Supply optional text, source information, string properties, and ordered children.
3. Pass the immutable root to a host or renderer adapter that understands the relevant node kinds.

**Outcome:** the consumer has a stable semantic snapshot. `GuiNode` itself does not render the tree or guarantee that every adapter supports every kind or property.

### 2. Locate a node by identifier

**Actor:** a host, adapter, or application service  
**Goal:** find a descendant (or the root itself) by ID.

1. Call `TryFind(id, out node)` when absence is an expected outcome.
2. Use `Find(id)` when absence should be treated as an error.
3. Account for depth-first, left-to-right traversal if IDs are duplicated; the first match wins.

**Outcome:** the caller receives the first matching node or a documented absence/failure result. The type does not enforce uniqueness of IDs.

### 3. Preserve a stable view of caller-supplied collections

**Actor:** a builder or application assembling a tree  
**Goal:** avoid later mutations of the original input collections changing an already-created node.

1. Construct a node with a property dictionary and child sequence.
2. Continue modifying the original collections if needed.
3. Read the node's `Properties` and `Children`.

**Outcome:** the node retains its own read-only snapshots. The collection snapshots do not make mutable objects stored outside these string-valued properties immutable; the child nodes themselves are immutable.

## Responsibility boundaries

- **Owns:** node identity, kind, optional text/source values, a string-to-string property snapshot, ordered child snapshot, and depth-first lookup.
- **Does not own:** rendering, layout calculation, live UI state, event dispatch, command authorization, animation, or framework-specific objects.
- **Verified source facts:** constructor validation, snapshot creation, read-only public properties, iterative depth-first lookup, and missing-node behavior are visible in the linked source.
- **Verification gap:** a dedicated direct `GuiNode` test suite has not yet been mapped. See the verification section in the [type page](../api/Core/GuiNode.md); do not interpret this use-case description as test evidence.

## Relationship semantics

The child collection is an association from a node to zero or more ordered child nodes. It is not a claim that the tree is globally unique by ID, that nodes can be reparented, or that a renderer has successfully manifested the model. Source structure, documented use cases, and runtime observations are separate evidence categories.
