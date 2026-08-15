using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Uni3DGS.Starter.Tests
{
    public sealed class RuntimeSpzLoadingTests
    {
        [UnityTest]
        public IEnumerator LoadsSpzV1ToV3()
        {
            foreach (string file in new[] { "Basic-v1.spz", "Basic-v2.spz", "Basic-v3.spz" })
            {
                var task = GaussianSplat.LoadAsync(StarterFixturePaths.Spz(file));
                while (!task.IsCompleted) yield return null;
                if (task.IsFaulted) throw task.Exception;
                using var splat = task.Result;
                Assert.That(splat.Count, Is.EqualTo(1));
            }
        }
    }
}
