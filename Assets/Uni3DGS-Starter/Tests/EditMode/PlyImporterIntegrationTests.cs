using NUnit.Framework;
using UnityEditor;

namespace Uni3DGS.Starter.Tests
{
    public sealed class PlyImporterIntegrationTests
    {
        [TestCase("SingleSplat.ply", 1, 0)]
        [TestCase("BasicSH0.ply", 1, 0)]
        [TestCase("BasicSH1.ply", 1, 1)]
        [TestCase("BasicSH2.ply", 1, 2)]
        [TestCase("BasicSH3.ply", 1, 3)]
        public void DeterministicFixtureImports(string file, int count, int degree)
        {
            string path = "Assets/Uni3DGS-Starter/TestData/PLY/" + file;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            var asset = AssetDatabase.LoadAssetAtPath<GaussianSplatAsset>(path);
            Assert.That(asset, Is.Not.Null);
            Assert.That(asset.Count, Is.EqualTo(count));
            Assert.That(asset.Metadata.SphericalHarmonicsDegree, Is.EqualTo(degree));
            Assert.That(asset.Metadata.SourceFormat, Is.EqualTo("PLY"));
        }
    }
}
