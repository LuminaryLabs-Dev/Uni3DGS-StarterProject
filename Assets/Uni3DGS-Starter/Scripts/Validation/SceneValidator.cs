using UnityEngine;

namespace Uni3DGS.Starter
{
    public static class SceneValidator
    {
        public static bool HasMainCamera() => Camera.main != null;

        public static int RendererCount()
            => Object.FindObjectsByType<GaussianSplatRenderer>(FindObjectsSortMode.None).Length;
    }
}
