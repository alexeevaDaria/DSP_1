using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using trDSP.WaveForms;

namespace trDSP
{
    public class Modulation
    {

        IWaveForm waveInit;
        IWaveForm waveFin;
        double Amplitude { get; set; }
        double frequency { get; set; }
        double phase { get; set; }

        ModulationType modulationType;

      //  double correction;

        public Modulation(double A, double f, double phi, ModulationType modType)
        {
            this.Amplitude = A;
            this.frequency = f;
            this.phase = phi;
            this.modulationType = modType;
        }

        public void CreateInit(Signal signal)
        {
            switch (signal)
            {
                case Signal.syn:
                    {
                        waveInit = new SynForm(Amplitude, frequency, phase);
                        break;
                    }
                /*case Signal.rectangle:
                    {
                        waveInit = new RectangleForm(Amplitude, frequency, phase, Q);
                        break;
                    }*/
                case Signal.triangle:
                    {
                        waveInit = new TriangleForm(Amplitude, frequency, phase);
                        break;
                    }
                case Signal.saw:
                    {
                        waveInit = new SawForm(Amplitude, frequency, phase);
                        break;
                    }
            }
        }

        public void CreateFin(Signal signal)
        {
            double temp1, temp2;

           // temp1 = Amplitude;
          //  temp2 = frequency;

            if (modulationType == ModulationType.amplitude)//amplitude
            {
                temp1 = Amplitude;
                temp2 = 2;
            }
            else
            {
                temp1 = 20;
                temp2 = frequency;
            }

            switch (signal)
            {
                //генерируется по своим данным и потом по тем же но только с другой амплитудой или частотой в зависимости от значений
                case Signal.syn:
                    {
                        waveFin = new SynForm(temp1, temp2, phase);
                        break;
                    }
                /*case Signal.rectangle:
                    {
                        waveInit = new RectangleForm(Amplitude, frequency, phase, Q);
                        break;
                    }*/
                case Signal.triangle:
                    {
                        waveFin = new TriangleForm(temp1, temp2, phase);
                        break;
                    }
                case Signal.saw:
                    {
                        waveFin = new SawForm(temp1, temp2, phase);
                        break;
                    }
            }
        }

        public double GetPointsForWave(int N, int n)
        {
            /* double w1 = waveInit.GetPointsForWave(N, n, 0);
             double w2 = waveFin.GetPointsForWave(N, n, 0);
             return w1+w2;*/
            double temp;
            
            if (modulationType == ModulationType.amplitude)
            {
                temp = waveFin.GetPointsForWave(N, n, 0);
                waveInit.Amplitude = temp;
            }
            else
            {
                waveInit.frequency = waveFin.GetPointsForWave(N, n, 0);
            }
            return waveInit.GetPointsForWave(N, n, 0);
        }
    }
}
