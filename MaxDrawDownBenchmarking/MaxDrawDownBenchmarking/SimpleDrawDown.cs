using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Benchmarking
{

    public class SimpleDrawDown
    {
        #region Public Properties

        public double Peak { get; set; }
        public double Trough { get; set; }
        public double MaxDrawDown { get; set; }
        #endregion Public Properties

        #region Public Constructors

        public SimpleDrawDown()
        {
            Peak = double.NegativeInfinity;
            Trough = double.PositiveInfinity;
            MaxDrawDown = 0;
        }

        #endregion Public Constructors

        #region Public Methods

        public void Calculate(double newValue)
        {
            if (newValue > Peak)
            {
                Peak = newValue;
                Trough = Peak;
            }
            else if (newValue < Trough)
            {
                Trough = newValue;
                var tmpDrawDown = Peak - Trough;
                if (tmpDrawDown > MaxDrawDown)
                    MaxDrawDown = tmpDrawDown;
            }
        }

        #endregion Public Methods
    }

}