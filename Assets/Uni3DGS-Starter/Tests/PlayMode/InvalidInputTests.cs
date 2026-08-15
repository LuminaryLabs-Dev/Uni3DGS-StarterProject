using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Uni3DGS.Starter.Tests
{
    public sealed class InvalidInputTests
    {
        [UnityTest]
        public IEnumerator SpzV4IsKnownExpectedFailure()
        {
            var task = GaussianSplat.LoadAsync(InvalidFixtureFactory.SpzV4());
            while (!task.IsCompleted) yield return null;
            Assert.That(task.IsFaulted, Is.True);
            Assert.That(task.Exception.GetBaseException(), Is.TypeOf<System.NotSupportedException>());
        }
    }
}
