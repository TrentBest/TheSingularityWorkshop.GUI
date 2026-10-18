# Subcomposition and Digital Art — Workshop Tool Contract

**Status:** Design contract and implementation plan. This document does not claim a voxel editor, mesh extractor, painting engine, or mutable asset repository is implemented.

## 00. The idea

Present the user with a block of digital material and tools to change it. The user carves, adds, paints, draws, annotates, saves, closes, and later reopens the work. The editable source remains authoritative; visible surfaces are derived from it.

The first sculpting mode is **voxel subcomposition**: a bounded volume is partitioned into small cells, each cell records whether material exists and which material it contains, and edits change those cells. The exposed boundary is converted into a mesh for efficient rendering. We must not create one managed object or one render primitive per voxel.

The term *subcomposition* names the authoring operation, not the mesh algorithm. The durable artifact is the editable model plus its edit history and metadata—not merely the latest mesh.

## 01. Place the capability in the ecosystem

The capability belongs at the intersection of a domain model, an editor/tool experience, rendering, and durable project storage. These responsibilities must remain independently replaceable. This document is linked from GUI because it defines the editor-facing contract; **GUI is not the home of the voxel model or storage implementation.**

```text
User / Workshop editor
        |
        | semantic tools: carve, fill, smooth, draw, paint, select
        v
GUI / editor manifestation
        |
        v
Subcomposition authoring capability (domain model)
  - stable asset identity
  - chunked volume and material assignments
  - edit operations and undo/redo
  - revisions and derived-data invalidation
        |
        +-----------------------> Editable Asset Store
        |                          - mutable drafts
        |                          - snapshots + operation journal
        |                          - versioned schema / recovery
        |
        v
Derived surface pipeline
  - changed-chunk invalidation
  - surface extraction / mesh generation
  - normals, material regions, collision proxy as needed
        |
        v
Renderer -> Visualize -> host surface
        |
        v
pixels / inspectable preview
```

### Ownership

| Concern | Owner |
| --- | --- |
| Voxel values, material IDs, dimensions, transforms, stable asset identity | Subcomposition domain |
| Carving, filling, smoothing, selection, drawing and painting gestures | Editor/tool capability |
| Neutral controls, panels, pointer routing, accessibility, host adaptation | GUI and its adapters |
| Camera-relative visibility, representation choice and presentation work | Renderer |
| GPU buffers, mesh uploads, rendering execution and diagnostics | Visualize |
| Mutable user project persistence, revisions, recovery and conflict policy | Editable Asset Store boundary |
| Composition of tools and storage/render capabilities for an experience | FSM_COS |
| Distribution of versioned tool MicroBundles | MicroBundleRepository |

The tool should be consumable by multiple experiences. It must not be embedded exclusively in Visualize, GUI, a game, or a single host. The editor is a consumer and composition of capabilities; it is not the owner of the underlying model semantics.

### Proposed package placement

Create a dedicated `TheSingularityWorkshop.Subcomposition` repository/package for the platform-neutral editable volume model, deterministic edit operations, stable asset identity, revisions, and surface-derivation contracts. Keep it free of GUI, renderer, GPU, and host types. The first implementation can remain deliberately small; do not create a forest of packages before the contracts justify it.

The editor-facing tool should be a separately composable capability that consumes the Subcomposition contract. Drawing/painting can initially be additional tools over shared asset/operation contracts; promote them into a separate `TheSingularityWorkshop.Drawing` capability when their independent scope warrants it. The mutable persistence boundary is a separate candidate package/contract (provisionally `TheSingularityWorkshop.AssetStore`), with filesystem and cloud adapters supplied independently. These names are proposals, not existing repositories.

### MicroBundles and package boundary

The authoring capability can be supplied as a MicroBundle. Its contract should declare the model-editing and derived-surface capabilities it requires. FSM_COS composes the selected implementation. Packages must not depend upward on the composition host, and peer capabilities should not become hard assembly dependencies merely because they are used together in an experience.

