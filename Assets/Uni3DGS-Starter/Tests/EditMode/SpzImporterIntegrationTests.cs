using NUnit.Framework;
using UnityEditor;

namespace Uni3DGS.Starter.Tests
{
    public sealed class SpzImporterIntegrationTests
    {
        [TestCase("Basic-v1.spz", "SPZ v1")]
        [TestCase("Basic-v2.spz", "SPZ v2")]
        [TestCase("Basic-v3.spz", "SPZ v3")]
        public void LegacySpzFixtureImports(string file, string format)
        {
            string path = "Assets/Uni3DGS-Starter/TestData/SPZ/" + file;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            var asset = AssetDatabase.LoadAssetAtPath<GaussianSplatAsset>(path);
            Assert.That(asset, Is.Not.Null);
            Assert.That(asset.Count, Is.EqualTo(1));
            Assert.That(asset.Metadata.SourceFormat, Is.EqualTo(format));
        }

        [Test]
        public void V4IsExpectedFailureInPackage001()
        {
            string path = InvalidFixtureFactory.SpzV4();
            Assert.Throws<System.NotSupportedException>(
                () => GaussianSplat.LoadAsync(path).GetAwaiter().GetResult());
        }
    }
}
