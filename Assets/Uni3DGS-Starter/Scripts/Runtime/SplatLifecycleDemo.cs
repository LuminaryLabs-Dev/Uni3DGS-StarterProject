using System.Threading.Tasks;
using UnityEngine;

namespace Uni3DGS.Starter
{
    public sealed class SplatLifecycleDemo : MonoBehaviour
    {
        RuntimeSplatLoader loader;

        public void Bind(RuntimeSplatLoader value) => loader = value;

        public Task LoadPly() => loader.LoadAsync(StarterFixturePaths.Ply(), GaussianCoordinateSystem.Auto);
        public Task ReloadPly() => LoadPly();
        public void Unload() => loader.Unload();
        public void DisableRenderer() { if (loader.Renderer != null) loader.Renderer.enabled = false; }
        public void EnableRenderer() { if (loader.Renderer != null) loader.Renderer.enabled = true; }
    }
}