MicroBundleRepository remains the durable delivery boundary for **published, versioned MicroBundle artifacts**. It stores and verifies opaque artifact bytes; it is not a mutable project database and should not be repurposed to store every user edit. User-authored projects require a distinct editable-asset persistence contract.

## 02. The source model: a block without a million objects

A voxel volume should be logically regular but physically chunked. A naive array of a million heavyweight C# voxel objects would squander memory before rendering begins.

### Initial representation

- Define a volume's dimensions, cell size, local origin/transform, schema version, and default material.
- Store occupancy compactly (for example, bit-packed or byte-packed cell state) and material assignment separately or in a compact indexed encoding.
- Partition the volume into chunks/bricks. Allocate or materialize chunks as needed; track each chunk's content revision.
- Give the whole asset a stable identity independent of its current mesh, bounds, or position in an editor.
- Keep source data deterministic and CPU-readable. GPU buffers and meshes are caches, never the only copy of the model.

The implementation should benchmark sparse and dense patterns before choosing a single storage encoding. A dense solid block is a poor fit for a per-cell heap object; a mostly empty sculpted space is a poor fit for needlessly allocating every chunk. The public contract should not expose a particular bit layout.

### Edits are semantic operations

An edit should describe intent and its affected region, for example:

- subtract material with a brush volume;
- add/fill material;
- replace material assignment without changing occupancy;
- smooth or relax a boundary;
- apply a stroke or a sequence of sampled brush positions;
- undo/redo a prior operation.

Operations should have explicit parameters, stable ordering, and a known affected region. This supports deterministic replay, dirty-region detection, undo/redo, autosave, and later collaboration. A brush operation must be bounded and validated before it mutates the model.

## 03. Mesh derivation is a cache

The model is not a triangle list. Mesh generation is a derived-data pipeline:

1. An edit updates authoritative source cells and increments the relevant revisions.
2. Only intersecting chunks and their required neighbors are marked dirty.
3. Surface extraction emits faces between occupied and empty cells, respecting material boundaries.
4. Adjacent coplanar faces can be merged (greedy meshing) to avoid emitting a separate quad for every exposed cell face.
5. Normals, material regions, bounds, and optional collision proxies are produced from the same source revision.
6. Renderer/Visualize receive a versioned derived result; stale results are discarded rather than overwriting newer edits.

For the first hard-edged carving prototype, use chunked occupancy and greedy surface meshing. This is a straightforward route to visible, inspectable results. Smooth sculpting is a later representation mode—such as a signed-distance field with dual contouring or marching cubes—not a reason to force every block-carving use case into a smooth-field representation.

Mesh simplification, LODs, collision geometry, and GPU residency are downstream products with independent revision/freshness policies. They must never silently replace the editable source.

## 04. Persistence: the missing Workshop capability

We need to distinguish three different kinds of storage:

1. **Live editing state** — current in-memory model and pending operations.
2. **Editable project persistence** — save/reopen, autosave, undo history, schema migration, recovery, and potentially concurrent edits.
3. **Published artifacts** — immutable, versioned, content-addressed tools or finished exports.

Only the third category is the MicroBundleRepository's existing responsibility. Do not confuse a published tool package with the user's editable sculpture.

### Proposed editable-asset contract

Introduce a separate boundary (name provisional: `IEditableAssetStore`) with operations equivalent to:

- create or open a project by stable asset ID;
- load a consistent snapshot and its schema/version metadata;
- append or checkpoint ordered edit operations;
- save a new snapshot with an expected revision;
- enumerate revisions and restore a prior revision;
- export a self-contained package for transfer or publication.

A stored project should include source data, asset metadata, material palette/references, operation journal or undo checkpoints, schema version, and checksums. Meshes may be cached alongside it, but cache corruption or cache-version mismatch must be recoverable by rebuilding from the source.

Use snapshot plus journal as the initial design candidate: periodic snapshots bound replay time, while the journal preserves recent editing history. The store should use optimistic revision checks so two writers cannot silently overwrite each other. Exact blob layout, database choice, and conflict UX remain open until the contract and recovery tests exist.

