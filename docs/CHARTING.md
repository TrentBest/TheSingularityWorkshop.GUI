# Renderer-neutral charting and SVG manifestation

## First vertical slice

Core's `BarChartDefinition` describes chart identity, accessible title/description, ordered categories, one or more named series, nullable observations, axis labels, and a value unit. It does not own colors, typography, layout, aggregation, sorting, or renderer behavior.

GUI.SVG currently manifests a deliberately narrow subset through `BarChartSvgRenderer`:

| Capability | Current SVG behavior |
|---|---|
| Single-series categorical vertical bars | Supported |
| Positive and negative values | Supported; bars are drawn relative to a visible zero baseline |
| Missing observations | Supported; no bar is drawn and the category is explicitly marked missing |
| Empty categories/series or all-missing observations | Supported; explicit “No data available” state |
| Category labels, series label, value labels, axis labels and unit | Supported |
| Accessible title and description | Supported; the description includes a textual category/value summary |
| Multiple series / grouped bars | Rejected with `NotSupportedException` until grouping semantics are defined |
| Stacked bars | Unsupported |
| Horizontal bars | Unsupported |
| Line/time-series, area, pie/donut, scatter, histogram, range/band, gauge | Not implemented |

This is a capability statement for this SVG renderer, not a claim of parity with Blazor or WPF. The SVG adapter intentionally fails closed for multiple series rather than silently overlaying or stacking values. Core accepts multiple series as data; a renderer must not invent their composition semantics.

## Runnable example

Run:

```sh
dotnet run --project samples/BarChartGallery/BarChartGallery.csproj --configuration Release -- ./artifacts/bar-chart-gallery.svg
```

The sample creates a four-quarter revenue chart with an intentionally missing Q2 observation. The generated SVG is a real output of `BarChartSvgRenderer`, not a static illustration. The Package GUI workflow generates, structurally verifies, and uploads this artifact as `gui-svg-bar-chart-gallery`.

## Accessibility and fallback

The SVG document exposes a title and description through `aria-labelledby`. The description includes the authored explanation and an ordered text summary of every category/value pair, including missing observations. Visible labels remain in the SVG for inspection and non-interactive consumers. This is a static chart; no hover, selection, keyboard interaction, or live data binding is claimed.

## Validation

The SVG test suite covers accessible metadata, category/value labels, missing-value omission, negative values relative to zero, empty/all-missing states, multi-series rejection, dimensions, and XML text escaping. Core owns input validation (identity, duplicate IDs, finite values, and category/value alignment); the SVG adapter owns manifestation-specific behavior.

## Boundaries and next decisions

The first slice does not settle grouped/stacked semantics, axis scaling policy for every domain, numeric/time axes, legends for multiple series, interactive accessibility, or cross-adapter capability negotiation. Add those as explicit contracts and tests before advertising them. Package boundaries remain unchanged; no chart-specific package is introduced.
