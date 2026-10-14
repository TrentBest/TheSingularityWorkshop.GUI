# Living Documentation

Documentation should demonstrate GUI architecture where motion or spatial relationships improve understanding.

![One semantic GUI, multiple manifestations](images/gui-svg-manifestation.svg)

## Principles

- **Truth before spectacle.** A diagram may illustrate a target, but must not imply the target is implemented.
- **Motion has a purpose.** Animate a transition or relationship, not every element by default.
- **Respect reduced motion.** Animated SVG examples should include a reduced-motion fallback.
- **Accessible by default.** Give meaningful visuals titles and descriptions and preserve readable contrast.
- **Link concept to code.** Implementation claims should point to source or tests where possible.
- **Keep source reviewable.** Prefer understandable SVG over opaque generated markup.

The architecture SVG above is explanatory, not runtime output from the SVG renderer. The [living energy-field example](examples/living-energy-vortex.svg) is a separate motion study: it demonstrates animation and reduced-motion handling, not application state or physically accurate lighting.

## Examples

- [Semantic GUI manifestations](images/gui-svg-manifestation.svg) — animated relationship diagram for Core, SVG, Blazor, and WPF.
- [Living energy field](examples/living-energy-vortex.svg) — alternating pulse and orbit study with a reduced-motion fallback.

## Document audit checklist

- [ ] Purpose and audience are clear.
- [ ] Implementation claims are checked against current source.
- [ ] Code samples match the public API.
- [ ] Links are appropriate and checked.
- [ ] A visual is added where it materially improves comprehension.
- [ ] Included visuals have accessible labeling and reduced-motion handling when animated.
- [ ] Planned capabilities are separated from implemented behavior.
- [ ] The document links to relevant implementation and tests.

Not every page needs a repeated decorative SVG. The goal is to visualize concepts whenever that improves understanding without adding noise or implying unsupported behavior.
