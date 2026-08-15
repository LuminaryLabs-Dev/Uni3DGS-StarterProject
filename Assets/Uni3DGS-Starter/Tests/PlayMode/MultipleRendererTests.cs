using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Uni3DGS.Starter.Tests
{
    public sealed class MultipleRendererTests
    {
        [UnityTest]
        public IEnumerator MultipleConsumersCanUseSameRuntimeSplat()
        {
            var task = GaussianSplat.LoadAsync(StarterFixturePaths.Ply());
            while (!task.IsCompleted) yield return null;
            if (task.IsFaulted) throw task.Exception;
            using var splat = task.Result;

            var a = new GameObject("A");
            var b = new GameObject("B");
            var ra = a.AddComponent<GaussianSplatRenderer>();
            var rb = b.AddComponent<GaussianSplatRenderer>();
            ra.Splat = splat;
            rb.Splat = splat;

            Assert.That(ra.Count, Is.EqualTo(1));
            Assert.That(rb.Count, Is.EqualTo(1));

            Object.Destroy(a);
            Object.Destroy(b);
            yield return null;
        }
    }
}
