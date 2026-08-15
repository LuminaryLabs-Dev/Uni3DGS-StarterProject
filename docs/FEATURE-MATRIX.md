# Feature Matrix

The starter project is the external-consumer integration contract for Uni3DGS.

| Feature | Scene | Automated | Current expectation |
|---|---:|---:|---|
| Package install | boot | yes | PASS |
| PLY editor import | 01 | yes | PASS |
| SPZ v1 editor import | 02 | yes | PASS |
| SPZ v2 editor import | 02 | yes | PASS |
| SPZ v3 editor import | 02 | yes | PASS |
| SPZ v4 | 11 | yes | EXPECTED FAILURE |
| Runtime PLY | 03 | yes | PASS |
| Runtime SPZ | 04 | yes | PASS |
| SH0 fixture | 05 | yes | PASS |
| SH1 fixture | 05 | yes | PASS |
| SH2 fixture | 05 | yes | PASS |
| SH3 fixture | 05 | yes | PASS |
| Public default opacity | 05 | yes | PASS |
| Public default splat scale | 05 | yes | PASS |
| Runtime settings mutation | 05 | contract probe | BLOCKED BY PUBLIC API |
| Auto coordinates | 06 | yes | PASS |
| RUF | 06 | yes | PASS |
| RDF | 06 | yes | PASS |
| RUB | 06 | yes | PASS |
| Transforms | 07 | yes | PASS |
| Scene depth/occlusion | 08 | visual | MANUAL VISUAL |
| Multiple clouds | 09 | yes | PASS |
| Lifecycle | 10 | yes | PASS |
| Bad inputs | 11 | yes | PASS |
| Performance | 12 | benchmark | TRACKED |

## Rule

A new public Uni3DGS feature is not considered integration-complete until this matrix has a corresponding starter-project example or regression test.

## Public API gap

In Uni3DGS `0.0.1`, `GaussianSplatRenderer.Settings` is read-only. This project does not use reflection or private serialized-field access to fake runtime settings support. Once the package exposes a public settings setter/API, scene `05` and its tests should be upgraded from contract probing to true interactive mutation.
