## SingularityBot Studio

A standalone browser-based operational GUI host built in this repository. It uses the GUI Blazor adapter and renderer-neutral semantic model, and is intentionally launched independently of any LLM session.

### Run locally

From the repository root:

```powershell
dotnet run --project samples/BotStudioWebApp/BotStudioWebApp.csproj
```

Open the localhost URL printed by ASP.NET Core.

### Connect to SingularityBot

Start the bot Runner on the **same computer** as the Bot Studio web host:

```powershell
dotnet run --project src/SingularityBot.Runner/SingularityBot.Runner.csproj -- --studio-bridge
```

The Runner prints its loopback bridge URL and a fresh bearer token. In Bot Studio, use the default bridge address `http://127.0.0.1:43127`, paste the token into the password field, and select **Connect read-only**.

The integration currently performs:
- an unauthenticated loopback health check;
- authenticated `GET /api/snapshot` and `GET /api/events?afterSequence=N&limit=100` requests;
- bounded two-second polling for snapshots and ordered event replay while the dashboard remains connected;
- a 100-event in-memory UI history, with the bridge retaining its bounded 256-event journal;
- manual disconnect and automatic polling shutdown after transport failure.

The Blazor server makes the HTTP requests to the loopback bridge. The web host itself rejects non-loopback clients, and the browser does not call the bridge directly, so bridge CORS remains disabled. This requires the Runner and the Bot Studio server process to run on the same computer; a static GitHub Pages deployment cannot connect to a user's local loopback bridge.

### Security and runtime boundary

- The bridge address is restricted to `http://127.0.0.1`; arbitrary hosts and remote addresses are rejected.
- The bearer token is entered manually and held only in the interactive server circuit's memory. It is not written to browser storage, URLs, or application logs.
- Disconnecting the UI stops its polling only. It does not stop, pause, or otherwise control the bot process.
- The bridge exposes health, authenticated read-only snapshots, and authenticated bounded event replay only. No match-start, command, script, or arbitrary execution endpoint is available.
- Match launch remains disabled in the UI until a separately reviewed and authorized command surface exists.

### Current boundary

The dashboard now displays the latest authenticated bot snapshot, including lifecycle, game loop, actual race, active mode, phase, resources, supply, visible unit counts, and job counts. It displays structured lifecycle, action-result, and diagnostic events returned by the bot's bounded event journal. It does not control a SingularityBot process.

The bot process remains independently launchable. The browser UI and any future LLM-assisted authoring layer are observers/controllers, not prerequisites for bot startup.

### Architecture

- `TheSingularityWorkshop.GUI.Core` owns the renderer-neutral semantic model.
- `TheSingularityWorkshop.GUI.Blazor` manifests the semantic header in Blazor.
- This sample owns the application shell and server-side local transport adapter.
- SingularityBot remains the authoritative owner of SC2 state and gameplay decisions.

### Semantic match-profile controls

The match-profile editor is composed from the public `GUI.Core` semantic control contracts and rendered by `GUI.Blazor`:

- opening mode uses a stable-ID `SelectDefinition`;
- map name uses a `TextInputDefinition`;
- opponent and difficulty use stable-ID `SelectDefinition` options.

The component supplies itself as the Blazor event receiver so accepted UI changes update the component's local configuration and re-render the semantic tree. The host handler allow-lists each command and validates every incoming option/value. These controls only edit an in-memory match profile: they do not start a match, issue bot commands, or imply that a selected strategy is executable. The authenticated bot bridge remains read-only.
