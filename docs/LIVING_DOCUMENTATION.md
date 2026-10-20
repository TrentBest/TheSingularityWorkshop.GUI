# Living Documentation

GUI documentation should use visuals when spatial relationships, state transitions, or architectural boundaries are easier to understand visually. The visual must clarify the implementation—not imply capabilities the source does not provide.

## Principles

- **Truth before spectacle.** Clearly distinguish implemented behavior, proposals, and authored concepts. An illustrative SVG is not proof of renderer output.
- **Motion has a purpose.** Animate meaningful transitions or relationships; avoid motion that adds no information.
- **Respect reduced motion.** Animated examples should provide a reduced-motion fallback.
- **Accessibility is part of the artifact.** Give meaningful SVGs titles and descriptions, preserve readable contrast, and verify local references.
- **Link claims to evidence.** Implementation claims should point to current source, tests, or reproducible hosted output where possible.
- **Keep generated and authored assets distinct.** Do not overwrite richer authored examples with generated output unless equivalence has been demonstrated.
- **Keep source reviewable.** Prefer understandable SVG and deterministic generated documentation over opaque output.

## Review checklist

- [ ] Purpose and audience are clear.
- [ ] Implementation claims match the current source and tests.
- [ ] Code samples match the public API.
- [ ] Links and local SVG references resolve.
- [ ] Visuals materially improve understanding rather than decorate the page.
- [ ] Animated visuals account for reduced-motion preferences.
- [ ] Planned capabilities are separated from implemented behavior.
- [ ] Generated output is not presented as runtime proof unless it came from the actual framework path.
- [ ] Relevant implementation and test evidence is linked.

Not every page needs a repeated decorative SVG. The goal is to make complex behavior legible without adding noise or implying unsupported behavior.

## Existing visual concepts

See [Visual concepts](visual-concepts/README.md) for exploratory artwork. Those files are explicitly authored concepts, not generated output from `SvgGuiRenderer` and not evidence of framework runtime behavior. For current adapter behavior, see the [SVG guide](SVG_GUIDE.md).
