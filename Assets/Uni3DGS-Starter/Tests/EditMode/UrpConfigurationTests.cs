using System.Linq;
using NUnit.Framework;
using Uni3DGS.Starter.Editor;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Uni3DGS.Starter.Tests
{
    public sealed class UrpConfigurationTests
    {
        [Test]
        public void RepairProducesUrpWithUni3DGSFeature()
        {
            StarterProjectSetup.EnsureProjectConfiguration();

            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(StarterProjectSetup.PipelinePath);
            var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(StarterProjectSetup.RendererPath);

            Assert.That(pipeline, Is.Not.Null);
            Assert.That(renderer, Is.Not.Null);
            Assert.That(GraphicsSettings.defaultRenderPipeline, Is.SameAs(pipeline));
            Assert.That(renderer.rendererFeatures.OfType<Uni3DGSRendererFeature>().Any(), Is.True);
        }
    }
}
