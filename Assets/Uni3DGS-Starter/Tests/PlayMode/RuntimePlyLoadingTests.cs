using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Uni3DGS.Starter.Tests
{
    public sealed class RuntimePlyLoadingTests
    {
        [UnityTest]
        public IEnumerator LoadsDeterministicPly()
        {
            var task = GaussianSplat.LoadAsync(StarterFixturePaths.Ply());
            while (!task.IsCompleted) yield return null;
            if (task.IsFaulted) throw task.Exception;
            using var splat = task.Result;
            Assert.That(splat.Count, Is.EqualTo(1));
            Assert.That(splat.Metadata.SourceFormat, Is.EqualTo("PLY"));
        }
    }
}
