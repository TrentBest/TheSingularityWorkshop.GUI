# WPF Builder Pattern Audit

This audit compares the WPF adapter in this repository with existing WPF builder implementations in the Workshop ecosystem. It is a transfer plan, not a claim that every feature from those projects belongs in this adapter.

## Sources inspected

- [SingularityWarehouse: `GuiBuilderWPF`](https://github.com/TrentBest/SingularityWarehouse/blob/10a36c0aa63653989abc17f38308274aad0cda76/Core.WPF/Builders/GuiBuilderWPF.cs) — fluent window configuration, native-control defaults, resource-key fallbacks, scrollable content, resize grip, ownership, and status/debug hooks.
- [SingularityWarehouse: `ReflectiveGuiBuilder`](https://github.com/TrentBest/SingularityWarehouse/blob/10a36c0aa63653989abc17f38308274aad0cda76/Core.WPF/Builders/ReflectiveGuiBuilder.cs) — reflection-driven model inspection, primitive editors, collection tools, and nested-object navigation.
- [SingularityWarehouse: `MultiPanelGuiBuilderWPF`](https://github.com/TrentBest/SingularityWarehouse/blob/10a36c0aa63653989abc17f38308274aad0cda76/Core.WPF/Builders/MultiPanelGuiBuilderWPF.cs) — composition and communication across sequenced panels.
- [RevitFamilyManagerBuilders: `GuiBuilderWPF`](https://github.com/TrentBest/RevitFamilyManagerBuilders/blob/5cb051405db33fbb485163540046de453a7eeaae/src/RevitFamilyManager.Builders/Abstractions/GuiBuilderWPF.cs) — reuses a native element builder and separates configurable window chrome/resource fallbacks from content construction.
- [WebPage: shared `GuiBuilder`](https://github.com/TrentBest/WebPage/blob/0110fb6c548abfefd53ac40afb131eaaacee482c/TheSingularityWorkshop/Workshop/Gui/GuiBuilder.cs) — neutral tree construction intended to feed different renderers.
- Current adapter: [`NativeGuiBuilder<T>`](../src/WPF/NativeGuiBuilder.cs), [`GuiPanelBuilderWPF<TPanel,TBuilder>`](../src/WPF/GuiPanelBuilderWPF.cs), [`StackGuiPanelBuilderWPF`](../src/WPF/StackGuiPanelBuilderWPF.cs), and [WPF conformance tests](../tests/WPF.Tests/).

## What transfers well

1. **System-native defaults with fluent overrides.** `NativeGuiBuilder<T>` already seeds WPF controls from `SystemColors` and `SystemFonts`, then exposes focused fluent methods. Regression tests now exercise both those defaults and overrides, including dimensions, spacing, alignment, and identity stability.
2. **A single declaration roster for repeatable builds.** `GuiPanelBuilderWPF<TPanel,TBuilder>` queues declarations and rebuilds the same root panel. A regression test now verifies fluent subclass type retention, child order, and no duplicate children after repeated `Build()` calls.
3. **Composition at the adapter boundary.** Existing builders demonstrate that panels can compose child builders. Keep parent/child relationships and any future event or message bus explicit and testable instead of silently introducing app-global state.
4. **Theme resource keys with system fallbacks.** The older builders use resource keys such as `FTIBackground` and `FTIBorder` with fallback brushes. This can be a separate opt-in theme adapter later; app-specific resource names should not become mandatory defaults in a reusable NuGet package.
5. **Window lifecycle/chrome as a separate concern.** Ownership, startup location, transparency, taskbar behavior, resizing, and close/maximize controls are useful host features, but they should live in a distinct window-host builder rather than being coupled to the neutral `GuiNode` renderer.

## What should not be copied blindly

- Do not bring Revit, Inventor, warehouse services, application-specific resource dictionaries, or logging dependencies into this package.
- Do not treat reflection over arbitrary objects as a safe editor contract. A future reflective builder needs explicit member-selection policy, depth/cycle protection, and tests for read-only/indexer properties and side-effecting getters.
- Do not confuse window chrome or a fluent WPF-specific construction API with cross-adapter parity. The WPF renderer remains a projection of the platform-neutral GUI model.
- Do not promise interaction, accessibility, command/event wiring, persistence, or full layout parity until those contracts are implemented and tested.

## Next WPF work

- [x] Add STA tests for native defaults and fluent overrides.
- [x] Verify repeatable stack-panel construction without duplicate children.
- [ ] Expand tests for grid track sizing/splitters, canvas placement, and nested component parent propagation.
- [ ] Add an actual WPF living sample that demonstrates the same neutral model rendered by WPF and links each visible feature to its tests.
- [ ] Decide whether a separate reusable window-host builder is in this release scope.
- [ ] Verify package README/XML docs, metadata, dependencies, and archive contents if WPF is selected for release.

## Verification boundary

The new builder tests are committed to `development`; check the linked Windows `test-wpf` job before treating them as verified. The renderer tests and these fluent builder tests are separate surfaces and should be reported separately in release notes.
