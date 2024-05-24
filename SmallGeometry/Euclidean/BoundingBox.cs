using System.Text.Json.Serialization;

using SmallGeometry.Exceptions;
using SmallGeometry.Interfaces;
using SmallGeometry.Primitives;

namespace SmallGeometry.Euclidean
{
    /// <summary>
    /// Bounding box for flat coordinate system.
    /// </summary>
    public class BoundingBox : ISridCoordinate, IBoundingBox2D
    {
        /// <summary>
        /// 
        /// </summary>
        public CoordinateSystem CoordinateSystem { get; }
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
        private Interval IntervalX { get; }
        /// <summary>
        /// Y-axis interval
        /// </summary>
        private Interval IntervalY { get; }



        /// <summary>
        /// Smallest bounding box containing all the points.
        /// </summary>
        /// <param name="points"></param>
        /// <exception cref="ArgumentException">points is empty</exception>
        /// <exception cref="ArgumentNullException">points is null</exception>
        /// <exception cref="CoordinateSystemDiscordanceException"></exception>
        public BoundingBox(IEnumerable<FlatPoint> points)
        {
            ArgumentNullException.ThrowIfNull(points);
            if (!points.Any())
            {
                throw new ArgumentException(ExceptionMessages.PointsCountZero, nameof(points));
            }

            {
                IEnumerable<CoordinateSystem> coordinateSystems = points.Select(p => CoordinateSystem).Distinct();
                if (coordinateSystems.Count() > 1)
                {
                    throw new CoordinateSystemDiscordanceException(coordinateSystems);
                }
            }

            double xmin = points.First().X;
            double ymin = points.First().Y;
            double xmax = xmin;
            double ymax = ymin;

            CoordinateSystem = points.First().CoordinateSystem;

            IntervalX = new Interval(xmin, xmax);
            IntervalY = new Interval(ymin, ymax);
        }

        /// <inheritdoc cref="BoundingBox.BoundingBox(IEnumerable{FlatPoint})"/>
        /// <param name="points"></param>
        public BoundingBox(params FlatPoint[] points)
            : this(points.AsEnumerable())
        {
        }

