#if DEBUG
using SmallGeometry.Euclidean;
using SmallGeometry.Geographic;
using SmallGeometry.Primitives;

namespace SmallGeometry
{
    /// <summary>
    /// Value comparing with some tolerance.
    /// </summary>
    public class SmallGeometryComparer
    {
        /// <summary>
        /// range: 0~360
        /// </summary>
        public double DegreeTolerance
        {
            get => _degreeTolerance;
            set { _degreeTolerance = Math.Abs(value) % 360; }
        }
        /// <summary>
        /// range: 0~double.MaxValue
        /// </summary>
        public double DoubleAbsoluteTolerance
        {
            get => _doubleAbsoulteTolerance;
            set { _doubleAbsoulteTolerance = Math.Abs(value); }
        }
        /// <summary>
        /// range: 0~double.MaxValue
        /// </summary>
        public double DoublePercentTolerance
        {
            get => _doublePercentTolerance;
            set { _doublePercentTolerance = Math.Abs(value); }
        }


        private double _degreeTolerance = 0.001;
        private double _doubleAbsoulteTolerance = 0.001;
        private double _doublePercentTolerance = 0.1;


        /// <summary>
        /// 
        /// </summary>
        public SmallGeometryComparer()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="degreeTolerance"></param>
        /// <param name="doubleAbsoulteTolerance"></param>
        /// <param name="doublePercentTolerance"></param>
        public SmallGeometryComparer(double degreeTolerance, double doubleAbsoulteTolerance, double doublePercentTolerance)
        {
            _degreeTolerance = degreeTolerance;
            _doubleAbsoulteTolerance = doubleAbsoulteTolerance;
            _doublePercentTolerance = doublePercentTolerance;
        }

        /// <summary>
        /// Are two vectors practically the same?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool IsSame(Vector2D a, Vector2D b)
        {
            if (a.Size == 0 && b.Size == 0)
            {
                return true;
            }
            else if (Vector2D.IsParallel(a, b, DegreeTolerance))
            {
                double sizeDiff = Math.Abs(a.Size - b.Size);
                double longerOne = Math.Max(a.Size, b.Size);

                bool isLengthPercentOk = (sizeDiff / longerOne) * 100 < DoublePercentTolerance;
                bool isLengthAbsoluteOk = sizeDiff < DoubleAbsoluteTolerance;

                return isLengthPercentOk && isLengthAbsoluteOk;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Are a, b practically the same?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool IsSame(FlatPoint a, FlatPoint b)
        {
            return a.GetDistance(b) < DoubleAbsoluteTolerance;
        }

        /// <summary>
        /// Are a, b practically the same?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool IsSame(GeoPoint a, GeoPoint b)
        {
            return a.GetDistanceInMeter(b) < DoubleAbsoluteTolerance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="truth"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsRelativelySame(double truth, double value)
        {
            double diff = Math.Abs(value / truth);
            double diffPercent = 100 * diff / truth;

            return diffPercent < DoublePercentTolerance;
        }
    }
}
#endif