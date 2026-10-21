![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Renderer/development/docs/assets/moniker.svg)

# GUI Documentation Branding

This guide adapts the shared Workshop documentation standard to the responsibility of this repository. Shared identity should help readers recognize the Workshop without making every package sound like the same package.

## The GUI identity

**Making software meaning visible to people.**

GUI is the boundary where semantic intent becomes something a person can inspect and interact with. Its voice should emphasize meaning, manifestation, and human understanding without claiming ownership of runtime behavior represented by the interface.

- **Core** describes platform-neutral GUI intent.
- **Adapters** manifest that intent through platform-specific mechanisms.
- **The consuming experience** owns its domain meaning, execution, and behavior.
- **Visuals** help a reader understand a model or observe an implementation. Every picture should state which it is doing.

## Shared Workshop identity

- Place the animated Workshop moniker at the top of Markdown documents, before the title, using the canonical shared asset.
- Use a consistent, linkable text footer where appropriate. Identity, tagline, and navigation must remain readable text and real links rather than being baked into an image.
- Keep the README reader journey where relevant: identity, a useful visual, What, Why, How, See it in a minute, and Documentation and theory.
- Treat visual markers as redundant cues. Heading text carries the meaning too; never rely on color alone.
- Keep deep-document titles focused. Shared branding frames the explanation rather than replacing it with boilerplate.

Canonical moniker:
https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Renderer/development/docs/assets/moniker.svg

## GUI-specific visual language

The most persuasive GUI visual is a truthful manifestation of semantic intent. Prefer, where practical:

1. Reproducible output from the actual adapter or running host.
2. A source-derived diagram that accurately describes current code and contracts.
3. An explanatory concept image explicitly labelled as conceptual.

Use the form that teaches the concept: a model/tree for semantic structure, a sequence for transitions and ownership, SVG output for inspectable manifestation, and a live capture for behavior that genuinely runs. Renderer and Visualize may offer different ways to see a world; neither should be implied by GUI's existence alone.

Every visual should answer: **What am I looking at? What does it demonstrate? What does it not prove?** Link implementation or test evidence for claims about behavior. Distinguish generated SVG from authored SVG and adapter output from a live runtime or GPU pipeline. Animation should explain a meaningful relationship and respect reduced-motion preferences.

## Suggested footer

Keep navigation relevant to the destination document and verify every link on the target branch.

<p align="center">
  <a href="https://github.com/TrentBest/TheSingularityWorkshop.GUI" aria-label="Visit The Singularity Workshop GUI repository">
    <img src="https://raw.githubusercontent.com/TrentBest/WebPage/development/TheSingularityWorkshop/wwwroot/Images/TheSingularityWorkshopLivingAvatar.svg" alt="The Singularity Workshop living avatar" width="96" />
  </a><br />
  <strong>The Singularity Workshop · GUI</strong><br />
  <em>Making software meaning visible to people.</em><br />
  <a href="https://github.com/TrentBest/TheSingularityWorkshop.GUI">Repository</a> · <a href="https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/docs/START_HERE.md">Explore GUI</a> · <a href="https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/docs/GALLERY.md">Visual Gallery</a> · <a href="https://github.com/TrentBest/TheSingularityWorkshop.GUI/issues">Issues and ideas</a>
</p>

## Review checklist

- [ ] Shared moniker is first and points to the canonical asset.
- [ ] The title and prose accurately describe the document's subject.
- [ ] Footer identity and links are readable, relevant, and valid.
- [ ] README section markers follow the shared semantic palette where applicable.
- [ ] Visuals are evidence-labelled and distinguish implementation from concept.
- [ ] GUI does not claim ownership of application execution or domain behavior.
- [ ] Branding supports clarity and invitation rather than hype or gatekeeping.

This is a guide for the branding pass, not a claim that every Markdown file has already been migrated. Track and verify the remaining document inventory separately.