        /// <summary>
        /// <inheritdoc cref="BoundingBox.BoundingBox(FlatPoint[])"/>
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="coordinateSystem">must be flat</param>
        /// <exception cref="NotSupportedException">coordinateSystem must be flat</exception>
        public BoundingBox(double x1, double x2, double y1, double y2, CoordinateSystem coordinateSystem)
            : this(new FlatPoint(x1, y1, coordinateSystem), new FlatPoint(x2, y2, coordinateSystem))
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intervalX"></param>
        /// <param name="intervalY"></param>
        /// <param name="coordinateSystem"></param>
        /// <exception cref="NotSupportedException"></exception>
        internal BoundingBox(Interval intervalX, Interval intervalY, CoordinateSystem coordinateSystem)
        {
            if (!CoordinateSystemUtil.IsCoordinateSystemFlat(coordinateSystem))
            {
                throw new NotSupportedException(ExceptionMessages.CoordinateSystemMustBeFlat + coordinateSystem);
            }

            IntervalX = intervalX;
            IntervalY = intervalY;
            CoordinateSystem = coordinateSystem;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="b"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BoundingBox(BoundingBox b)
        {
            ArgumentNullException.ThrowIfNull(b);

            CoordinateSystem = b.CoordinateSystem;
            IntervalX = b.IntervalX;
            IntervalY = b.IntervalY;
        }



        /// <summary>
        /// Pads xPadding left and right, yPadding top and bottom.
        /// </summary>
        /// <param name="xPadding"></param>
        /// <param name="yPadding"></param>
        /// <returns></returns>
        public BoundingBox GetPaddedCopy(double xPadding, double yPadding)
        {
            var intervalX = new Interval(Right + xPadding, Left - xPadding);
            var intervalY = new Interval(Top + yPadding, Bottom - yPadding);

            return new BoundingBox(intervalX, intervalY, CoordinateSystem);
        }

        /// <summary>
        /// Picks a random point inside the bounding box.
        /// </summary>
        /// <returns></returns>
        public FlatPoint PickRandomPoint()
        {
            return new FlatPoint(IntervalX.Random(), IntervalY.Random(), CoordinateSystem);
        }

        /// <summary>
        /// Determines whether p is in this bounding box.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        /// <exception cref="CoordinateSystemDiscordanceException"></exception>
        public bool Contains(FlatPoint p)
        {
            CoordinateSystemDiscordanceException.ThrowWhenDifferent(this, p);
            return Contains(p.X, p.Y);
        }

        /// <summary>
        /// Determine whether (x,y) is in this bounding box.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Contains(double x, double y)
        {
            return IntervalX.Contains(x)
                && IntervalY.Contains(y);
        }

        /// <summary>
        /// Determine whether b intersects this bounding box.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CoordinateSystemDiscordanceException"></exception>
        public bool Intersects(BoundingBox b)
        {
            ArgumentNullException.ThrowIfNull(b);
            CoordinateSystemDiscordanceException.ThrowWhenDifferent(this, b);

            return IntervalX.Intersects(b.IntervalX)
                && IntervalY.Intersects(b.IntervalY);
        }

        /// <summary>
        /// Transform into flat bounding box.
        /// </summary>
        /// <param name="targetCoordinateSystem"></param>
        /// <returns></returns>
        /// <exception cref="CoordinateSystemNoneException"></exception>
        /// <exception cref="TransformException"></exception>
        public BoundingBox Transform(CoordinateSystem targetCoordinateSystem)
        {
            if (CoordinateSystem == targetCoordinateSystem)
            {
                return this;
            }
            else if (targetCoordinateSystem == CoordinateSystem.None)
            {
                throw new CoordinateSystemNoneException(nameof(targetCoordinateSystem));
            }
            else if (CoordinateSystemUtil.IsCoordinateSystemFlat(targetCoordinateSystem))
            {
                FlatPoint min = GetBottomLeft().Transform(targetCoordinateSystem);
                FlatPoint max = GetTopRight().Transform(targetCoordinateSystem);

                return new BoundingBox(min, max);
            }
            else
            {
                throw new TransformException(CoordinateSystem, targetCoordinateSystem);
            }
        }

        /// <summary>
        /// Transform into GeoBoundingBox.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CoordinateSystemNoneException"></exception>
        /// <exception cref="TransformException"></exception>
        public Geographic.GeoBoundingBox TransformToGeoBoundingBox()
        {
            Geographic.GeoPoint min = GetBottomLeft().TransformToGeoPoint();
            Geographic.GeoPoint max = GetTopRight().TransformToGeoPoint();

            return new Geographic.GeoBoundingBox(min, max);
        }

        /// <summary>
        /// Gets smallest bounding box containing this bounding box and b.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CoordinateSystemDiscordanceException"></exception>
        public BoundingBox Union(BoundingBox b)
        {
            ArgumentNullException.ThrowIfNull(b);
            CoordinateSystemDiscordanceException.ThrowWhenDifferent(this, b);
            
            return new BoundingBox(
                Interval.Union(this.IntervalX, b.IntervalX),
                Interval.Union(this.IntervalY, b.IntervalY),
                CoordinateSystem);
        }

        /// <summary>
        /// Min x, y
        /// </summary>
        /// <returns></returns>
        public FlatPoint GetBottomLeft()
        {
            return new FlatPoint(Left, Bottom, CoordinateSystem);
        }

        /// <summary>
        /// Max x, y
        /// </summary>
        /// <returns></returns>
        public FlatPoint GetTopRight()
        {
            return new FlatPoint(Right, Top, CoordinateSystem);
        }

        /// <summary>
        /// Returns a string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[ X:{IntervalX}, Y:{IntervalY} ]";
        }
    }
}
