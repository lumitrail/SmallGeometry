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
        public double DegreeErrorTolerance
        {
            get => _degreeErrorTolerance;
            set { _degreeErrorTolerance = Math.Abs(value) % 360; }
        }
        /// <summary>
        /// range: 0~double.MaxValue
        /// </summary>
        public double LengthAbsoluteErrorTolerance
        {
            get => _lengthAbsoluteErrorTolerance;
            set { _lengthAbsoluteErrorTolerance = Math.Abs(value); }
        }
        /// <summary>
        /// range: 0~double.MaxValue
        /// </summary>
        public double LengthPercentErrorTolerance
        {
            get => _lengthPercentErrorTolerance;
            set { _lengthPercentErrorTolerance = Math.Abs(value); }
        }


        private double _degreeErrorTolerance = 0;
        private double _lengthAbsoluteErrorTolerance = 0;
        private double _lengthPercentErrorTolerance = 0;


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
            else if (Vector2D.IsParallel(a, b, DegreeErrorTolerance))
            {
                double sizeDiff = Math.Abs(a.Size - b.Size);
                double longerOne = Math.Max(a.Size, b.Size);

                bool isLengthPercentOk = (sizeDiff / longerOne) * 100 < LengthPercentErrorTolerance;
                bool isLengthAbsoluteOk = sizeDiff < LengthAbsoluteErrorTolerance;

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
            return a.GetDistance(b) < LengthAbsoluteErrorTolerance;
        }

        /// <summary>
        /// Are a, b practically the same?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool IsSame(GeoPoint a, GeoPoint b)
        {
            return a.GetDistanceInMeter(b) < LengthAbsoluteErrorTolerance;
        }
    }
}
#endif