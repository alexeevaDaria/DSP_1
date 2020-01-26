using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace trDSP
{
    public interface IWaveForm
    {
        double Amplitude { get; set; }
        double frequency { get; set; }
        double phase { get; set; }
        //double QFactor { get; set; }

        double GetPointsForWave(int N, int n, int NoizeStep);
    }
}
