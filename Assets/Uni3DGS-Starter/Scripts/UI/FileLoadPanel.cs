using System;
using UnityEngine;

namespace Uni3DGS.Starter
{
    public sealed class FileLoadPanel
    {
        public string Path = string.Empty;
        public GaussianCoordinateSystem Coordinates = GaussianCoordinateSystem.Auto;

        public void Draw(Func<string, GaussianCoordinateSystem, System.Threading.Tasks.Task> load)
        {
            GUILayout.Label("Runtime file path");
            Path = GUILayout.TextField(Path);
            GUILayout.Label("Coordinates: " + Coordinates);
            GUILayout.BeginHorizontal();
            foreach (GaussianCoordinateSystem value in Enum.GetValues(typeof(GaussianCoordinateSystem)))
                if (GUILayout.Button(value.ToString())) Coordinates = value;
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Load") && !string.IsNullOrWhiteSpace(Path))
                _ = load(Path, Coordinates);
        }
    }
}
