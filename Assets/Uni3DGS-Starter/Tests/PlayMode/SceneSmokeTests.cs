using NUnit.Framework;
using UnityEngine;

namespace Uni3DGS.Starter.Tests
{
    public sealed class SceneSmokeTests
    {
        [Test]
        public void ScenarioControllerCanBeConstructed()
        {
            var go = new GameObject("scenario");
            try
            {
                var controller = go.AddComponent<StarterSceneController>();
                Assert.That(controller, Is.Not.Null);
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }
    }
}
