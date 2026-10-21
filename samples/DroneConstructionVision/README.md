# Drone Construction Vision — GUI.SVG sample

This sample creates a platform-neutral GUI node scene and manifests it with SvgGuiRenderer. It is the first actual GUI.SVG scene model for the Drone Construction idea.

The output is a concept storyboard, not live simulation telemetry. It explains the target image, target grid, ten emitters, construction agents, and the cellular traffic model. Once the deterministic simulation loop exists, this scene can be driven from real snapshots rather than fixed illustration data.

Run from the repository root:

    dotnet run --project samples/DroneConstructionVision/DroneConstructionVision.csproj -- docs/assets/drone-construction-vision.svg

The sample references the local Core and SVG projects so the renderer contract is exercised from source rather than depending on an unpublished package.
