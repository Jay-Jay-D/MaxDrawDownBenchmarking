using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Benchmarking
{
    public static class SimpleDrawDown
    {
        public static double Run(double[] ccr)
        {
            var peak = double.NegativeInfinity;
            var trough = double.PositiveInfinity;
            var maxDrawDown = 1d;
            var cumCcr = 0d;
            var newValue = 0d;

            foreach (var obs in ccr)
            {
                cumCcr += obs;
                newValue = Math.Exp(cumCcr);
                if (newValue > peak)
                {
                    peak = newValue;
                    trough = peak;
                }
                else if (newValue < trough)
                {
                    trough = newValue;
                    var tmpDrawDown = trough / peak;
                    if (tmpDrawDown < maxDrawDown)
                        maxDrawDown = tmpDrawDown;
                }
            }
            return 1-maxDrawDown;
        }
    }
}