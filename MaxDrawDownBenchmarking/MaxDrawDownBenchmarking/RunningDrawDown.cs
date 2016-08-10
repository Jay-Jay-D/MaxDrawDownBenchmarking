using System.Collections.Generic;

namespace Benchmarking
{
    public class RunningDrawDown
    {
        #region Private Fields

        private int _n;
        private List<DrawDown> _drawdownObjs;
        private DrawDown _currentDrawDown;
        private DrawDown _maxDrawDownObj;
        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// The Peak of the MaxDrawDown
        /// </summary>
        public double DrawDownPeak
        {
            get
            {
                if (_maxDrawDownObj == null) return double.NegativeInfinity;
                return _maxDrawDownObj.Peak;
            }
        }

        /// <summary>
        /// The Trough of the Max DrawDown
        /// </summary>
        public double DrawDownTrough
        {
            get
            {
                if (_maxDrawDownObj == null) return double.PositiveInfinity;
                return _maxDrawDownObj.Trough;
            }
        }

        /// <summary>
        /// The Size of the DrawDown - Peak to Trough
        /// </summary>
        public double DrawDown
        {
            get
            {
                if (_maxDrawDownObj == null) return 0;
                return _maxDrawDownObj.DrawDownAmount;
            }
        }

        /// <summary>
        /// The Index into the Window that the Peak of the DrawDown is seen
        /// </summary>
        public int PeakIndex
        {
            get
            {
                if (_maxDrawDownObj == null) return 0;
                return _maxDrawDownObj.PeakIndex;
            }
        }

        /// <summary>
        /// The Index into the Window that the Trough of the DrawDown is seen
        /// </summary>
        public int TroughIndex
        {
            get
            {
                if (_maxDrawDownObj == null) return 0;
                return _maxDrawDownObj.TroughIndex;
            }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Creates a running window for the calculation of MaxDrawDown within the window
        /// </summary>
        /// <param name="n">the number of periods within the window</param>
        public RunningDrawDown(int n)
        {
            _n = n;
            _currentDrawDown = null;
            _drawdownObjs = new List<DrawDown>();
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// The new value to add onto the end of the current window (the first value will drop off)
        /// </summary>
        /// <param name="newValue">the new point on the curve</param>
        public void Calculate(double newValue)
        {
            if (double.IsNaN(newValue)) return;

            if (_currentDrawDown == null)
            {
                var drawDown = new DrawDown(_n, newValue);
                _currentDrawDown = drawDown;
                _maxDrawDownObj = drawDown;
            }
            else
            {
                //shift current drawdown back one. and if the first observation falling outside the window means we encounter a new peak after the current trough, we start tracking a new drawdown
                var drawDownFromNewPeak = _currentDrawDown.MoveBack(false);

                //this is a special case, where a new lower peak (now the highest) is created due to the drop of of the pre-existing highest peak, and we are not yet tracking a new peak
                if (drawDownFromNewPeak != null)
                {
                    _drawdownObjs.Add(_currentDrawDown); //record this drawdown into our running drawdowns list)
                    _currentDrawDown.SkipMoveBackDoubleCalc = true; //MoveBack() is calculated again below in _drawdownObjs collection, so we make sure that is skipped this first time
                    _currentDrawDown = drawDownFromNewPeak;
                    _currentDrawDown.MoveBack(true);
                }

                if (newValue > _currentDrawDown.Peak)
                {
                    //we need a new drawdown object, as we have a new higher peak
                    var drawDown = new DrawDown(_n, newValue);
                    //do we have an existing drawdown object, and does it have more than 1 observation
                    if (_currentDrawDown.Count > 1)
                    {
                        _drawdownObjs.Add(_currentDrawDown); //record this drawdown into our running drawdowns list)
                        _currentDrawDown.SkipMoveBackDoubleCalc = true; //MoveBack() is calculated again below in _drawdownObjs collection, so we make sure that is skipped this first time
                    }
                    _currentDrawDown = drawDown;
                }
                else
                {
                    //add the new observation to the current drawdown
                    _currentDrawDown.Add(newValue);
                }
            }

            //does our new drawdown surpass any of the previous drawdowns?
            //if so, we can drop the old drawdowns, as for the remainer of the old drawdowns lives in our lookup window, they will be smaller than the new one
            var newDrawDown = _currentDrawDown.DrawDownAmount;
            _maxDrawDownObj = _currentDrawDown;
            var maxDrawDown = newDrawDown;
            var keepDrawDownsList = new List<DrawDown>();
            foreach (var drawDownObj in _drawdownObjs)
            {
                drawDownObj.MoveBack(true);
                if (drawDownObj.DrawDownAmount > newDrawDown)
                {
                    keepDrawDownsList.Add(drawDownObj);
                }

                //also calculate our max drawdown here
                if (drawDownObj.DrawDownAmount > maxDrawDown)
                {
                    maxDrawDown = drawDownObj.DrawDownAmount;
                    _maxDrawDownObj = drawDownObj;
                }
            }
            _drawdownObjs = keepDrawDownsList;
        }

        #endregion Public Methods
    }
}