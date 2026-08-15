using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Uni3DGS.Starter.Tests
{
    public sealed class CoordinateSystemTests
    {
        [UnityTest]
        public IEnumerator EveryPublicCoordinateModeLoads()
        {
            foreach (GaussianCoordinateSystem mode in new[]
            {
                GaussianCoordinateSystem.Auto,
                GaussianCoordinateSystem.RUF,
                GaussianCoordinateSystem.RDF,
                GaussianCoordinateSystem.RUB
            })
            {
                var task = GaussianSplat.LoadAsync(StarterFixturePaths.Ply(), mode);
                while (!task.IsCompleted) yield return null;
                if (task.IsFaulted) throw task.Exception;
                using var splat = task.Result;
                Assert.That(splat.Count, Is.EqualTo(1));
            }
        }
    }
}
