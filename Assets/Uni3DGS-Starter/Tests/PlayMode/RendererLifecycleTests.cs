using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Uni3DGS.Starter.Tests
{
    public sealed class RendererLifecycleTests
    {
        [UnityTest]
        public IEnumerator AssignDisableEnableRelease()
        {
            var task = GaussianSplat.LoadAsync(StarterFixturePaths.Ply());
            while (!task.IsCompleted) yield return null;
            if (task.IsFaulted) throw task.Exception;
            using var splat = task.Result;

            var go = new GameObject("lifecycle");
            var renderer = go.AddComponent<GaussianSplatRenderer>();
            renderer.Splat = splat;
            Assert.That(renderer.Count, Is.EqualTo(1));

            renderer.enabled = false;
            yield return null;
            renderer.enabled = true;
            yield return null;
            Assert.That(renderer.Count, Is.EqualTo(1));

            Object.Destroy(go);
            yield return null;
        }
    }
}
