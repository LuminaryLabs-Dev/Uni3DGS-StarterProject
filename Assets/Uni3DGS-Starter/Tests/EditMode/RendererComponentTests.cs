using NUnit.Framework;
using UnityEngine;

namespace Uni3DGS.Starter.Tests
{
    public sealed class RendererComponentTests
    {
        [Test]
        public void RendererExposesExpectedDefaultSettings()
        {
            var go = new GameObject("renderer-test");
            try
            {
                var renderer = go.AddComponent<GaussianSplatRenderer>();
                Assert.That(renderer.Settings.SplatScale, Is.EqualTo(1f));
                Assert.That(renderer.Settings.Opacity, Is.EqualTo(1f));
                Assert.That(renderer.Settings.MaxSphericalHarmonicsDegree, Is.EqualTo(3));
                Assert.That(renderer.Count, Is.EqualTo(0));
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }
    }
}
