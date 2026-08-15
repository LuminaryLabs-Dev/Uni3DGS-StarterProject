# Updating Uni3DGS

1. Change only the `com.luminarylabs.uni3dgs` Git revision in `Packages/manifest.json`.
2. Open the project in Unity `6000.0.58f2`.
3. Run **Tools → Uni3DGS Starter → Repair Project Setup**.
4. Run all Uni3DGS package tests.
5. Run all starter EditMode tests.
6. Run all starter PlayMode tests.
7. Open every visual-regression scene affected by the package change.
8. Update `docs/FEATURE-MATRIX.md`.
9. Commit the new pin only after the contract is green.

Do not copy package source into this project to work around a failure. Fix Uni3DGS itself.
