using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace trDSP.WaveForms
{
    public class SynForm: IWaveForm
    {
        public double Amplitude { get; set; }
        public double frequency { get; set; }
        public double phase { get; set; }
        public SynForm(double Amplitude, double frequency, double phase)
        {
            this.Amplitude = Amplitude;
            this.frequency = frequency;
            this.phase = phase;
        }
        public double GetPointsForWave(int N, int n, int NoizeStep)
        {
           // double y = Amplitude * Math.Sin(2 * Math.PI * frequency * n / N + (double)phase / 180 * Math.PI);
            return NoizeStep + Amplitude * Math.Sin(2 * Math.PI * frequency * n / N + (double)phase / 180 * Math.PI);
        }
    }
}
