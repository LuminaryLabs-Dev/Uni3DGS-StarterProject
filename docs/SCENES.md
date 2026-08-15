# Scene Guide

Each scene is intentionally narrow.

- `00-Overview` — one PLY, environment, orbit camera, live status.
- `01-Imported-PLY` — importer contract plus visible runtime fallback.
- `02-Imported-SPZ` — v1-v3 importer contract plus visible runtime fallback.
- `03-Runtime-PLY` — `GaussianSplat.LoadAsync` from StreamingAssets.
- `04-Runtime-SPZ` — runtime SPZ v3.
- `05-Render-Settings` — reports public settings and SH fixture metadata.
- `06-Coordinate-Systems` — same data loaded as Auto, RUF, RDF, RUB.
- `07-Transforms` — translated, rotated, uniform and non-uniform transform cases.
- `08-Depth-Occlusion` — splat positioned behind/in front of opaque Unity cubes.
- `09-Multiple-Splats` — three runtime renderers sharing independently loaded data.
- `10-Lifecycle` — interactive load/unload/enable/disable/reload controls.
- `11-Invalid-Inputs` — missing, empty, malformed, unsupported, and SPZ v4.
- `12-Performance` — frame/load/memory telemetry; accepts an external path in the on-screen UI.
