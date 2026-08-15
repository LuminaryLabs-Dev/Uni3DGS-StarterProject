using UnityEngine;
using UnityEngine.Profiling;

namespace Uni3DGS.Starter
{
    public sealed class PerformancePanel
    {
        float smoothedDelta = 1f / 60f;

        public void Tick()
        {
            smoothedDelta = Mathf.Lerp(smoothedDelta, Mathf.Max(Time.unscaledDeltaTime, 0.00001f), 0.05f);
        }

        public void Draw(GaussianSplatRenderer renderer, float lastLoadMilliseconds)
        {
            float fps = 1f / smoothedDelta;
            GUILayout.Label($"FPS: {fps:0.0}");
            GUILayout.Label($"Frame: {smoothedDelta * 1000f:0.00} ms");
            GUILayout.Label($"Splat count: {(renderer != null ? renderer.Count : 0):N0}");
            GUILayout.Label($"Last load: {lastLoadMilliseconds:0.00} ms");
            GUILayout.Label($"Unity allocated: {Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f):0.0} MiB");
        }
    }
}
