using System.Collections.Generic;
using UnityEngine;

namespace Uni3DGS.Starter
{
    public sealed class BenchmarkRecorder
    {
        readonly Queue<float> samples = new();
        const int Window = 120;

        public void Tick()
        {
            samples.Enqueue(Time.unscaledDeltaTime);
            while (samples.Count > Window) samples.Dequeue();
        }

        public float AverageFrameMilliseconds
        {
            get
            {
                if (samples.Count == 0) return 0f;
                float total = 0f;
                foreach (float value in samples) total += value;
                return total / samples.Count * 1000f;
            }
        }
    }
}
