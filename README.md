# TheSingularityWorkshop.GUI

Platform-specific recursive GUI builders.

This repository is the home for GUI construction infrastructure that should not live inside a consuming application.

## Architecture

- **Core** — platform-neutral recursive GUI tree and coordinate primitives.
- **Blazor** — Blazor manifestation and fluent web-facing builders.
- **WPF** — WPF manifestation and fluent desktop builders.
- Future platform adapters can be added without moving application/domain code into the builder repository.

The consuming application describes GUI intent. The platform builder materializes that intent.

The WebPage repository is the first migration target for the Blazor builders. RevitFamilyManagerBuilders remains unchanged; its WPF builders are being migrated here as the reusable source for forward-moving applications.
