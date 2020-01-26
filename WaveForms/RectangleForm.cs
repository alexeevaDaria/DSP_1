using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace trDSP.WaveForms
{
    public class RectangleForm : IWaveForm
    {
        public double Amplitude { get; set; }
        public double frequency { get; set; }
        public double phase { get; set; }

        public double QForm { get; set; }
        public RectangleForm(double Amplitude, double frequency, double phase, double Q)
        {
            this.Amplitude = Amplitude;
            this.frequency = frequency;
            this.phase = phase;
            this.QForm = Q;
        }
        public double GetPointsForWave(int N, int n, int NoizeStep)
        {
            return NoizeStep + Amplitude * GetQ(n, N);
        }

        private double GetQ(int n, int N)
        {
            double sin = Math.Sin(2 * Math.PI * frequency * n / N + (double)phase / 180 * Math.PI) + 1;
            return sin >= QForm
                ? 1
                : 0;
        }
    }
}
