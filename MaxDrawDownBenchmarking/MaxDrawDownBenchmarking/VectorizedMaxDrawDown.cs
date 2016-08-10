using System;
using System.Linq;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using System.Diagnostics;

namespace Benchmarking
{

    public static class VectorizedMaxDrawDown
    {
        #region Public Methods

        public static double Run(double[] ccr)
        {
            double[] cumCCR = new double[ccr.Length];
            double summedCcr = 0;
            for (int idx = 0; idx < ccr.Length; idx++)
            {
                summedCcr += ccr[idx];
                cumCCR[idx] = summedCcr;
            }
            var cumulativeCcr = Vector<double>.Build.DenseOfArray(cumCCR);

            cumulativeCcr.PointwiseExp(cumulativeCcr);
            var invCumulativeCcr = 1 / cumulativeCcr;
            var cumulativeCcrMat = (Vector<double>.OuterProduct(cumulativeCcr, invCumulativeCcr) - 1);
            return cumulativeCcrMat.LowerTriangle()
                                   .Enumerate()
                                   .Min();
        }

        #endregion Public Methods
    }

}