Azure Blob Storage can be a persistence substrate for snapshots and immutable revisions, but Blob Storage itself is not the asset model or conflict-resolution policy. The MicroBundleRepository may inspire verified content-addressed storage practices; its published-artifact API should remain separate.

### Persistence acceptance tests

- Save, close, reopen, and reproduce the same cell state and material assignments.
- Replay the same ordered edit journal and obtain the same source revision/content hash.
- Recover after an interrupted write without mixing data from two revisions.
- Reject a stale expected revision instead of silently clobbering a newer edit.
- Load an older schema through an explicit migration or report a clear unsupported-version error.
- Delete or corrupt a derived mesh cache and rebuild it from saved source.
- Export/import a project without depending on the editor's in-memory objects or GPU handles.

## 05. Drawing and painting are neighboring tools

Drawing and painting should share project identity, persistence, selection, undo/redo, and material vocabulary where that is semantically appropriate. They should not be collapsed into voxel sculpting.

- **2D drawing:** vector strokes, paths, fills, layers, transforms, and optional raster layers.
- **3D surface painting:** brush strokes projected onto a surface, with stable mapping to surface/asset coordinates and a defined response when the underlying mesh changes.
- **Voxel/material painting:** change cell material assignments without changing occupancy.
- **Annotations:** dimensions, guides, labels, reference images, and notes that may remain non-destructive overlays.
- **Color/material authoring:** distinguish display color from a physically meaningful material definition; a painted red surface is not automatically a material with particular roughness, reflectance, or thermal properties.

The first shared primitive should be a versioned, serializable **stroke/operation** contract, not a universal drawing UI. A stroke records tool identity/type, parameters, coordinate space, ordered samples, target asset/layer, and affected region. Adapters can then manifest the same semantic tool in desktop, browser, or immersive hosts.

## 06. First proof: carve, derive, save, reopen

The smallest end-to-end proof should be intentionally modest:

1. Create a block with a documented grid size and cell size.
2. Show the block with a clear wire/grid overlay and a visible brush cursor.
3. Subtract a spherical or box-shaped brush from one face.
4. Derive a mesh from the changed chunk and show the cavity.
5. Paint a material onto a bounded region without changing occupancy.
6. Undo and redo both operations.
7. Save the source model, close the session, reopen it, and regenerate the same surface.
8. Display source revision, dirty chunk count, derived mesh revision, cell/chunk counts, and save status.

The proof is not successful merely because a mesh appears. It is successful when the visible surface can be traced back to durable source data, and the same model can be edited again after reopening.

## 07. Implementation gates

- **Gate A — Contract:** define dimensions, coordinate conventions, stable IDs, cell/material semantics, operation records, and error behavior.
- **Gate B — CPU prototype:** chunk storage, carve/fill/material edit, deterministic tests, and source revisioning.
- **Gate C — Surface proof:** greedy meshing for changed chunks, seam tests across chunk boundaries, and a living visualization derived from actual model data.
- **Gate D — Persistence:** snapshot/journal format, revision checks, save/reopen, interrupted-write recovery, and cache rebuild.
- **Gate E — Editor experience:** tool selection, brush feedback, undo/redo, drawing/painting tools, and responsive incremental updates.

Do not claim million-cell authoring performance from the target cell count alone. Benchmark memory, edit latency, meshing time, allocation, save size, and reopen/rebuild time across dense, sparse, and heavily edited models. Start with a manageable volume and scale only after measurements.

## 08. Open decisions

- Initial supported cell formats and chunk dimensions.
- Whether the first project store is local filesystem, cloud-backed, or a local-first adapter over a shared contract.
- Snapshot frequency and edit-journal compaction.
- Surface painting representation: texture layers, per-face/cell material assignments, or both.
- Smooth-volume representation and conversion fidelity.
- Collaboration and merge semantics.
- Import/export formats and asset licensing/attribution metadata.

These are explicit design decisions, not reasons to delay the first carve/mesh/save/reopen proof.
