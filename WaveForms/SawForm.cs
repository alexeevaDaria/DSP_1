using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace trDSP.WaveForms
{
    public class SawForm:IWaveForm
    {
        public double Amplitude { get; set; }
        public double frequency { get; set; }
        public double phase { get; set; }
        public SawForm(double Amplitude, double frequency, double phase)
        {
            this.Amplitude = Amplitude;
            this.frequency = frequency;
            this.phase = phase;
        }
        public double GetPointsForWave(int N, int n, int NoizeStep)
        {
            return NoizeStep + (2 * Amplitude / Math.PI) * Math.Atan(Math.Tan(Math.PI * frequency * n / N + (double)phase / 2 * 180 * Math.PI));
        }
    }
}
