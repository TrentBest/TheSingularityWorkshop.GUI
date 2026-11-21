## SingularityBot Studio

A standalone browser-based operational GUI host built in this repository. It uses the GUI Blazor adapter and renderer-neutral semantic model, and is intentionally launched independently of any LLM session.

### Run locally

From the repository root:

```powershell
dotnet run --project samples/BotStudioWebApp/BotStudioWebApp.csproj
```

Open the localhost URL printed by ASP.NET Core.

### Current boundary

The first milestone establishes a durable web application shell and match-configuration surface. It does **not** yet control a SingularityBot process: the dashboard explicitly reports the local bridge as offline, disables match launch, and does not fabricate match telemetry.

The next integration milestone is a narrow local bot-control bridge with:

- explicit process lifecycle and health reporting;
- validated match-start requests mapped to existing compiled bot modes;
- read-only match snapshots and structured event streaming;
- allow-listed, validated operator commands;
- fail-closed behavior when the bot, SC2 session, or bridge disconnects.

The bot process must remain independently launchable. The browser UI and any future LLM-assisted authoring layer are observers/controllers, not prerequisites for bot startup.

### Architecture

- `TheSingularityWorkshop.GUI.Core` owns the renderer-neutral semantic model.
- `TheSingularityWorkshop.GUI.Blazor` manifests the semantic header in Blazor.
- This sample owns the application shell, browser interaction, and future local transport adapter.
- SingularityBot remains the authoritative owner of SC2 state and gameplay decisions.
