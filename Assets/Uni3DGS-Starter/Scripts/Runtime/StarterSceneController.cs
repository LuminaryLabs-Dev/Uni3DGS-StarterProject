using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Uni3DGS.Starter
{
    public sealed class StarterSceneController : MonoBehaviour
    {
        public enum Scenario
        {
            Overview,
            ImportedPly,
            ImportedSpz,
            RuntimePly,
            RuntimeSpz,
            RenderSettings,
            CoordinateSystems,
            Transforms,
            DepthOcclusion,
            MultipleSplats,
            Lifecycle,
            InvalidInputs,
            Performance
        }

        [SerializeField] Scenario scenario;
        [SerializeField] bool autoRun = true;

        readonly List<GaussianSplat> ownedSplats = new();
        readonly List<GameObject> spawned = new();
        readonly SplatControlPanel controlPanel = new();
        readonly FileLoadPanel filePanel = new();
        readonly PerformancePanel performancePanel = new();
        readonly BenchmarkRecorder benchmark = new();

        RuntimeSplatLoader primaryLoader;
        GaussianSplatRenderer primaryRenderer;
        OrbitCamera orbit;
        string status = "Ready";
        string details = "";
        float lastLoadMilliseconds;
        bool busy;

        public Scenario ActiveScenario => scenario;
        public string Status => status;

        async void Start()
        {
            BuildEnvironment();
            if (autoRun) await RunScenarioAsync();
        }

        void Update()
        {
            performancePanel.Tick();
            benchmark.Tick();
        }

        async Task RunScenarioAsync()
        {
            if (busy) return;
            busy = true;
            status = "Running";
            details = string.Empty;

            try
            {
                switch (scenario)
                {
                    case Scenario.Overview:
                    case Scenario.ImportedPly:
                    case Scenario.RuntimePly:
                        await BuildSingleAsync(StarterFixturePaths.Ply(), GaussianCoordinateSystem.Auto);
                        break;

                    case Scenario.ImportedSpz:
                    case Scenario.RuntimeSpz:
                        await BuildSingleAsync(StarterFixturePaths.Spz("Basic-v3.spz"), GaussianCoordinateSystem.Auto);
                        break;

                    case Scenario.RenderSettings:
                        await BuildSingleAsync(StarterFixturePaths.Ply("BasicSH3.ply"), GaussianCoordinateSystem.Auto);
                        details = "Renderer settings are public/read-only in Uni3DGS 0.0.1. This scene validates the public values without private-field access.";
                        break;

                    case Scenario.CoordinateSystems:
                        await BuildCoordinateSystemsAsync();
                        break;

                    case Scenario.Transforms:
                        await BuildTransformsAsync();
                        break;

                    case Scenario.DepthOcclusion:
                        await BuildDepthAsync();
                        break;

                    case Scenario.MultipleSplats:
                        await BuildMultipleAsync();
                        break;

                    case Scenario.Lifecycle:
                        await BuildSingleAsync(StarterFixturePaths.Ply(), GaussianCoordinateSystem.Auto);
                        break;

                    case Scenario.InvalidInputs:
                        await RunInvalidInputsAsync();
                        break;

                    case Scenario.Performance:
                        await BuildSingleAsync(StarterFixturePaths.Ply("SmallScene.ply"), GaussianCoordinateSystem.Auto);
                        filePanel.Path = StarterFixturePaths.Ply("SmallScene.ply");
                        break;
                }

                if (scenario != Scenario.InvalidInputs)
                    status = "PASS";
            }
            catch (Exception ex)
            {
                status = "FAIL";
                details = ex.GetType().Name + ": " + ex.Message;
                Debug.LogException(ex);
            }
            finally
            {
                busy = false;
            }
        }

        void BuildEnvironment()
        {
            if (Camera.main == null)
            {
                var cameraGo = new GameObject("Main Camera");
                cameraGo.tag = "MainCamera";
                var camera = cameraGo.AddComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0.035f, 0.045f, 0.06f);
                orbit = cameraGo.AddComponent<OrbitCamera>();
                orbit.Target = transform;
            }

            if (FindFirstObjectByType<Light>() == null)
            {
                var lightGo = new GameObject("Directional Light");
                var light = lightGo.AddComponent<Light>();
                light.type = LightType.Directional;
                light.intensity = 1.2f;
                lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }

            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Environment Floor";
            floor.transform.position = new Vector3(0f, -1.25f, 0f);
            floor.transform.localScale = Vector3.one * 2.5f;
            spawned.Add(floor);
        }

        async Task<GaussianSplat> LoadTimedAsync(string path, GaussianCoordinateSystem coordinates)
        {
            var watch = Stopwatch.StartNew();
            var splat = await GaussianSplat.LoadAsync(path, coordinates);
            watch.Stop();
            lastLoadMilliseconds = (float)watch.Elapsed.TotalMilliseconds;
            ownedSplats.Add(splat);
            return splat;
        }

        async Task BuildSingleAsync(string path, GaussianCoordinateSystem coordinates)
        {
            var splat = await LoadTimedAsync(path, coordinates);
            var go = new GameObject("Gaussian Splat");
            primaryRenderer = go.AddComponent<GaussianSplatRenderer>();
            primaryRenderer.Splat = splat;
            spawned.Add(go);

            primaryLoader = go.AddComponent<RuntimeSplatLoader>();
            primaryLoader.Renderer = primaryRenderer;
            primaryLoader.Path = path;
            primaryLoader.SourceCoordinates = coordinates;

            orbit.Target = go.transform;
            details = $"{splat.Metadata.SourceFormat} · {splat.Count:N0} splats · SH{splat.Metadata.SphericalHarmonicsDegree}";
        }

        async Task BuildCoordinateSystemsAsync()
        {
            var modes = new[]
            {
                GaussianCoordinateSystem.Auto,
                GaussianCoordinateSystem.RUF,
                GaussianCoordinateSystem.RDF,
                GaussianCoordinateSystem.RUB
            };

            for (int i = 0; i < modes.Length; i++)
            {
                var splat = await LoadTimedAsync(StarterFixturePaths.Ply(), modes[i]);
                var go = new GameObject("Splat " + modes[i]);
                go.transform.position = new Vector3((i - 1.5f) * 2f, 0f, 0f);
                var renderer = go.AddComponent<GaussianSplatRenderer>();
                renderer.Splat = splat;
                spawned.Add(go);
                if (primaryRenderer == null) primaryRenderer = renderer;
            }
            details = "Auto / RUF / RDF / RUB loaded from the same deterministic source.";
        }

        async Task BuildTransformsAsync()
        {
            var splat = await LoadTimedAsync(StarterFixturePaths.Ply("SmallScene.ply"), GaussianCoordinateSystem.Auto);
            var transforms = new (Vector3 pos, Vector3 euler, Vector3 scale, string name)[]
            {
                (new Vector3(-4f,0f,0f), Vector3.zero, Vector3.one, "Original"),
                (new Vector3(-2f,1f,0f), Vector3.zero, Vector3.one, "Translated"),
                (Vector3.zero, new Vector3(0f,45f,25f), Vector3.one, "Rotated"),
                (new Vector3(2f,0f,0f), Vector3.zero, Vector3.one * 1.5f, "Uniform Scale"),
                (new Vector3(4f,0f,0f), Vector3.zero, new Vector3(1.75f,0.65f,1f), "Non-Uniform Scale")
            };

            foreach (var spec in transforms)
            {
                var go = new GameObject(spec.name);
                go.transform.SetPositionAndRotation(spec.pos, Quaternion.Euler(spec.euler));
                go.transform.localScale = spec.scale;
                var renderer = go.AddComponent<GaussianSplatRenderer>();
                renderer.Splat = splat;
                spawned.Add(go);
                if (primaryRenderer == null) primaryRenderer = renderer;
            }
            details = "One runtime splat is consumed by transformed renderers.";
        }

        async Task BuildDepthAsync()
        {
            await BuildSingleAsync(StarterFixturePaths.Ply("SmallScene.ply"), GaussianCoordinateSystem.Auto);
            primaryRenderer.transform.position = new Vector3(0f, 0f, 2f);

            var blocker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blocker.name = "Opaque Depth Blocker";
            blocker.transform.position = new Vector3(0f, 0f, 1f);
            blocker.transform.localScale = new Vector3(1.5f, 1.5f, 0.5f);
            spawned.Add(blocker);

            var behind = GameObject.CreatePrimitive(PrimitiveType.Cube);
            behind.name = "Behind Splat Reference";
            behind.transform.position = new Vector3(2f, 0f, 3f);
            spawned.Add(behind);
            details = "Move the camera: opaque Unity geometry should occlude splats according to scene depth.";
        }

        async Task BuildMultipleAsync()
        {
            for (int i = 0; i < 3; i++)
            {
                var splat = await LoadTimedAsync(i == 1 ? StarterFixturePaths.Spz() : StarterFixturePaths.Ply(), GaussianCoordinateSystem.Auto);
                var go = new GameObject($"Gaussian Splat {i + 1}");
                go.transform.position = new Vector3((i - 1) * 2.2f, 0f, i * 0.35f);
                var renderer = go.AddComponent<GaussianSplatRenderer>();
                renderer.Splat = splat;
                spawned.Add(go);
                if (primaryRenderer == null) primaryRenderer = renderer;
            }
            details = "Three independent GaussianSplatRenderer components are active.";
        }

        async Task RunInvalidInputsAsync()
        {
            var checks = new List<string>();
            await ExpectFailure(checks, "missing file", Path.Combine(Application.streamingAssetsPath, "definitely-missing.ply"), typeof(FileNotFoundException));
            await ExpectFailure(checks, "empty file", InvalidFixtureFactory.EmptyPly(), typeof(Exception));
            await ExpectFailure(checks, "malformed PLY", InvalidFixtureFactory.MalformedPly(), typeof(Exception));
            await ExpectFailure(checks, "unsupported extension", InvalidFixtureFactory.UnsupportedExtension(), typeof(NotSupportedException));
            await ExpectFailure(checks, "SPZ v4", InvalidFixtureFactory.SpzV4(), typeof(NotSupportedException));
            status = checks.TrueForAll(x => x.StartsWith("PASS")) ? "PASS" : "FAIL";
            details = string.Join("\n", checks);
        }

        static async Task ExpectFailure(List<string> results, string name, string path, Type expected)
        {
            try
            {
                using var splat = await GaussianSplat.LoadAsync(path);
                results.Add($"FAIL {name}: unexpectedly loaded {splat.Count}");
            }
            catch (Exception ex)
            {
                bool ok = expected == typeof(Exception) || expected.IsAssignableFrom(ex.GetType());
                results.Add($"{(ok ? "PASS" : "FAIL")} {name}: {ex.GetType().Name}");
            }
        }

        async Task LoadExternalAsync(string path, GaussianCoordinateSystem coordinates)
        {
            if (primaryRenderer == null)
            {
                var go = new GameObject("External Gaussian Splat");
                primaryRenderer = go.AddComponent<GaussianSplatRenderer>();
                spawned.Add(go);
            }

            var splat = await LoadTimedAsync(path, coordinates);
            primaryRenderer.Splat = splat;
            details = $"{splat.Metadata.SourceFormat} · {splat.Count:N0} splats";
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(16, 16, 430, Screen.height - 32), GUI.skin.box);
            GUILayout.Label($"Uni3DGS Starter · {scenario}");
            GUILayout.Label($"Status: {status}");
            GUILayout.Label(FeatureProbe.Describe());
            if (!string.IsNullOrEmpty(details)) GUILayout.Label(details);

            if (GUILayout.Button("Run / Re-run scenario") && !busy)
                _ = RunScenarioAsync();

            if (orbit != null && GUILayout.Button("Reset Camera"))
                orbit.ResetView();

            if (primaryRenderer != null)
                controlPanel.Draw(primaryRenderer);

            if (scenario == Scenario.Lifecycle && primaryLoader != null)
            {
                GUILayout.Label("Lifecycle");
                if (GUILayout.Button("Reload")) _ = primaryLoader.LoadAsync(StarterFixturePaths.Ply(), GaussianCoordinateSystem.Auto);
                if (GUILayout.Button("Unload")) primaryLoader.Unload();
                if (GUILayout.Button("Disable Renderer")) primaryRenderer.enabled = false;
                if (GUILayout.Button("Enable Renderer")) primaryRenderer.enabled = true;
            }

            if (scenario == Scenario.Performance)
            {
                filePanel.Draw(LoadExternalAsync);
                performancePanel.Draw(primaryRenderer, lastLoadMilliseconds);
                GUILayout.Label($"Average frame: {benchmark.AverageFrameMilliseconds:0.00} ms");
            }

            GUILayout.EndArea();
        }

        void OnDestroy()
        {
            if (primaryLoader != null) primaryLoader.Unload();
            foreach (GaussianSplat splat in ownedSplats) splat?.Dispose();
            ownedSplats.Clear();
        }
    }
}
