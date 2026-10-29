<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# CoordinateSystemUtilities UML

**Author:** Trent Best  
**Target:** `TheSingularityWorkshop.Workshop.Gui.CoordinateSystemUtilities`

## Class diagram

![Source-derived structural view of CoordinateSystemUtilities](source-files/Core/TheSingularityWorkshop-Workshop-Gui-CoordinateSystemUtilities-f821ba6eab.svg)

The target is a stateless static helper. Its private finite-range validator supports normalization and denormalization; distance intentionally follows a separate direct calculation path.

## Use case

Normalize a value for a 0–100 semantic coordinate system, then map it back to the source domain:

```csharp
var normalized = CoordinateSystemUtilities.Normalize(value, minimum, maximum);
var originalScale = CoordinateSystemUtilities.Denormalize(normalized, minimum, maximum);
```

The range endpoints must be finite and strictly increasing. Normalization does not clamp out-of-range values.

## Verified relationships

- Uses .NET numeric operations only.
- Has no renderer, GUI node, or UI-framework dependency.
- Exposes four public static numeric operations and one private range-validation helper.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
