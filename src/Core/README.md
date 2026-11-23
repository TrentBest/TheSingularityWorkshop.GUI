# TheSingularityWorkshop.GUI.Core

The platform-neutral model and recursive builder primitives for The Singularity Workshop GUI.

**Package:** `TheSingularityWorkshop.GUI.Core`  
**Current project target:** `.NET 10.0`

## What this package provides

- `GuiNode`: an immutable recursive description with kind, stable string identity, optional text/source, a read-only property map, and ordered children.
- `GuiBuilder`: a fluent way to construct a tree and produce a `GuiNode` snapshot.
- `BarChartDefinition`, `BarChartCategory`, and `BarChartSeries`: renderer-neutral categorical chart data with stable identity, aligned finite/missing values, axis labels, units, and an accessible description.
- Typed `TextInputDefinition`, `SelectDefinition`, and `ToggleDefinition` value controls with stable host command IDs.
- `BadgeDefinition` and ordered immutable `BadgeSet` for semantic document badges.
- Explicit repository-relative or HTTPS `GuiAssetReference` and `GuiLinkReference` values.
- Tree lookup through `Find` and `TryFind`.

Core does not render controls, own a UI lifecycle, or execute platform callbacks. Value-control definitions describe values and command identities only; the consuming host authorizes and handles changes. Badge definitions describe roles and assets without depending on Markdown, GitHub, a filesystem, or a particular static-site generator. Use adapter packages to turn those neutral contracts into platform-specific or document-specific representations.

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

## Categorical bar chart data

```csharp
var chart = new BarChartDefinition(
    "quarterly-sales",
    "Quarterly sales",
    "Sales by quarter in thousands of dollars.",
    new[] { new BarChartCategory("q1", "Quarter 1"), new BarChartCategory("q2", "Quarter 2") },
    new[] { new BarChartSeries("revenue", "Revenue", new double?[] { 120, null }) },
    categoryAxisLabel: "Quarter",
    valueAxisLabel: "Revenue",
    valueUnit: "USD thousands");
```

A missing value is `null`, not zero. Series values align by index with the chart category sequence. Empty charts are valid; duplicate identities, non-finite values, and mismatched series lengths are rejected. See [BarChartDefinition](BarChartDefinition.md) for the contract and current limits. This is a data model, not a renderer or a claim of adapter support.

## Semantic document badges

```csharp
var badges = BadgeSet.Empty
    .WithBadge(new BadgeDefinition(
        "lod-1",
        BadgeRole.Classification,
        "LOD 1",
        "LOD 1 classification badge",
        GuiAssetReference.Repository("docs/assets/rulebook-lod-1.svg")))
    .WithBadge(new BadgeDefinition(
        "sourcebook-cover",
        BadgeRole.ArtifactIdentity,
        "Dungeons and Underground",
        "Dungeons and Underground sourcebook cover",
        GuiAssetReference.Repository("docs/books/covers/dungeons-and-underground.svg")));
```

The order is intentional: classification and artifact identity are separate roles. The Markdown adapter receives a target document path and resolves both assets relative to that file.

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
