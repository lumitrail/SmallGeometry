using System.Text.Json.Serialization;

namespace SmallGeometry.Geographic
{
    /// <summary>
    /// Immutable line of EPSG 4326(=WGS84) points
    /// </summary>
    public class GeoLine : IEnumerable<GeoPoint>
    {
        /// <summary>
        /// EPSG 4326(=WGS84)
        /// </summary>
        [JsonIgnore]
        public CoordinateSystem CoordinateSystem => CoordinateSystem.Epsg4326;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public GeoPoint this[int idx] => _points[idx];

        /// <summary>
        /// Number of points
        /// </summary>
        public int Count => _points.Count;

        /// <summary>
        /// 
        /// </summary>
        public GeoBoundingBox BoundingBox { get; }

        private IReadOnlyList<GeoPoint> _points { get; }
        private double _lengthInMeter = -1;


        /// <summary>
        /// <inheritdoc cref="GeoLine"/>
        /// </summary>
        /// <param name="points"></param>
        /// <exception cref="ArgumentException">points is null</exception>
        /// <exception cref="ArgumentNullException">points is null</exception>
        public GeoLine(IEnumerable<GeoPoint> points)
        {
            ArgumentNullException.ThrowIfNull(points);

            int pointCount = points.Count();
            if (pointCount == 0)
            {
                throw new ArgumentException(ExceptionMessages.PointsCountZero, nameof(points));
            }
            else if (pointCount == 1)
            {
                _points = [points.First(), points.First()];
            }
            else // pointsCount > 1
            {
                _points = points.ToArray();
            }

            BoundingBox = new GeoBoundingBox(_points);
        }

        /// <inheritdoc cref="GeoLine(IEnumerable{GeoPoint})"/>
        public GeoLine(params GeoPoint[] points)
            : this(points.AsEnumerable())
        {
        }

        /// <inheritdoc cref="GooglePolyline5Codec.Decode(string)"/>
        public GeoLine(string googlePolyline5)
            : this(GooglePolyline5Codec.Decode(googlePolyline5))
        {
        }


        /// <summary>
        /// Get a copy of line with same points follwing previous point removed.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">line is empty</exception>
        /// <exception cref="ArgumentNullException">line is null</exception>
        public static List<GeoPoint> RemoveDuplicatedPoints(IEnumerable<GeoPoint> line)
        {
            ArgumentNullException.ThrowIfNull(line);
            if (!line.Any())
            {
                throw new ArgumentException(ExceptionMessages.PointsCountZero, nameof(line));
            }

            var result = new List<GeoPoint>(line.Count())
            {
                line.First()
            };

            foreach (GeoPoint p in line)
            {
                if (p != result.Last())
                {
                    result.Add(p);
                }
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GeoLine GetReversedCopy()
        {
            var reversedPoints = new List<GeoPoint>(_points);
            reversedPoints.Reverse();
            return new GeoLine(reversedPoints);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double GetLengthInMeter()
        {
            if (_lengthInMeter < 0)
            {
                _lengthInMeter = 0;
                for (int i=1; i<Count; ++i)
                {
                    _lengthInMeter += _points[i - 1].GetDistanceInMeter(_points[i]);
                }
            }

            return _lengthInMeter;
        }

        /// <summary>
        /// <inheritdoc cref="GooglePolyline5Codec.Encode(IEnumerable{GeoPoint})"/>
        /// </summary>
        /// <returns></returns>
        public string GetGooglePolyline5()
        {
            return GooglePolyline5Codec.Encode(this);
        }

        /// <summary>
        /// [[x,y],[x,y],...,[x,y]]
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[{string.Join(',', _points.Select(p => p.ToString()).ToArray())}]";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<GeoPoint> GetEnumerator()
        {
            return _points.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _points.GetEnumerator();
        }

        /// <summary>
        /// Gets a copy of list.
        /// </summary>
        /// <returns></returns>
        public List<GeoPoint> ToList()
        {
            return new List<GeoPoint>(_points);
        }

        /// <summary>
        /// Gets a copy of array.
        /// </summary>
        /// <returns></returns>
        public GeoPoint[] ToArray()
        {
            return _points.ToArray();
        }
    }
}
