# GUI Visual Reference

The GUI project should preserve useful visual artifacts as **reference manifestations**, not as semantic contracts.

The first reference image is the Workshop map produced during the current WebPage exploration. It is useful because it demonstrates a concrete manifestation of several concepts the neutral GUI model is intended to represent:

- a large recursive interface rather than a single widget;
- navigation surfaces and spatial information;
- a map/scene region alongside an indexed information surface;
- selectable destinations and operational controls;
- textual, geometric, and spatial representation in one experience;
- a distinction between the semantic experience and the eventual platform manifestation.

## Asset convention

When the source image is committed, place it at:

`docs/assets/workshop-map-reference.jpg`

The filename is intentionally semantic rather than generator-specific. The image is a reference artifact; it does not define the Core GUI model and should not be treated as a pixel-perfect implementation contract.

## Why it belongs here

This image is not merely artwork. It is an early **digital shadow** of the Workshop experience.

The important engineering question is therefore not "how do we reproduce these pixels?" It is:

> Can the semantic GUI model describe the observable structure represented by this image, and can multiple adapters manifest that structure without changing its meaning?

That makes visual references useful as future golden/reference artifacts while keeping Core platform-neutral.

## Intended progression

1. Preserve the visual reference.
2. Identify its semantic elements independently of HTML/CSS/WPF/Unity/etc.
3. Express those semantics through Core.
4. Manifest the same semantic artifact through an adapter.
5. Compare the resulting manifestation against the reference where appropriate.

The image therefore becomes evidence for expressiveness, not an implementation dependency.
