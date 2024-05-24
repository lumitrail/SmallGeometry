using SmallGeometry.Exceptions;
using SmallGeometry.Primitives;

namespace SmallGeometry.Euclidean
{
    /// <summary>
    /// Immutable line of FlatPoints.
    /// </summary>
    public class FlatLine : IEnumerable<FlatPoint>
    {
        /// <summary>
        /// 
        /// </summary>
        public CoordinateSystem CoordinateSystem { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public FlatPoint this[int idx] => new FlatPoint(Points[idx], CoordinateSystem);

        /// <summary>
        /// Number of points.
        /// </summary>
        public int Count => Points.Count;

        /// <summary>
        /// 
        /// </summary>
        public FlatBoundingBox BoundingBox { get; }

        /// <summary>
        /// 
        /// </summary>
        private IReadOnlyList<Coordinate2D> Points { get; }
        private double _length = -1;


        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <exception cref="ArgumentException">points is empty</exception>
        /// <exception cref="ArgumentNullException">points is null</exception>
        /// <exception cref="Exceptions.CoordinateSystemNoneException">points contains both None and else</exception>
        /// <exception cref="Exceptions.TransformException"></exception>
        public FlatLine(IEnumerable<FlatPoint> points)
        {
            ArgumentNullException.ThrowIfNull(points);
            
            int pointsCount = points.Count();
            if (pointsCount == 0)
            {
                throw new ArgumentException(ExceptionMessages.PointsCountZero, nameof(points));
            }
            else if (pointsCount == 1)
            {
                Points = [points.First().Coordinate2D, points.First().Coordinate2D];
            }
            else
            {
                Points = points.Select(p => p.Coordinate2D).ToList();
            }
            CoordinateSystem = points.First().CoordinateSystem;
            BoundingBox = new FlatBoundingBox(Points, CoordinateSystem);
        }

        /// <inheritdoc cref="FlatLine.FlatLine(IEnumerable{FlatPoint})"/>
        public FlatLine(params FlatPoint[] points)
            : this(points.AsEnumerable())
        {
        }
        #endregion


        #region Static functions
        

        /// <summary>
        /// Transform points into one coordinate system(To points.First()).
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">points is empty</exception>
        /// <exception cref="ArgumentNullException">points is null</exception>
        /// <exception cref="Exceptions.CoordinateSystemNoneException">points contains both None and else</exception>
        /// <exception cref="Exceptions.TransformException"></exception>
        private static IReadOnlyList<FlatPoint> HomogenizeCoordinateSystem(IEnumerable<FlatPoint> points)
        {
            ArgumentNullException.ThrowIfNull(points);
            int pointsCount = points.Count();

            if (pointsCount == 0)
            {
                throw new ArgumentException(ExceptionMessages.PointsCountZero, nameof(points));
            }

            CoordinateSystem[] coordinateSystems = points.Select(p => p.CoordinateSystem).Distinct().ToArray();
            System.Diagnostics.Debug.Assert(coordinateSystems.Length != 0);

            if (coordinateSystems.Length == 1)
            {
                return points.ToArray();
            }
            if (coordinateSystems.Contains(CoordinateSystem.None))
            {
                throw new Exceptions.CoordinateSystemNoneException(nameof(points));
            }

            CoordinateSystem transformTo = points.First().CoordinateSystem;

            IReadOnlyList<FlatPoint> result = points
                .Select(p => p.Transform(transformTo))
                .ToList();

            return result;
        }
        #endregion


        #region Flat line alone
        /// <summary>
        /// Gets Euclidean length.
        /// </summary>
        /// <returns></returns>
        public double GetLength()
        {
            if (_length < 0)
            {
                _length = 0;
                for (int i = 1; i < Count; ++i)
                {
                    _length += this[i - 1].GetDistance(this[i]);
                }
            }

            return _length;
        }

        /// <summary>
        /// Gets line segments of the line.
        /// </summary>
        /// <returns></returns>
        public List<FlatLineSegment> GetLineSegments()
        {
            var result = new List<FlatLineSegment>(Count - 1);

            for (int i = 1; i < Count; ++i)
            {
                var s = new FlatLineSegment(this[i - 1], this[i]);
                result.Add(s);
            }

            return result;
        }

        /// <summary>
        /// Gets a line segment of 0-based index i.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public FlatLineSegment GetLineSegment(int i)
        {
            if (i < 0 || i >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(i));
            }

            return new FlatLineSegment(this[i], this[i - 1]);
        }

