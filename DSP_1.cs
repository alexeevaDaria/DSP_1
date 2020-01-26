using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using trDSP.WaveForms;

namespace trDSP
{

    public partial class DSP_1 : Form
    {
        private static Random _random = new Random();
        private static int NoizeStep { get { return _random.Next(-100, 100); } }
        private static bool noize;
        private static bool radioButtonSignal;// true - harmonic
                                              // false - polyharmonic

        System.Windows.Forms.DataVisualization.Charting.Series DataSer;
        System.Windows.Forms.DataVisualization.Charting.Legend HarS;
        private List<TrackBar> trLst = new List<TrackBar>();
        private List<Label> lbLst = new List<Label>();
        double A, f, phi;
        double[] phc = new double[15];
        Signal signal;

        public DSP_1()
        {
            for (int i = 0; i <= 14; i++)
            {
                if (i % 3 == 0)
                    phc[i] = 360;
                else
                    phc[i] = 100;
            }
            InitializeComponent();

            comboBoxSignalType.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;

            BuildHarmGraph();
           
        }

        private void BuildHarmGraph()
        {
            signal = (Signal)comboBoxSignalType.SelectedIndex;
            try
            {
                chart1.Series.Remove(DataSer);
            }
            finally
            {
                DataSer = new System.Windows.Forms.DataVisualization.Charting.Series();
                HarS = new System.Windows.Forms.DataVisualization.Charting.Legend();
                DataSer.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                DataSer.Color = Color.Red;
                HarS.Name = "Harmonic signal";
                HarS.Title = "Harmonic signal";
                //chart1.Legends.Add(HarS);
                int N = 1024;
                A = (double)trackBar2.Value;
                f = (double)trackBar3.Value;
                phi = (double)trackBar1.Value;
                double Q = 0.25;
                label4.Text = Convert.ToString(trackBar2.Value);
                label5.Text = Convert.ToString(trackBar3.Value);
                label6.Text = Convert.ToString(trackBar1.Value);
                if(signal == Signal.rectangle)
                    DataSer.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StepLine;

                IWaveForm instance = null;

                switch (signal)
                {
                    case Signal.syn:
                    {
                            instance = new SynForm(A, f, phi);
                            break;
                    }
                    case Signal.rectangle:
                    {
                            instance = new RectangleForm(A, f, phi, Q);
                            break;
                    }
                    case Signal.triangle:
                        {
                            instance = new TriangleForm(A, f, phi);
                            break;
                        }
                    case Signal.saw:
                        {
                            instance = new SawForm(A, f, phi);
                            break;
                        }
                }
                for (int n = 1; n <= N - 1; n++)
                {
                    switch (signal)
                    {
                    case Signal.rectangle:
                        {
                            DataSer.Points.AddXY((double)n / N, instance.GetPointsForWave(N, n, noize ? NoizeStep : 0));                             
                            break;
                        }
                        default:
                        {
                             DataSer.Points.AddXY(n, instance.GetPointsForWave(N, n, noize ? NoizeStep : 0));
                             break;
                        }
                    }
                    
                }
            }
            chart1.ResetAutoValues();
            chart1.Series.Add(DataSer);
        }

        private void BuildPolyHarmGraph(object sender, EventArgs e)
        {
            signal = (Signal)comboBoxSignalType.SelectedIndex;
            try
            {
                chart1.Series.Remove(DataSer);
            }
            finally
            {

                for (int i = 0; i <= 14; i++)
                {                  
                    if (comboBox5.SelectedIndex*3 == i)
                        phc[i] = (double)trackBar1.Value;
                    if (comboBox5.SelectedIndex*3 + 1 == i)
                        phc[i] = (double)trackBar3.Value;
                    if (comboBox5.SelectedIndex*3 + 2 == i)
                        phc[i] = (double)trackBar2.Value;

                    // lbLst[i].Text = Convert.ToString(trLst[i].Value);
                    //phc[i] = trLst[i].Value;
                }
                label4.Text = Convert.ToString(trackBar2.Value);
                label5.Text = Convert.ToString(trackBar3.Value);
                label6.Text = Convert.ToString(trackBar1.Value);

                DataSer = new System.Windows.Forms.DataVisualization.Charting.Series();
                HarS = new System.Windows.Forms.DataVisualization.Charting.Legend();
                DataSer.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                DataSer.Color = Color.Red;
                HarS.Name = "Harmonic signal";
                HarS.Title = "Harmonic signal";

                double Q = 0.5;

                IWaveForm[] arrWaves = new IWaveForm[5];
               
                for (int i =0; i<5; i++)
                {

                    switch (signal)
                    {
                        case Signal.syn:
                            {
                                arrWaves[i] = new SynForm(phc[i * 3 + 2], phc[i * 3 + 1], phc[i * 3]);                          
                                break;
                            }
                        case Signal.rectangle:
                            {
                                arrWaves[i] = new RectangleForm(phc[i * 3 + 2], phc[i * 3 + 1], phc[i * 3], Q);
                                break;
                            }
                        case Signal.triangle:
                            {
                                arrWaves[i] = new TriangleForm(phc[i * 3 + 2], phc[i * 3 + 1], phc[i * 3]);
                                break;
                            }
                        default:
                            {
                                arrWaves[i] = new SawForm(phc[i * 3 + 2], phc[i * 3 + 1], phc[i * 3]);
                                break;
                            }
                    }
                }               
              
                double t = 0;
                int N = 1024;

                if(signal == Signal.rectangle)
                    DataSer.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StepLine;


                for (int n = 1; n <= N - 1; n++)
                {
                    //создать 5 объектов с разеными параметрами и по очерелди их перебиратьъ

                    double res = 0;
                    //res = WaveBase.GetPointsForWave(N, n, noize ? NoizeStep : 0);
                    for (int k = 0; k < 5; k++)
                    {
                       // tarrWaves[0].Amplitude
                        t = arrWaves[k].GetPointsForWave(N, n, noize ? NoizeStep : 0);
                        //t= (phc[k*3 + 2]) * Math.Sin(2 * Math.PI * (phc[k*3 + 1]) * n / N + (double)(phc[k*3]) / 180 * Math.PI);
                        // double t = (3 * phc[k * 3 + 2]) * Math.Sin(2 * Math.PI * (3 * phc[k * 3 + 1]) * n / N + (double)(3 * phc[k * 3]) / 180 * Math.PI);
                        res += t;
                    }
                    if (signal == Signal.rectangle)
                        DataSer.Points.AddXY((double)n / N, res);
                    else
                        DataSer.Points.AddXY(n, res);
                }
            }
            chart1.ResetAutoValues();
            chart1.Series.Add(DataSer);
        }

