using System.Text.Json.Serialization;

using SmallGeometry.Exceptions;
using SmallGeometry.Interfaces;
using SmallGeometry.Primitives;

namespace SmallGeometry.Geographic
{
    /// <summary>
    /// Bounding box for ellipsoidal coordinate system.
    /// </summary>
    public class GeoBoundingBox : ISridCoordinate, IBoundingBox2D
    {
        /// <summary>
        /// 
        /// </summary>
        public CoordinateSystem CoordinateSystem => CoordinateSystem.Epsg4326;
        /// <summary>
        /// 
        /// </summary>
        public double Top => IntervalY.Max;
        /// <summary>
        /// 
        /// </summary>
        public double Bottom => IntervalY.Min;
        /// <summary>
        /// 
        /// </summary>
        public double Left => IntervalX.Min;
        /// <summary>
        /// 
        /// </summary>
        public double Right => IntervalX.Max;


        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public double Width => IntervalX.Length;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public double Height => IntervalY.Length;


        /// <summary>
        /// X-axis interval
        /// </summary>
        private Interval2D IntervalX { get; }
        /// <summary>
        /// Y-axis interval
        /// </summary>
        private Interval2D IntervalY { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="longitude1"></param>
        /// <param name="longitude2"></param>
        /// <param name="latitude1"></param>
        /// <param name="latitude2"></param>
        public GeoBoundingBox(double longitude1, double longitude2, double latitude1, double latitude2)
        {
            IntervalX = new Interval2D(longitude1, longitude2);
            IntervalY = new Interval2D(latitude1, latitude2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <exception cref="ArgumentException">points is empty</exception>
        /// <exception cref="ArgumentNullException">points is null</exception>
        public GeoBoundingBox(IEnumerable<GeoPoint> points)
        {
            ArgumentNullException.ThrowIfNull(points);
            if (!points.Any())
            {
                throw new ArgumentException(ExceptionMessages.PointsCountZero, nameof(points));
            }

            double xmin = points.First().Longitude;
            double ymin = points.First().Latitude;
            double xmax = xmin;
            double ymax = ymin;

            foreach (GeoPoint p in points)
            {
                xmin = Math.Min(xmin, p.Longitude);
                ymin = Math.Min(ymin, p.Latitude);

                xmax = Math.Max(xmax, p.Longitude);
                ymax = Math.Max(ymax, p.Latitude);
            }

            IntervalX = new Interval2D(xmin, xmax);
            IntervalY = new Interval2D(ymin, ymax);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public GeoBoundingBox(params GeoPoint[] points)
            : this(points.AsEnumerable())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intervalX"></param>
        /// <param name="intervalY"></param>
        private GeoBoundingBox(Interval2D intervalX, Interval2D intervalY)
        {
            IntervalX = intervalX;
            IntervalY = intervalY;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="b"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public GeoBoundingBox(GeoBoundingBox b)
        {
            ArgumentNullException.ThrowIfNull(b);
            IntervalX = b.IntervalX;
            IntervalY = b.IntervalY;
        }

        /// <summary>
        /// Pads xPadding left and right, yPadding top and bottom.
        /// </summary>
        /// <param name="xPadding"></param>
        /// <param name="yPadding"></param>
        /// <returns></returns>
        public GeoBoundingBox GetPaddedCopy(double xPadding, double yPadding)
        {
            var intervalX = new Interval2D(Right + xPadding, Left - xPadding);
            var intervalY = new Interval2D(Top + yPadding, Bottom - yPadding);

            return new GeoBoundingBox(intervalX, intervalY);
        }

        /// <summary>
        /// Picks a random point inside the bounding box.
        /// </summary>
        /// <returns></returns>
        public GeoPoint PickRandomPoint()
        {
            return new GeoPoint(IntervalX.Random(), IntervalY.Random());
        }

        /// <summary>
        /// Determine whether p is in this bounding box.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Contains(GeoPoint p)
        {
            return Contains(p.Longitude, p.Latitude);
        }

        /// <summary>
        /// Determine whether (x,y) is in this bounding box.
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public bool Contains(double longitude, double latitude)
        {
            return IntervalX.Contains(longitude)
                && IntervalY.Contains(latitude);
        }

        /// <summary>
        /// Determine whether b intersects this bounding box.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Intersects(GeoBoundingBox b)
        {
            ArgumentNullException.ThrowIfNull(b);

            return IntervalX.Intersects(b.IntervalX)
                && IntervalY.Intersects(b.IntervalY);
        }

        /// <summary>
        /// Transform to flat BoundingBox.
        /// </summary>
        /// <param name="targetCoordinateSystem"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">targetCoordinateSystem is not flat</exception>
        /// <exception cref="TransformException"></exception>
        public Euclidean.FlatBoundingBox TransformToFlatBoundingBox(CoordinateSystem targetCoordinateSystem)
        {
            if (!CoordinateSystemUtil.IsCoordinateSystemFlat(targetCoordinateSystem))
            {
                throw new NotSupportedException(ExceptionMessages.CoordinateSystemMustBeFlat + targetCoordinateSystem);
            }
            else if (targetCoordinateSystem == CoordinateSystem.None)
            {
                return new Euclidean.FlatBoundingBox(Left, Right, Top, Bottom, CoordinateSystem.None);
            }
            else
            {
                Euclidean.FlatPoint min = GetBottomLeft().Transform(targetCoordinateSystem);
                Euclidean.FlatPoint max = GetTopRight().Transform(targetCoordinateSystem);

                return new Euclidean.FlatBoundingBox(min, max);
            }
        }

        /// <summary>
        /// Gets smallest bounding box containing this bounding box and b.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public GeoBoundingBox Union(GeoBoundingBox b)
        {
            ArgumentNullException.ThrowIfNull(b);

            return new GeoBoundingBox(IntervalX.Union(b.IntervalX), IntervalY.Union(b.IntervalY));
        }

        /// <summary>
        /// Min longitude, latitude
        /// </summary>
        /// <returns></returns>
        public GeoPoint GetBottomLeft()
        {
            return new GeoPoint(Left, Bottom);
        }

        /// <summary>
        /// Max longitude, latitude
        /// </summary>
        /// <returns></returns>
        public GeoPoint GetTopRight()
        {
            return new GeoPoint(Right, Top);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj is GeoBoundingBox b)
            {
                return Top == b.Top
                    && Bottom == b.Bottom
                    && Left == b.Left
                    && Right == b.Right;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Crc32Wrapper.GetCrc32Hash([Top, Bottom, Left, Right]);
        }

        /// <summary>
        /// Returns a string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[ X:{IntervalX}, Y:{IntervalY}]";
        }
    }
}
