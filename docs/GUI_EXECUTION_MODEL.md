# GUI Execution Model

**Status: proposed architecture, not a completed runtime contract.** The repository currently provides a semantic GUI tree and manifestation adapters; it does not yet provide the full execution host described here.

The target architecture treats the GUI as more than a renderer. It proposes that the outermost GUI boundary become the **execution host** for an experience: accepting something runnable, resolving required micro-bundles and capabilities, establishing execution context, routing external input/data, and selecting an appropriate manifestation. These are responsibilities the future contract and implementation must define and test.

This is a deliberate inversion of the conventional relationship:

```text
Traditional:

Application
  -> GUI
    -> Platform
      -> Device

Workshop:

GUI execution boundary
  -> Experience / app
    -> Micro-bundles
      -> preferred interoperables / tools / editors / data adapters

             ^
             |
      physical/digital world
      is adapted at the boundary
```

## The central operation

The conceptual operation is:

```csharp
Gui.Execute(app);
```

and therefore:

```csharp
foreach (var appToRun in appsToRun)
{
    Gui.Execute(appToRun);
}
```

The important property is not the syntax. The important property is **who owns execution**.

The caller does not need to know whether an experience manifests as:

- a conventional desktop interface,
- a web experience,
- a diagnostic surface,
- a data-only process,
- a 2D scene,
- a 3D scene,
- an immersive experience,
- static/noise,
- or no visible representation at all.

The proposed execution boundary would determine how the requested experience can run and what observation/interaction surface is appropriate.

## GUI as outermost wrapper

The target model therefore separates two responsibilities that must remain distinct:

1. **Execution**
   - accept a runnable experience
   - establish runtime context
   - resolve/activate required capabilities
   - coordinate lifecycle
   - route input and external data
   - observe execution state
   - terminate, suspend, resume, or replace the experience

2. **Manifestation**
   - represent the observable state
   - expose interaction
   - render or otherwise publish data
   - adapt to the selected presentation target

Execution does not imply visual output.

A valid execution may produce:

- pixels,
- audio,
- haptics,
- structured data,
- telemetry,
- a machine-readable state stream,
- another application-facing API,
- or no outward representation.

This is why GUI should not be reduced to a widget tree.

## Micro-bundles as execution dependencies

Micro-bundles are the mechanism by which an experience declares what it needs without embedding platform assumptions into the experience itself.

A bundle may associate itself with:

- interoperable standards,
- editors,
- import/export formats,
- device adapters,
- renderers,
- data providers,
- storage systems,
- domain tools,
- or other executable capabilities.

The experience asks for capability.

The proposed execution environment would determine how that capability is supplied.

This keeps platform-specific knowledge at the edge rather than forcing every application to know the physical or digital implementation behind the capability.

## The physical/digital boundary

Physical devices and external digital systems are not the foundation of the experience.

They are **boundary implementations**.

Keyboard, mouse, controller, display, headset, sensor, filesystem, network service, graphics API, or other external mechanism can be translated into the semantic execution/input/data model at the outer boundary.

The same experience can therefore execute without any particular physical manifestation.

This is the stronger form of platform neutrality:

> The experience does not need to know what reality is providing the inputs or where its outputs will manifest.

## Inversion of control

This architecture is an inversion of control in the conventional sense: the application does not own the GUI lifecycle and then call into a GUI toolkit.

Instead, under the proposed model, the GUI execution boundary would own the lifecycle and run the requested experience.

The relationship becomes:

```text
Caller
  |
  | Execute(app)
  v
GUI Execution Host
  |
  +--> resolve manifest
  +--> resolve micro-bundles
  +--> establish execution context
  +--> connect external boundaries
  +--> run experience
  +--> observe state
  +--> manifest representation
  +--> route interaction
  +--> lifecycle management
```

This should not be confused with saying that the GUI becomes the domain runtime.

The domain remains authoritative over its own semantics and state.

The intended GUI execution host is the **outer orchestration boundary**.

## Proposed vocabulary

- **Execution host** — owns the lifecycle of a requested runnable experience.
- **Runnable experience** — something that can be activated by the execution host.
- **Execution context** — the capabilities, configuration, inputs, outputs, and lifecycle environment supplied to a running experience.
- **Manifestation** — the observable representation selected for an execution.
- **Boundary adapter** — translates physical or external digital mechanisms into semantic capabilities.
- **Micro-bundle** — a composable capability package that can provide or select implementations.
- **Execution boundary** — the outer boundary at which an experience becomes runnable and external reality is translated into the digital model.

## What `Gui.Execute` must eventually mean

The API should not be designed as a glorified renderer call.

Conceptually:

```csharp
Gui.Execute(runnable);
```

means:

> "Take this runnable thing, establish everything it needs to execute, run it, and provide whatever representation and interaction surface its execution requires."

The exact return type and lifetime model remain intentionally unspecified until the execution contracts are defined.

That is important. We should not prematurely bake asynchronous execution, threading, UI ownership, process ownership, or a particular platform into the semantic core.

## Non-goals

This model does **not** mean:

- the GUI owns domain state;
- every application must become a GUI;
- every execution must render pixels;
- OpenXR, WPF, Blazor, Unity, or another platform becomes the semantic foundation;
- micro-bundles become GUI code;
- the execution host becomes a universal replacement for operating systems.

The GUI is the outer boundary and orchestrator. The things inside it remain independently meaningful.

## Architectural consequence

The existing semantic GUI model is therefore incomplete if it only describes:

```text
GuiNode -> Adapter -> Platform
```

The larger model is:

```text
                    GUI EXECUTION HOST
                           |
             +-------------+-------------+
             |             |             |
          Execute       Observe       Interact
             |             |             |
             +-------------+-------------+
                           |
                    Runnable Experience
                           |
                    Micro-bundle graph
                           |
                 Domain / behavior / data
                           |
              Boundary adapters / reality
                           |
                devices / external systems


              Manifestation is orthogonal
                           |
              +------------+------------+
              |            |            |
            pixels       audio       pure data
              |            |            |
          web/desktop   immersive   headless/etc.
```

The next design task is therefore not "add a GUI runner."

It is to define the **semantic execution contract** that makes `Gui.Execute(...)` meaningful without coupling Core to a particular application model, scheduler, renderer, operating system, or device API.
