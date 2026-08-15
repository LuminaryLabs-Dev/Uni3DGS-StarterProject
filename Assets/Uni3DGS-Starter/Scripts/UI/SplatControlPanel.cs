using UnityEngine;

namespace Uni3DGS.Starter
{
    public sealed class SplatControlPanel
    {
        public void Draw(GaussianSplatRenderer renderer)
        {
            if (renderer == null)
            {
                GUILayout.Label("Renderer: none");
                return;
            }

            GaussianSettings settings = renderer.Settings;
            GUILayout.Label($"Splats: {renderer.Count:N0}");
            GUILayout.Label($"Splat scale: {settings.SplatScale:0.###}");
            GUILayout.Label($"Opacity: {settings.Opacity:0.###}");
            GUILayout.Label($"Max SH degree: {settings.MaxSphericalHarmonicsDegree}");

            GUI.enabled = false;
            GUILayout.HorizontalSlider(settings.SplatScale, 0.1f, 3f);
            GUILayout.HorizontalSlider(settings.Opacity, 0f, 2f);
            GUI.enabled = true;
            GUILayout.Label("Runtime mutation is intentionally disabled: Uni3DGS 0.0.1 exposes Settings as read-only.");
        }
    }
}
