using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Benchmarking
{

    public class DrawDown
    {
        #region Private Fields

        private int _n;
        private int _startIndex, _endIndex, _troughIndex;
        private LinkedList<double> _values;
        #endregion Private Fields

        #region Public Properties

        public int Count { get; set; }
        public double Peak { get; set; }
        public double Trough { get; set; }
        public bool SkipMoveBackDoubleCalc { get; set; }

        public int PeakIndex
        {
            get
            {
                return _startIndex;
            }
        }

        public int TroughIndex
        {
            get
            {
                return _troughIndex;
            }
        }

        //peak to trough return
        public double DrawDownAmount
        {
            get
            {
                return Peak - Trough;
            }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="n">max window for drawdown period</param>
        /// <param name="peak">drawdown peak i.e. start value</param>
        public DrawDown(int n, double peak)
        {
            _n = n - 1;
            _startIndex = _n;
            _endIndex = _n;
            _troughIndex = _n;
            Count = 1;
            _values = new LinkedList<double>();
            _values.AddLast(peak);
            Peak = peak;
            Trough = peak;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// adds a new observation on the drawdown curve
        /// </summary>
        /// <param name="newValue"></param>
        public void Add(double newValue)
        {
            //push the start of this drawdown backwards
            //_startIndex--;
            //the end of the drawdown is the current period end
            _endIndex = _n;
            //the total periods increases with a new observation
            Count++;
            //track what all point values are in the drawdown curve
            _values.AddLast(newValue);
            //update if we have a new trough
            if (newValue < Trough)
            {
                Trough = newValue;
                _troughIndex = _endIndex;
            }
        }

        /// <summary>
        /// Shift this Drawdown backwards in the observation window
        /// </summary>
        /// <param name="trackingNewPeak">whether we are already tracking a new peak or not</param>
        /// <returns>a new drawdown to track if a new peak becomes active</returns>
        public DrawDown MoveBack(bool trackingNewPeak, bool recomputeWindow = true)
        {
            if (!SkipMoveBackDoubleCalc)
            {
                _startIndex--;
                _endIndex--;
                _troughIndex--;
                if (recomputeWindow)
                    return RecomputeDrawdownToWindowSize(trackingNewPeak);
            }
            else
                SkipMoveBackDoubleCalc = false;

            return null;
        }

        #endregion Public Methods

        #region Private Methods

        private DrawDown RecomputeDrawdownToWindowSize(bool trackingNewPeak)
        {
            //the start of this drawdown has fallen out of the start of our observation window, so we have to recalculate the peak of the drawdown
            if (_startIndex < 0)
            {
                Peak = double.NegativeInfinity;
                _values.RemoveFirst();
                Count--;

                //there is the possibility now that there is a higher peak, within the current drawdown curve, than our first observation
                //when we find it, remove all data points prior to this point
                //the new peak must be before the current known trough point
                int iObservation = 0, iNewPeak = 0, iNewTrough = _troughIndex, iTmpNewPeak = 0, iTempTrough = 0;
                double newDrawDown = 0, tmpPeak = 0, tmpTrough = double.NegativeInfinity;
                DrawDown newDrawDownObj = null;
                foreach (var pointOnDrawDown in _values)
                {
                    if (iObservation < _troughIndex)
                    {
                        if (pointOnDrawDown > Peak)
                        {
                            iNewPeak = iObservation;
                            Peak = pointOnDrawDown;
                        }
                    }
                    else if (iObservation == _troughIndex)
                    {
                        newDrawDown = Peak - Trough;
                        tmpPeak = Peak;
                    }
                    else
                    {
                        //now continue on through the remaining points, to determine if there is a nested-drawdown, that is now larger than the newDrawDown
                        //e.g. higher peak beyond _troughIndex, with higher trough than that at _troughIndex, but where new peak minus new trough is > newDrawDown
                        if (pointOnDrawDown > tmpPeak)
                        {
                            tmpPeak = pointOnDrawDown;
                            tmpTrough = tmpPeak;
                            iTmpNewPeak = iObservation;
                            //we need a new drawdown object, as we have a new higher peak
                            if (!trackingNewPeak)
                                newDrawDownObj = new DrawDown(_n + 1, tmpPeak);
                        }
                        else
                        {
                            if (!trackingNewPeak && newDrawDownObj != null)
                            {
                                newDrawDownObj.MoveBack(true, false); //recomputeWindow is irrelevant for this as it will never fall before period 0 in this usage scenario
                                newDrawDownObj.Add(pointOnDrawDown);  //keep tracking this new drawdown peak
                            }

                            if (pointOnDrawDown < tmpTrough)
                            {
                                tmpTrough = pointOnDrawDown;
                                iTempTrough = iObservation;
                                var tmpDrawDown = tmpPeak - tmpTrough;

                                if (tmpDrawDown > newDrawDown)
                                {
                                    newDrawDown = tmpDrawDown;
                                    iNewPeak = iTmpNewPeak;
                                    iNewTrough = iTempTrough;
                                    Peak = tmpPeak;
                                    Trough = tmpTrough;
                                }
                            }
                        }
                    }
                    iObservation++;
                }

                _startIndex = iNewPeak; //our drawdown now starts from here in our observation window
                _troughIndex = iNewTrough;
                for (int i = 0; i < _startIndex; i++)
                {
                    _values.RemoveFirst(); //get rid of the data points prior to this new drawdown peak
                    Count--;
                }
                return newDrawDownObj;
            }
            return null;
        }

        #endregion Private Methods
    }

}