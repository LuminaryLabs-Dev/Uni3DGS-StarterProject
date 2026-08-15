# Uni3DGS Starter Project

A Unity 6.0 LTS consumer, integration-test, visual-regression, and benchmark harness for [Uni3DGS](https://github.com/LuminaryLabs-Dev/Uni3DGS).

## Clone → open → press Play

1. Clone this repository.
2. Open the repository root in **Unity 6000.0.58f2**.
3. Wait for Package Manager to resolve the pinned Uni3DGS Git dependency.
4. Uni3DGS Starter Setup automatically repairs the URP asset and adds `Uni3DGSRendererFeature`.
5. Open `Assets/Uni3DGS-Starter/Scenes/00-Overview.unity`.
6. Press Play.

The project consumes Uni3DGS through UPM. No Uni3DGS runtime/editor/shader source is copied into this repository.

## Pinned package

`com.luminarylabs.uni3dgs` is pinned to:

`47a691bec3f00c8c0801c692f9ff76b628a5776c`

Update the pin deliberately and run the complete integration suite before accepting a new package revision.

## Scene suite

| Scene | Purpose |
|---|---|
| `00-Overview` | First-run showcase |
| `01-Imported-PLY` | PLY editor-import contract |
| `02-Imported-SPZ` | SPZ editor-import contract |
| `03-Runtime-PLY` | Runtime PLY loading |
| `04-Runtime-SPZ` | Runtime SPZ loading |
| `05-Render-Settings` | Public renderer-settings surface |
| `06-Coordinate-Systems` | Auto/RUF/RDF/RUB loading |
| `07-Transforms` | Position, rotation, scale, hierarchy |
| `08-Depth-Occlusion` | Unity geometry + splat depth interaction |
| `09-Multiple-Splats` | Multiple independent splat renderers |
| `10-Lifecycle` | Load, replace, unload, enable, disable, recreate |
| `11-Invalid-Inputs` | Expected failure behavior |
| `12-Performance` | Load/frame/memory benchmark harness |

## Tests

Open **Window → General → Test Runner** and run both EditMode and PlayMode tests.

The project also marks `com.luminarylabs.uni3dgs` as testable so the package's own tests are visible in this consumer project.

## Repair setup

If URP settings are changed or deleted:

**Tools → Uni3DGS Starter → Repair Project Setup**

The repair pass creates/repairs:

- `Assets/Settings/Uni3DGS-Renderer.asset`
- `Assets/Settings/Uni3DGS-URP.asset`
- active Graphics/Quality render pipeline assignment
- `Uni3DGSRendererFeature`
- build-scene registration

## Known 0.0.1 package constraints

- SPZ v1-v3 are supported.
- SPZ v4 is intentionally an expected failure until Uni3DGS vendors a Zstandard decoder.
- `GaussianSplatRenderer.Settings` is currently read-only at runtime. The starter validates the public defaults and documents the missing runtime mutation API instead of reaching into package internals.

See `docs/FEATURE-MATRIX.md`.