        /// <summary>
        /// Get a copy of line with same points follwing previous point removed.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">line is empty</exception>
        /// <exception cref="ArgumentNullException">line is null</exception>
        /// <exception cref="Exceptions.CoordinateSystemNoneException">points contains both None and else</exception>
        /// <exception cref="Exceptions.TransformException"></exception>
        public static List<FlatPoint> RemoveDuplicatedPoints(IEnumerable<FlatPoint> line)
        {
            IReadOnlyList<FlatPoint> transformed = HomogenizeCoordinateSystem(line);
            var result = new List<FlatPoint>(transformed.Count)
            {
                transformed[0]
            };

            foreach (FlatPoint fp in transformed)
            {
                if (fp != result.Last())
                {
                    result.Add(fp);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FlatLine GetReversedCopy()
        {
            var reversedPoints = new List<FlatPoint>(this);
            reversedPoints.Reverse();
            return new FlatLine(reversedPoints);
        }

        /// <summary>
        /// Copies this and generates flatpoints on the line so that distance between two consecutive points is no larger than maxDistance.
        /// </summary>
        /// <param name="maxDistance"></param>
        /// <returns></returns>
        public FlatLine Interpolate(int maxDistance)
        {
            int estimatedCount = (int)(GetLength() / maxDistance) + Count;
            var result = new List<FlatPoint>(estimatedCount);

            FlatPoint previousPoint = this[0];
            foreach (FlatPoint p in Points)
            {
                double distance = previousPoint.GetDistance(p);

                if (distance > maxDistance)
                {
                    int divisons = (int)(distance / maxDistance);

                    Vector v = new Vector(previousPoint, p).GetNormalizedVector() * maxDistance;

                    for (int i=0; i<divisons; ++i)
                    {
                        FlatPoint sectionPoint = previousPoint + (i * v);
                        result.Add(sectionPoint);
                    }
                }

                result.Add(p);
                previousPoint = p;
            }

            return new FlatLine(result);
        }
        #endregion


        #region Flat line and point
        /// <summary>
        /// Gets points on the line nearest to target.
        /// </summary>
        /// <param name="target"></param>
        /// <returns>key: 0-based index of line segment</returns>
        /// <remarks>There can be multiple results when their distances to the line are exactly the same.</remarks>
        public List<KeyValuePair<int, FlatPoint>> GetNearestPoints(FlatPoint target)
        {
            double minDistance = double.MaxValue;
            var segmentWiseH = new List<(int index, FlatPoint p, double distance)>(Count - 1);

            for (int i=0; i<Count-1; ++i)
            {
                FlatLineSegment currentSegment = GetLineSegment(i);
                FlatPoint H = currentSegment.GetNearestPointTo(target);
                double distance = H.GetDistance(target);

                if (distance <= minDistance)
                {
                    minDistance = distance;
                    segmentWiseH.Add((i, H, distance));
                }
            }

            var result = new List<KeyValuePair<int, FlatPoint>>(segmentWiseH.Count);
            foreach (var d in segmentWiseH)
            {
                if (d.distance <= minDistance)
                {
                    result.Add(new KeyValuePair<int, FlatPoint>(d.index, d.p));
                }
            }

            return result;
        }

        /// <summary>
        /// Determins if target is on the right of the line segment nearest to it.
        /// </summary>
        /// <param name="target"></param>
        /// <returns>key: 0-based index of line segment</returns>
        /// <remarks>There can be multiple results when their distances to the line are exactly the same.</remarks>
        public List<KeyValuePair<int, bool>> IsPointOnTheRightSide(FlatPoint target)
        {
            List<KeyValuePair<int, FlatPoint>> nearestPoints = GetNearestPoints(target);

            var result = new List<KeyValuePair<int, bool>>(nearestPoints.Count);

            foreach (var kv in nearestPoints)
            {
                Vector lineDirection = GetLineSegment(kv.Key).GetVector();
                Vector nearestToTarget = new Vector(kv.Value, target);
                double trueWhenPositive = Vector.CrossProduct(nearestToTarget, lineDirection);

                result.Add(new KeyValuePair<int, bool>(kv.Key, trueWhenPositive > 0));
            }

            return result;
        }

        /// <summary>
        /// Trim both end of line, nearest to startPoint and endPoint(line is reversed to startPoint and endPoint).
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="includeStartEndPoint"></param>
        /// <returns></returns>
        public FlatLine Trim(FlatPoint startPoint, FlatPoint endPoint, bool includeStartEndPoint)
        {
            KeyValuePair<int, FlatPoint> startNearest = GetNearestPoints(startPoint).First();
            KeyValuePair<int, FlatPoint> endNearest = GetNearestPoints(endPoint).Last();

            var result = new List<FlatPoint>(Count + 2);

            bool reverse = false;
            if (startNearest.Key > endNearest.Key)
            {

            }
            else if (startNearest.Key == endNearest.Key)
            {
                Vector toStartNearest = 
            }
        }
        #endregion


        #region Line and line
        /// <summary>
        /// Gets intersections of lines.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exceptions.CoordinateSystemNoneException"></exception>
        /// <exception cref="Exceptions.TransformException"></exception>
        public static List<FlatPoint> GetIntersections(FlatLine a, FlatLine b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);

            FlatLine bTrans = new FlatLine(HomogenizeCoordinateSystem(b));

            // Max possible result.Count is (a.Count * b.Count), but...
            var result = new List<FlatPoint>(a.Count + bTrans.Count);

            List<FlatLineSegment> aSegments = a.GetLineSegments();
            List<FlatLineSegment> bSegments = bTrans.GetLineSegments();

            foreach (var aSeg in aSegments)
            {
                foreach (var bSeg in bSegments)
                {
                    FlatPoint? intersection = aSeg.FindIntersectingPointOrNull(bSeg);

                    if (intersection.HasValue)
                    {
                        result.Add(intersection.Value);
                    }
                }
            }

            return result;
        }


        /// <inheritdoc cref="GetIntersections(FlatLine, FlatLine)"/>
        public List<FlatPoint> GetIntersections(FlatLine b)
        {
            return GetIntersections(this, b);
        }
        #endregion

        /// <summary>
        /// [[x,y],[x,y],...,[x,y]]
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[{string.Join(',', Points.Select(p => p.ToString()))}]";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<FlatPoint> GetEnumerator()
        {
            return Points.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Points.GetEnumerator();
        }
    }
}
