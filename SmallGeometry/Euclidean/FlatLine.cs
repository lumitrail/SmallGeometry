using SmallGeometry.Exceptions;
using SmallGeometry.Primitives;

namespace SmallGeometry.Euclidean
{
    /// <summary>
    /// Immutable line of FlatPoints.
    /// </summary>
    public class FlatLine : IEnumerable<FlatPoint>, Interfaces.ISridCoordinate
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
        public FlatPoint this[int idx] => Points[idx];

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
        private IReadOnlyList<FlatPoint> Points { get; }
        private double _length = -1;


        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <exception cref="ArgumentException">points is empty</exception>
        /// <exception cref="ArgumentNullException">points is null</exception>
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
                Points = [points.First(), points.First()];
            }
            else
            {
                var coordinateSystems = points.Select(p => p.CoordinateSystem).Distinct().ToList();
                if (coordinateSystems.Count > 2)
                {
                    throw new CoordinateSystemDiscordanceException(coordinateSystems);
                }

                Points = points.Select(p => p).ToList();
            }

            CoordinateSystem = points.First().CoordinateSystem;
            BoundingBox = new FlatBoundingBox(Points);
        }

        /// <inheritdoc cref="FlatLine.FlatLine(IEnumerable{FlatPoint})"/>
        /// <param name="points"></param>
        public FlatLine(params FlatPoint[] points)
            : this(points.AsEnumerable())
        {
        }

        /// <summary>
        /// Deep copy constructor
        /// </summary>
        /// <param name="b"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public FlatLine(FlatLine b)
        {
            ArgumentNullException.ThrowIfNull(b);

            Points = b.Points;
            CoordinateSystem = b.CoordinateSystem;
            BoundingBox = b.BoundingBox;
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
                var s = new FlatLineSegment(Points[i - 1], Points[i]);
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

            return new FlatLineSegment(Points[i], Points[i - 1]);
        }

        /// <summary>
        /// Get a copy of line with same points follwing previous point removed.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">line is null</exception>
        /// <exception cref="CoordinateSystemDiscordanceException"></exception>
        public static List<FlatPoint> RemoveDuplicatedPoints(IEnumerable<FlatPoint> line)
        {
            ArgumentNullException.ThrowIfNull(line);
            if (!line.Any())
            {
                return new List<FlatPoint>();
            }

            {
                var coordinateSystems = line.Select(p => p.CoordinateSystem).Distinct().ToList();

                if (coordinateSystems.Count > 2)
                {
                    throw new CoordinateSystemDiscordanceException(coordinateSystems);
                }
            }

            var result = new List<FlatPoint>(line.Count());

            foreach (FlatPoint fp in line)
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

            FlatPoint previousPoint = Points[0];
            foreach (var p in Points)
            {
                double distance = FlatPoint.GetDistance(p, previousPoint);

                if (distance > maxDistance)
                {
                    int divisons = (int)(distance / maxDistance);

                    Vector2D v = new Vector2D(previousPoint, p).GetNormalizedVector() * maxDistance;

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
                Vector2D lineDirection = GetLineSegment(kv.Key).GetVector();
                Vector2D nearestToTarget = new Vector2D(kv.Value, target);
                double trueWhenPositive = Vector2D.CrossProduct(nearestToTarget, lineDirection);

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

            if (startNearest.Key == endNearest.Key)
            {
                Vector2D vStartNearest = GetLineSegment(startNearest.Key).GetVector();
                Vector2D vEndNearest = GetLineSegment(endNearest.Key).GetVector();

                if (vStartNearest.Size <= vEndNearest.Size)
                {
                    if (includeStartEndPoint)
                    {
                        result.Add(startPoint);
                    }
                    result.Add(startNearest.Value);
                    result.Add(endNearest.Value);
                    if (includeStartEndPoint)
                    {
                        result.Add(endPoint);
                    }
                }
                else
                {
                    if (includeStartEndPoint)
                    {
                        result.Add(endPoint);
                    }
                    result.Add(endNearest.Value);
                    result.Add(startNearest.Value);
                    if (includeStartEndPoint)
                    {
                        result.Add(startPoint);
                    }
                }
            }
            else if (startNearest.Key > endNearest.Key) // reversed!
            {
                if (includeStartEndPoint)
                {
                    result.Add(endPoint);
                }
                for (int i=startNearest.Key; i>endNearest.Key; --i)
                {
                    result.Add(Points[i]);
                }
                if (includeStartEndPoint)
                {
                    result.Add(startPoint);
                }
            }
            else // straight!
            {
                if (includeStartEndPoint)
                {
                    result.Add(startPoint);
                }
                for (int i=startNearest.Key; i<=endNearest.Key; ++i)
                {
                    result.Add(Points[i]);
                }
                if (includeStartEndPoint)
                {
                    result.Add(endPoint);
                }
            }

            return new FlatLine(result);
        }
        #endregion


        #region Line and line
        /// <summary>
        /// Gets intersections of lines. O(N^2)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CoordinateSystemDiscordanceException"></exception>
        public static List<FlatPoint> GetIntersections(FlatLine a, FlatLine b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            CoordinateSystemDiscordanceException.ThrowWhenDifferent(a, b);

            // Max possible result.Count is (a.Count * b.Count), but...
            var result = new List<FlatPoint>(a.Count + b.Count);

            List<FlatLineSegment> aSegments = a.GetLineSegments();
            List<FlatLineSegment> bSegments = b.GetLineSegments();

            foreach (var aSeg in aSegments)
            {
                foreach (var bSeg in bSegments)
                {
                    FlatPoint? intersection = FlatLineSegment.FindIntersectingPointOrNull(aSeg, bSeg);

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



        /// <summary>
        /// Transform points into one coordinate system(To points.First()).
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">points is null</exception>
        /// <exception cref="CoordinateSystemNoneException">points contains both None and else</exception>
        /// <exception cref="TransformException"></exception>
        private static IReadOnlyList<FlatPoint> HomogenizeCoordinateSystem(IEnumerable<FlatPoint> points)
        {
            ArgumentNullException.ThrowIfNull(points);

            CoordinateSystem[] coordinateSystems = points.Select(p => p.CoordinateSystem).Distinct().ToArray();
            System.Diagnostics.Debug.Assert(coordinateSystems.Length != 0);

            if (coordinateSystems.Length == 1)
            {
                return points.ToArray();
            }
            if (coordinateSystems.Contains(CoordinateSystem.None))
            {
                throw new CoordinateSystemNoneException(nameof(points));
            }

            CoordinateSystem transformTo = points.First().CoordinateSystem;

            IReadOnlyList<FlatPoint> result = points
                .Select(p => p.Transform(transformTo))
                .ToList();

            return result;
        }
    }
}
