# Testing

## EditMode

Validates:
- package resolution
- deterministic PLY/SPZ editor import
- public renderer component defaults
- URP project setup and renderer-feature registration

## PlayMode

Validates:
- runtime PLY loading
- runtime SPZ loading
- coordinate-system modes
- renderer lifecycle
- multiple renderers
- Unity transform behavior
- scene-controller smoke construction
- known failure paths

## Manual visual

`08-Depth-Occlusion` is the primary visual depth regression scene.

## Package tests

`Packages/manifest.json` lists `com.luminarylabs.uni3dgs` in `testables`, exposing the Git dependency's package tests in Test Runner.