        private void BuildModulationGraph(object sender, EventArgs e, Signal init, Signal fin)
        {
            Signal signalInit = init;
            Signal signalFin = fin;
            ModulationType modulationType = (ModulationType)comboBox4.SelectedIndex;
            try
            {
                chart1.Series.Remove(DataSer);
            }
            finally
            {
                DataSer = new System.Windows.Forms.DataVisualization.Charting.Series();
                HarS = new System.Windows.Forms.DataVisualization.Charting.Legend();
                DataSer.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                DataSer.Color = Color.Red;
                HarS.Name = "Harmonic signal";
                HarS.Title = "Harmonic signal";

                //chart1.Legends.Add(HarS);
                int N = 1024;
                A = (double)trackBar2.Value;
                f = (double)trackBar3.Value;
                phi = (double)trackBar1.Value; 

             //   double correction = (double)trackBar19.Value;

               // double Q = 0.25;
                label4.Text = Convert.ToString(trackBar2.Value);
                label5.Text = Convert.ToString(trackBar3.Value);
                label6.Text = Convert.ToString(trackBar1.Value);
                //  label37.Text = Convert.ToString(trackBar19.Value);

                 modulationType = (ModulationType)comboBox4.SelectedIndex;
                Modulation newWave = new Modulation(A, f, phi, modulationType);

                newWave.CreateInit(signalInit);
                newWave.CreateFin(signalFin);

                double t = 0;

                for (int n = 1; n <= N - 1; n++)
                {
                    t = newWave.GetPointsForWave(N, n);
                   /* double res = 0;
                    for (int k = 0; k <= 1; k++)
                    {
                        t = arrWaves[k].GetPointsForWave(N, n, noize ? NoizeStep : 0);
                        //t= (phc[k*3 + 2]) * Math.Sin(2 * Math.PI * (phc[k*3 + 1]) * n / N + (double)(phc[k*3]) / 180 * Math.PI);
                        // double t = (3 * phc[k * 3 + 2]) * Math.Sin(2 * Math.PI * (3 * phc[k * 3 + 1]) * n / N + (double)(3 * phc[k * 3]) / 180 * Math.PI);
                        res += t;
                    }*/
                    //if(DataSer.Points.)
                     DataSer.Points.AddXY(n, t);
                }
            }
            chart1.ResetAutoValues();
            chart1.Series.Add(DataSer);
        }

        //объединить
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex != 2)
            comboBox1_SelectedIndexChanged(sender, e);
            /*if (comboBox1.SelectedIndex == 0)
                BuildHarmGraph();
            if (comboBox1.SelectedIndex == 2)
                BuildModulationGraph(sender, e);*/
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != 2)
                comboBox1_SelectedIndexChanged(sender, e);
            /*if (comboBox1.SelectedIndex == 0)
                BuildHarmGraph();
            if (comboBox1.SelectedIndex == 2)
                BuildModulationGraph(sender, e);*/
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != 2)
                comboBox1_SelectedIndexChanged(sender, e);
            /*if (comboBox1.SelectedIndex == 0)
                BuildHarmGraph();
            if (comboBox1.SelectedIndex == 2)
                BuildModulationGraph(sender, e);*/
        }

        private void comboBoxSignalType_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1_SelectedIndexChanged(sender, e);
        }


        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1_SelectedIndexChanged(sender, e);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1_SelectedIndexChanged(sender, e);
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1_SelectedIndexChanged(sender, e);
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            trackBar1.Value = (int)phc[comboBox5.SelectedIndex*3];
            trackBar3.Value = (int)phc[comboBox5.SelectedIndex*3 + 1];
            trackBar2.Value = (int)phc[comboBox5.SelectedIndex*3 + 2];

            label4.Text = Convert.ToString(trackBar2.Value);
            label5.Text = Convert.ToString(trackBar3.Value);
            label6.Text = Convert.ToString(trackBar1.Value);
            comboBox1_SelectedIndexChanged(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BuildModulationGraph(sender, e, Signal.syn, Signal.triangle);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BuildModulationGraph(sender, e, Signal.triangle, Signal.syn);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            BuildModulationGraph(sender, e, Signal.saw, Signal.triangle);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void NoizeBox_CheckedChanged(object sender, EventArgs e)
        {
            noize = NoizeBox.Checked;
            comboBox1_SelectedIndexChanged(sender, e);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                groupBox1.Enabled = true;
                groupBox4.Enabled = false;
                groupBox9.Enabled = false;

                BuildHarmGraph();
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                groupBox1.Enabled = true;
                groupBox4.Enabled = true;
                groupBox9.Enabled = false;

                BuildPolyHarmGraph(sender, e);
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                groupBox1.Enabled = true;
                groupBox4.Enabled = false;
                groupBox9.Enabled = true;

                button1_Click(sender, e);
            }
            else
                return;
        }

               

    }

}
