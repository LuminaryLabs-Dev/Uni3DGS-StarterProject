using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Uni3DGS.Starter
{
    public sealed class MultipleSplatDemo : MonoBehaviour
    {
        readonly List<GaussianSplat> splats = new();
        readonly List<GameObject> objects = new();

        public async Task BuildAsync(string path, int count = 3)
        {
            Clear();
            for (int i = 0; i < count; i++)
            {
                var splat = await GaussianSplat.LoadAsync(path);
                splats.Add(splat);

                var go = new GameObject($"Gaussian Splat {i + 1}");
                go.transform.SetParent(transform, false);
                go.transform.localPosition = new Vector3((i - (count - 1) * 0.5f) * 2f, 0f, 0f);
                var renderer = go.AddComponent<GaussianSplatRenderer>();
                renderer.Splat = splat;
                objects.Add(go);
            }
        }

        public void Clear()
        {
            foreach (var go in objects)
                if (go != null) Destroy(go);
            objects.Clear();

            foreach (var splat in splats) splat?.Dispose();
            splats.Clear();
        }

        void OnDestroy() => Clear();
    }
}
