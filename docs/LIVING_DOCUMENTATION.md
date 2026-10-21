# Living Documentation

A **living picture** (or **living image**) is an explanation that demonstrates the behavior of the thing it describes. It is more than a static illustration: it makes meaningful state, sequence, scale, cause, and effect observable to a reader. When appropriate, narration should explain what is happening and why, including for people who are listening rather than watching.

A living picture must be honest about its evidence. It may be a conceptual animation, a source-derived diagram, a rendering produced by the real framework, or a live demonstration. Label which kind it is. Never imply that an authored concept or animation proves runtime behavior when it does not.

## Examples of living pictures

- **FSM_COS assembly:** Show a crane lifting one component into position, attaching it, and then collecting the next. The sequence should communicate the manifest-driven assembly order and the responsibility of each component. If the image is illustrative, say so; if it is driven by actual manifest or runtime data, identify that source.
- **Finite-state-machine transition:** Highlight the current state with a gentle breathing pulse. Show the relevant condition or conditions being evaluated, then the current state's exit action, the transition, and the next state becoming current. Represent lifecycle flags accurately: for example, do not show the new state's `HasEntered` flag as true until the implementation actually sets it. The sequence should be grounded in source and tests whenever it claims to demonstrate FSM_API behavior.
- **Quantities and scale:** Use counts, proportional sizes, grouped objects, axes, or small multiples to make magnitudes and differences legible. Label units, denominators, sample sizes, and whether values are measured, calculated, illustrative, or hypothetical. A thousand items should look meaningfully different from ten; do not use decorative motion as a substitute for scale.
- **Architecture and data flow:** Show which component supplies data, which component transforms it, and which component presents it. Animate only the relationships or steps that help explain the contract.

## Principles

- **Truth before spectacle.** Clearly distinguish implemented behavior, proposals, and authored concepts. An illustrative SVG is not proof of renderer output.
- **Motion has a purpose.** Animate meaningful transitions or relationships; avoid motion that adds no information.
- **Narration explains causality.** Spoken or written narration should describe the current step, what caused it, and what changes next. Do not rely on color, movement, or sound alone to communicate essential meaning.
- **Respect reduced motion.** Animated examples should provide a reduced-motion fallback.
- **Accessibility is part of the artifact.** Give meaningful SVGs titles and descriptions, preserve readable contrast, and verify local references.
- **Link claims to evidence.** Implementation claims should point to current source, tests, or reproducible hosted output where possible.
- **Keep generated and authored assets distinct.** Do not overwrite richer authored examples with generated output unless equivalence has been demonstrated.
- **Keep source reviewable.** Prefer understandable SVG and deterministic generated documentation over opaque output.

## Review checklist

- [ ] Purpose and audience are clear.
- [ ] The picture teaches a behavior, relationship, sequence, or scale—not just a visual theme.
- [ ] Each meaningful visual change has an understandable cause and consequence.
- [ ] Narration or accompanying text explains the scene for people who cannot rely on the animation.
- [ ] Implementation claims match the current source and tests.
- [ ] Code samples match the public API.
- [ ] Links and local SVG references resolve.
- [ ] Visuals materially improve understanding rather than decorate the page.
- [ ] Animated visuals account for reduced-motion preferences.
- [ ] Planned capabilities are separated from implemented behavior.
- [ ] Generated output is not presented as runtime proof unless it came from the actual framework path.
- [ ] Quantities identify units and distinguish measured data from illustrative values.
- [ ] Relevant implementation and test evidence is linked.

Not every page needs a repeated decorative SVG. The goal is to make complex behavior legible without adding noise or implying unsupported behavior.

## Existing visual concepts

See [Visual concepts](visual-concepts/README.md) for exploratory artwork. Those files are explicitly authored concepts, not generated output from `SvgGuiRenderer` and not evidence of framework runtime behavior. For current adapter behavior, see the [SVG guide](SVG_GUIDE.md).
