using System.IO;
using UnityEngine;

namespace Uni3DGS.Starter
{
    public static class StarterFixturePaths
    {
        public static string Ply(string fileName = "SingleSplat.ply")
            => Path.Combine(Application.streamingAssetsPath, "Uni3DGS", "PLY", fileName);

        public static string Spz(string fileName = "Basic-v3.spz")
            => Path.Combine(Application.streamingAssetsPath, "Uni3DGS", "SPZ", fileName);
    }
}
