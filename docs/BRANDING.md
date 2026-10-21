# GUI Documentation Branding

This document adapts the Workshop's shared [Documentation Standard](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/development/DOCUMENTATION_STANDARD.md) to the responsibility of this repository. Shared identity should help readers recognize the Workshop; it must not make every package sound like the same package.

## The GUI's identity

**Making software meaning visible to people.**

GUI is the boundary where semantic intent becomes something a person can inspect and interact with. Its domain voice should emphasize meaning, manifestation, and human understanding—not claim ownership of the runtime behavior being represented.

- **Core** describes platform-neutral GUI intent.
- **Adapters** manifest that intent through platform-specific mechanisms.
- **The consuming experience** owns its domain meaning, execution, and behavior.
- **Visuals** help a reader understand the model or observe an implementation. They must state which one they are doing.

## Shared Workshop identity

- Place the animated Workshop moniker at the very top of Markdown documents, before the title or other content. Use the canonical shared asset rather than creating near-identical copies: https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Renderer/development/docs/assets/moniker.svg
- Use a consistent, linkable text-based HTML footer. The footer may include the shared Workshop avatar, but the identity, tagline, and navigation must remain readable text and real links—not be baked into an SVG image.
- Keep the README's common opening sequence where relevant: ✳️ 00 identity, a useful money-shot visual, 🟦 01 What, 🟣 02 Why, 🩵 03 How, 🟢 04 See it in a minute, and 🟪 05 Documentation and theory.
- Treat markers as redundant visual cues: number and heading text carry the meaning too. Do not rely on color alone.
- Keep each deep document's title and subject focused. The common brand should frame the explanation, not replace it with repeated boilerplate.

## GUI-specific visual language

The most persuasive GUI visual is a truthful manifestation of semantic intent. Prefer, in order where practical:

1. A reproducible output from the actual adapter or running host.
2. A source-derived diagram that accurately describes current code and contracts.
3. An explanatory concept image, explicitly labeled as conceptual.

Use the form that teaches the concept: a model/tree for semantic structure, a sequence for transitions and ownership, SVG output for inspectable manifestation, and a live capture for behavior that genuinely runs. Renderer and Visualize may offer different ways to see a world; neither should be implied by GUI's existence alone.

Every visual should answer: **What am I looking at? What does it demonstrate? What does it not prove?** Link implementation or test evidence for claims about behavior. Distinguish generated SVG from authored SVG, and adapter output from a live runtime or GPU pipeline. Animation should explain a meaningful relationship and respect reduced-motion preferences.

## Standard footer

Use the footer below as a consistent starting point. Keep navigation relevant to the destination document, and verify every link on the target branch.

    <p align="center">
      <a href="https://github.com/TrentBest/TheSingularityWorkshop.GUI" aria-label="Visit The Singularity Workshop GUI repository">
        <img src="https://raw.githubusercontent.com/TrentBest/WebPage/development/TheSingularityWorkshop/wwwroot/Images/TheSingularityWorkshopLivingAvatar.svg" alt="The Singularity Workshop living avatar — a digital vortex of information converging on its central eye" width="96" />
      </a><br />
      <strong>The Singularity Workshop · GUI</strong><br />
      <em>Making software meaning visible to people.</em><br />
      <a href="https://github.com/TrentBest/TheSingularityWorkshop.GUI">Repository</a> · <a href="https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/docs/START_HERE.md">Explore GUI</a> · <a href="https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/samples/demonstrations/GALLERY.md">Visual Gallery</a> · <a href="https://github.com/TrentBest/TheSingularityWorkshop.GUI/issues">Issues and ideas</a>
    </p>

## Review checklist

- [ ] Shared moniker is first and points to the canonical asset.
- [ ] The title and prose accurately describe this document's own subject.
- [ ] Footer identity and links are readable, relevant, and valid.
- [ ] README section markers follow the shared semantic palette where applicable.
- [ ] Visuals are evidence-labeled and distinguish implementation from concept.
- [ ] GUI does not claim ownership of application execution or domain behavior.
- [ ] The brand supports clarity and invitation rather than hype or gatekeeping.

This standard is a guide for the branding pass, not a claim that every Markdown file has already been migrated. Track and verify the remaining document inventory separately rather than declaring full coverage.
