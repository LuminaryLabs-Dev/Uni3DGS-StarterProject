using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Uni3DGS.Starter.Editor
{
    [InitializeOnLoad]
    public static class StarterProjectSetup
    {
        public const string SettingsDirectory = "Assets/Settings";
        public const string RendererPath = SettingsDirectory + "/Uni3DGS-Renderer.asset";
        public const string PipelinePath = SettingsDirectory + "/Uni3DGS-URP.asset";

        static StarterProjectSetup()
        {
            EditorApplication.delayCall += EnsureProjectConfiguration;
        }

        [MenuItem("Tools/Uni3DGS Starter/Repair Project Setup")]
        public static void EnsureProjectConfiguration()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                return;

            EnsureFolder("Assets", "Settings");

            var rendererData = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(RendererPath);
            if (rendererData == null)
            {
                rendererData = ScriptableObject.CreateInstance<UniversalRendererData>();
                rendererData.name = "Uni3DGS-Renderer";
                AssetDatabase.CreateAsset(rendererData, RendererPath);
            }

            if (!rendererData.rendererFeatures.OfType<Uni3DGSRendererFeature>().Any())
            {
                var feature = ScriptableObject.CreateInstance<Uni3DGSRendererFeature>();
                feature.name = "Uni3DGS Renderer Feature";
                rendererData.rendererFeatures.Add(feature);
                AssetDatabase.AddObjectToAsset(feature, rendererData);
                feature.Create();
                EditorUtility.SetDirty(feature);
                EditorUtility.SetDirty(rendererData);
                rendererData.SetDirty();
            }

            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath);
            if (pipeline == null)
            {
                pipeline = UniversalRenderPipelineAsset.Create(rendererData);
                pipeline.name = "Uni3DGS-URP";
                pipeline.supportsCameraDepthTexture = true;
                AssetDatabase.CreateAsset(pipeline, PipelinePath);
            }

            GraphicsSettings.defaultRenderPipeline = pipeline;
            QualitySettings.renderPipeline = pipeline;

            EnsureBuildScenes();
            AssetDatabase.SaveAssets();
        }

        static void EnsureBuildScenes()
        {
            string overview = "Assets/Uni3DGS-Starter/Scenes/00-Overview.unity";
            var desired = new[] { new EditorBuildSettingsScene(overview, true) };
            if (EditorBuildSettings.scenes.Length != 1 || EditorBuildSettings.scenes[0].path != overview)
                EditorBuildSettings.scenes = desired;
        }

        static void EnsureFolder(string parent, string child)
        {
            string path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder(parent, child);
        }
    }
}
