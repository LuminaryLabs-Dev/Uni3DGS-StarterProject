using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Uni3DGS.Starter
{
    public sealed class RuntimeSplatLoader : MonoBehaviour
    {
        public GaussianSplatRenderer Renderer;
        public GaussianCoordinateSystem SourceCoordinates = GaussianCoordinateSystem.Auto;
        public string Path;
        public bool LoadOnStart;

        public GaussianSplat Loaded { get; private set; }
        public string LastError { get; private set; }

        async void Start()
        {
            if (LoadOnStart && !string.IsNullOrWhiteSpace(Path))
                await LoadAsync(Path, SourceCoordinates);
        }

        public async Task LoadAsync(string path, GaussianCoordinateSystem sourceCoordinates)
        {
            LastError = null;
            try
            {
                GaussianSplat next = await GaussianSplat.LoadAsync(path, sourceCoordinates);
                GaussianSplat previous = Loaded;
                Loaded = next;
                if (Renderer != null) Renderer.Splat = next;
                previous?.Dispose();
            }
            catch (Exception ex)
            {
                LastError = ex.GetType().Name + ": " + ex.Message;
                throw;
            }
        }

        public void Unload()
        {
            if (Renderer != null) Renderer.Splat = null;
            Loaded?.Dispose();
            Loaded = null;
        }

        void OnDestroy() => Unload();
    }
}
