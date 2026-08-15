using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Uni3DGS.Starter
{
    public static class FeatureProbe
    {
        public static bool IsUrpActive
            => GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset;

        public static string Describe()
            => IsUrpActive ? "URP active" : "URP NOT active";
    }
}
