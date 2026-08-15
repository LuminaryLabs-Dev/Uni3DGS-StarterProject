using NUnit.Framework;
using UnityEngine;

namespace Uni3DGS.Starter.Tests
{
    public sealed class TransformTests
    {
        [Test]
        public void RendererUsesNormalUnityTransform()
        {
            var parent = new GameObject("parent");
            var child = new GameObject("child");
            try
            {
                child.transform.SetParent(parent.transform);
                child.AddComponent<GaussianSplatRenderer>();
                parent.transform.position = new Vector3(10f, 2f, -3f);
                child.transform.localPosition = new Vector3(1f, 2f, 3f);
                child.transform.localRotation = Quaternion.Euler(10f, 20f, 30f);
                child.transform.localScale = new Vector3(2f, 1f, 0.5f);

                Assert.That(child.transform.position, Is.EqualTo(new Vector3(11f, 4f, 0f)));
                Assert.That(child.transform.lossyScale.x, Is.EqualTo(2f).Within(0.001f));
            }
            finally
            {
                Object.DestroyImmediate(parent);
            }
        }
    }
}
