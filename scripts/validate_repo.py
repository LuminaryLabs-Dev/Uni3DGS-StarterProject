#!/usr/bin/env python3
import gzip
import json
import struct
from pathlib import Path

root = Path(__file__).resolve().parents[1]
errors = []

scene_names = [
    "Overview", "Imported-PLY", "Imported-SPZ", "Runtime-PLY", "Runtime-SPZ",
    "Render-Settings", "Coordinate-Systems", "Transforms", "Depth-Occlusion",
    "Multiple-Splats", "Lifecycle", "Invalid-Inputs", "Performance",
]

required = [
    "README.md",
    "Packages/manifest.json",
    "ProjectSettings/ProjectVersion.txt",
    "Assets/Uni3DGS-Starter/Editor/StarterProjectSetup.cs",
    "Assets/Uni3DGS-Starter/Scripts/Runtime/StarterSceneController.cs",
    "Assets/Uni3DGS-Starter/Tests/EditMode/PlyImporterIntegrationTests.cs",
    "Assets/Uni3DGS-Starter/Tests/PlayMode/RuntimePlyLoadingTests.cs",
    "docs/FEATURE-MATRIX.md",
]
required += [f"Assets/Uni3DGS-Starter/Scenes/{i:02d}-{name}.unity" for i, name in enumerate(scene_names)]
required += [f"Assets/Uni3DGS-Starter/Scenes/{i:02d}-{name}.unity.meta" for i, name in enumerate(scene_names)]

for relative in required:
    if not (root / relative).exists():
        errors.append("missing " + relative)

# Project/package contract.
manifest_path = root / "Packages/manifest.json"
try:
    manifest = json.loads(manifest_path.read_text())
except Exception as exc:
    errors.append(f"invalid Packages/manifest.json: {exc}")
    manifest = {}

dep = manifest.get("dependencies", {}).get("com.luminarylabs.uni3dgs", "")
expected_commit = "47a691bec3f00c8c0801c692f9ff76b628a5776c"
expected_url = "https://github.com/LuminaryLabs-Dev/Uni3DGS.git?path=/Packages/Uni3DGS#" + expected_commit
if dep != expected_url:
    errors.append("Uni3DGS dependency is not pinned to the exact expected Git package URL/commit")
if "com.luminarylabs.uni3dgs" not in manifest.get("testables", []):
    errors.append("Uni3DGS is not exposed through testables")
if manifest.get("dependencies", {}).get("com.unity.render-pipelines.universal") != "17.0.4":
    errors.append("URP is not pinned to 17.0.4")

version = (root / "ProjectSettings/ProjectVersion.txt").read_text() if (root / "ProjectSettings/ProjectVersion.txt").exists() else ""
if "6000.0.58f2" not in version:
    errors.append("Unity version is not pinned to 6000.0.58f2")

# No package-source duplication in the consumer project.
if (root / "Packages/Uni3DGS").exists():
    errors.append("starter must consume Uni3DGS through UPM; embedded Packages/Uni3DGS is forbidden")
for forbidden_dir in ["Runtime", "Editor", "Shaders"]:
    copied = root / "Assets/Uni3DGS" / forbidden_dir
    if copied.exists():
        errors.append(f"copied package source is forbidden: {copied.relative_to(root)}")

# Scene contract: each scene binds the corresponding scenario integer.
for i, name in enumerate(scene_names):
    path = root / f"Assets/Uni3DGS-Starter/Scenes/{i:02d}-{name}.unity"
    if path.exists():
        text = path.read_text(errors="ignore")
        if f"scenario: {i}" not in text:
            errors.append(f"scene scenario mismatch: {path.relative_to(root)}")
        if "guid: c901a8d9e4f54d2bb8be0c4357700001" not in text:
            errors.append(f"scene is not bound to stable StarterSceneController GUID: {path.relative_to(root)}")

# JSON sanity for every asmdef.
for asmdef in root.glob("Assets/**/*.asmdef"):
    try:
        json.loads(asmdef.read_text())
    except Exception as exc:
        errors.append(f"invalid asmdef {asmdef.relative_to(root)}: {exc}")

