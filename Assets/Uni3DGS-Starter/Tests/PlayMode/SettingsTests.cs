using NUnit.Framework;
using UnityEngine;

namespace Uni3DGS.Starter.Tests
{
    public sealed class SettingsTests
    {
        [Test]
        public void PublicSettingsContractIsReadable()
        {
            var go = new GameObject("settings");
            try
            {
                var renderer = go.AddComponent<GaussianSplatRenderer>();
                GaussianSettings settings = renderer.Settings;
                Assert.That(settings.SplatScale, Is.GreaterThan(0f));
                Assert.That(settings.Opacity, Is.GreaterThanOrEqualTo(0f));
                Assert.That(settings.MaxSphericalHarmonicsDegree, Is.InRange(0, 3));
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }
    }
}
