using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace Benchmarking
{
    internal class BenchmarkingDrawDown
    {
        #region Public Methods

        public static void Main(string[] args)
        {
            int iterations = 4000;
            int obs = 500;
            var rng = new Normal(0, 0.001);
            List<double[]> ccrList = new List<double[]>();
            for (int i = 0; i < iterations; i++)
            {
                double[] c = new double[obs];
                rng.Samples(c);
                ccrList.Add(c);
            }

            #region Vectorized

            GC.Collect();
            var mdd = VectorizedMaxDrawDown.Run(ccrList.First()); // run once outside of loop to avoid initialization costs
            Console.WriteLine("Vectorized");
            Stopwatch sw = Stopwatch.StartNew();
            foreach (var ccr in ccrList)
            {
                mdd = VectorizedMaxDrawDown.Run(ccr);
            }
            sw.Stop();
            Console.WriteLine((sw.ElapsedTicks));
            #endregion Vectorized

            #region SimpleDrawDown Class

            GC.Collect();
            // run once outside of loop to avoid initialization costs
            mdd = SimpleDrawDown.Run(ccrList.First());
            Console.WriteLine("SimpleDrawDown");
            sw.Restart();
            foreach (var ccr in ccrList)
            {
                mdd = SimpleDrawDown.Run(ccr);
            }
            sw.Stop();
            Console.WriteLine((sw.ElapsedTicks));
            #endregion SimpleDrawDown Class

            #region DrawDown Class

            GC.Collect();
            var maxDrawDown = new DrawDown(obs, 0);
            // run once outside of loop to avoid initialization costs
            foreach (var val in ccrList.First())
            {
                maxDrawDown.Add(val);
            }
            Console.WriteLine("DrawDown Class");
            sw.Restart();
            foreach (var ccr in ccrList)
            {
                foreach (var val in ccr)
                {
                    maxDrawDown.Add(val);
                }
            }
            sw.Stop();
            Console.WriteLine((sw.ElapsedTicks));
            #endregion DrawDown Class

            #region RunningDrawDown Class

            GC.Collect();
            var runningMaxDrawDown = new RunningDrawDown(obs);
            // run once outside of loop to avoid initialization costs
            foreach (var val in ccrList.First())
            {
                runningMaxDrawDown.Calculate(val);
            }
            Console.WriteLine("RunningDrawDown Class");
            sw.Restart();
            foreach (var ccr in ccrList)
            {
                foreach (var val in ccr)
                {
                    maxDrawDown.Add(val);
                }
            }
            sw.Stop();
            Console.WriteLine((sw.ElapsedTicks));

            #endregion RunningDrawDown Class

            Console.Read();
        }

        #endregion Public Methods
    }
}