# Deterministic PLY fixtures: binary little-endian and expected SH property counts.
def read_ply_header(path: Path):
    data = path.read_bytes()
    marker = b"end_header\n"
    end = data.find(marker)
    if end < 0:
        raise ValueError("end_header not found")
    header = data[: end + len(marker)].decode("ascii")
    return header, data[end + len(marker):]

ply_expectations = {
    "SingleSplat.ply": (1, 0),
    "BasicSH0.ply": (1, 0),
    "BasicSH1.ply": (1, 9),
    "BasicSH2.ply": (1, 24),
    "BasicSH3.ply": (1, 45),
    "SmallScene.ply": (16, 0),
}
for name, (count, rest_count) in ply_expectations.items():
    path = root / "Assets/Uni3DGS-Starter/TestData/PLY" / name
    if not path.exists():
        errors.append(f"missing PLY fixture {name}")
        continue
    try:
        header, payload = read_ply_header(path)
        if "format binary_little_endian 1.0" not in header:
            errors.append(f"{name} is not binary_little_endian PLY")
        if f"element vertex {count}" not in header:
            errors.append(f"{name} vertex count header mismatch")
        actual_rest = sum(1 for line in header.splitlines() if " f_rest_" in line)
        if actual_rest != rest_count:
            errors.append(f"{name} f_rest property count {actual_rest} != {rest_count}")
        if not payload:
            errors.append(f"{name} has no vertex payload")
    except Exception as exc:
        errors.append(f"invalid PLY fixture {name}: {exc}")

# Deterministic SPZ fixtures: legacy gzip v1-v3 + raw NGSP v4 detector.
magic = 0x5053474E
for version_num in (1, 2, 3):
    path = root / f"Assets/Uni3DGS-Starter/TestData/SPZ/Basic-v{version_num}.spz"
    try:
        payload = gzip.decompress(path.read_bytes())
        if len(payload) < 16:
            raise ValueError("legacy payload shorter than header")
        got_magic, got_version, count = struct.unpack_from("<III", payload, 0)
        if got_magic != magic or got_version != version_num or count != 1:
            errors.append(f"Basic-v{version_num}.spz header mismatch")
    except Exception as exc:
        errors.append(f"invalid SPZ v{version_num} fixture: {exc}")

v4 = root / "Assets/Uni3DGS-Starter/TestData/Invalid/v4-Unsupported.spz.bin"
try:
    data = v4.read_bytes()
    if len(data) != 32 or struct.unpack_from("<I", data, 0)[0] != magic or struct.unpack_from("<I", data, 4)[0] != 4:
        errors.append("SPZ v4 expected-failure fixture header mismatch")
except Exception as exc:
    errors.append(f"invalid SPZ v4 fixture: {exc}")

for inert in ("Empty.ply.bin", "Malformed.ply.bin", "v4-Unsupported.spz.bin"):
    if not (root / "Assets/Uni3DGS-Starter/TestData/Invalid" / inert).exists():
        errors.append(f"missing inert invalid fixture {inert}")

# The valid runtime and editor fixture copies must remain byte-identical.
for group in ("PLY", "SPZ"):
    source_dir = root / "Assets/Uni3DGS-Starter/TestData" / group
    runtime_dir = root / "Assets/StreamingAssets/Uni3DGS" / group
    if source_dir.exists():
        for source in source_dir.iterdir():
            if source.is_file():
                runtime = runtime_dir / source.name
                if not runtime.exists() or runtime.read_bytes() != source.read_bytes():
                    errors.append(f"runtime fixture copy differs: {group}/{source.name}")

# Architecture guard.
for path in root.rglob("*"):
    if not path.is_file() or "Library" in path.parts:
        continue
    if path.name == "validate_repo.py":
        continue
    text = ""
    if path.suffix.lower() in {".cs", ".md", ".json", ".py", ".yml", ".yaml", ".asset", ".txt"}:
        try:
            text = path.read_text(errors="ignore")
        except Exception:
            pass
    forbidden = "Uni3DGS" + ".App"
    if forbidden in text or forbidden in str(path):
        errors.append(f"forbidden app-layer reference in {path.relative_to(root)}")

if errors:
    print("Uni3DGS Starter validation failed:")
    for error in errors:
        print(" -", error)
    raise SystemExit(1)

print("Uni3DGS Starter repository contract OK")
