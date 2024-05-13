using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

        private List<GeoPoint> _points { get; }

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
        /// <inheritdoc cref="GeoLine"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public GeoLine(GeoPoint a, GeoPoint b)
        {
            _points = [a, b];
        }

        /// <summary>
        /// <inheritdoc cref="GeoLine"/>
        /// </summary>
        /// <param name="points"></param>
        /// <exception cref="ArgumentException">points is empty</exception>
        /// <exception cref="ArgumentNullException">points is null</exception>
        public GeoLine(params GeoPoint[] points)
        {
            ArgumentNullException.ThrowIfNull(points);

            int pointCount = points.Count();

            if (pointCount == 0)
            {
                throw new ArgumentException(ExceptionMessages.PointsCountZero, nameof(points));
            }
            else
            {
#warning 구현 지점
            }
        }

        /// <summary>
        /// Parses polyline
        /// </summary>
        /// <remarks>
        /// See https://developers.google.com/maps/documentation/utilities/polylinealgorithm
        /// </remarks>
        /// <param name="googlePolyline5"></param>
        public GeoLine(string googlePolyline5)
        {
#warning 구현 지점
            _points = [];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="geoPoints"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string GetGooglePolyline5(IEnumerable<GeoPoint> geoPoints)
        {
            return GooglePolyline5Codec.Encode(geoPoints);
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
        /// Gets a copy lf array.
        /// </summary>
        /// <returns></returns>
        public GeoPoint[] ToArray()
        {
            return _points.ToArray();
        }
    }
}
