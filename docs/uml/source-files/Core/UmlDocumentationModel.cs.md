# UmlDocumentationModel.cs.cs

- **Project:** Core
- **Source:** [`src/Core/UmlDocumentationModel.cs`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/1471b9aa90db1ad0de9ec2653f8422dbe5fa16d0/src/Core/UmlDocumentationModel.cs)
- **Source revision:** [`1471b9a`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/1471b9aa90db1ad0de9ec2653f8422dbe5fa16d0)

## Responsibility

Defines versioned, renderer-neutral documentation facts produced by build-time source analysis.

## Source-grounded facts

- The model separates provenance, type/member identities, source spans, relationship kinds/resolution/evidence, and human-authored use-case records.
- The source comment explicitly distinguishes this DTO model from a Roslyn syntax tree, compiled reflection snapshot, or rendered diagram.

## Declared types and generated UML evidence

- `UmlDocumentationModel`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlDocumentationModel-2fd1a67acd.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlDocumentationModel-2fd1a67acd.svg`
- `UmlDocumentationModelJson`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlDocumentationModelJson-9f2f4ccf69.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlDocumentationModelJson-9f2f4ccf69.svg`
- `UmlDocumentedMember`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlDocumentedMember-b6a856e937.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlDocumentedMember-b6a856e937.svg`
- `UmlDocumentedRelationship`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlDocumentedRelationship-ca6bae7402.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlDocumentedRelationship-ca6bae7402.svg`
- `UmlDocumentedType`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlDocumentedType-ec952389d5.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlDocumentedType-ec952389d5.svg`
- `UmlModelProvenance`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlModelProvenance-af9be2178c.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlModelProvenance-af9be2178c.svg`
- `UmlOwnershipEvidence`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlOwnershipEvidence-3895d42b24.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlOwnershipEvidence-3895d42b24.svg`
- `UmlRelationshipKind`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlRelationshipKind-d6973eb801.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlRelationshipKind-d6973eb801.svg`
- `UmlRelationshipProvenance`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlRelationshipProvenance-97cbac5851.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlRelationshipProvenance-97cbac5851.svg`
- `UmlRelationshipResolution`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlRelationshipResolution-f9547b0e5b.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlRelationshipResolution-f9547b0e5b.svg`
- `UmlSourceDeclaration`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlSourceDeclaration-52388fe722.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlSourceDeclaration-52388fe722.svg`
- `UmlSourceSpan`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlSourceSpan-5218d0093c.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlSourceSpan-5218d0093c.svg`
- `UmlTypeIdentity`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlTypeIdentity-8a1522d337.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlTypeIdentity-8a1522d337.svg`
- `UmlUseCase`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlUseCase-a2dd51dd9d.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlUseCase-a2dd51dd9d.svg`
- `UmlUseCaseActor`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlUseCaseActor-70bdf54e01.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlUseCaseActor-70bdf54e01.svg`
- `UmlUseCaseAlternateFlow`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlUseCaseAlternateFlow-94bd04c523.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlUseCaseAlternateFlow-94bd04c523.svg`
- `UmlUseCaseDocument`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlUseCaseDocument-bc41bd79ce.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlUseCaseDocument-bc41bd79ce.svg`
- `UmlUseCaseEvidence`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlUseCaseEvidence-4cda25682c.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlUseCaseEvidence-4cda25682c.svg`
- `UmlUseCaseOwnership`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlUseCaseOwnership-3504d8304b.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlUseCaseOwnership-3504d8304b.svg`
- `UmlUseCaseReview`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlUseCaseReview-81e6b143fb.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlUseCaseReview-81e6b143fb.svg`
- `UmlUseCaseStep`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlUseCaseStep-ea1a9c83f9.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-UmlUseCaseStep-ea1a9c83f9.svg`

The type page and SVG filenames above are from the downloadable `solution-derived-uml-inventory` artifact in [run #39509559623](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39509559623). The SVG is generated from the compiler-backed documentation model and shows the focused type neighborhood; it is not a substitute for reading the source. The generated artifact is not yet committed beside this authored guide.

## Verification boundaries

- The solution workflow built this project's exact Release assembly and reconciled its assembly identity/type metadata against the Roslyn model.
- The generated model verifies type inventory, not member-level compiled metadata parity.
- SVG accessibility title/description and local structure are validated by CI; painted text bounds and visual layout still require a separate visual audit.
