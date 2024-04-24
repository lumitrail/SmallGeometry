using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmallGeometry.Exceptions;
using SmallGeometry.Geographic;

namespace SmallGeometry.Euclidean
{
    public class BoundingBox
    {
        public CoordinateSystem CoordinateSystem { get; }
        /// <summary>
        /// X-axis interval
        /// </summary>
        public Interval IntervalX { get; }
        /// <summary>
        /// Y-axis interval
        /// </summary>
        public Interval IntervalY { get; }
        public double Top => IntervalY.Max;
        public double Bottom => IntervalY.Min;
        public double Left => IntervalX.Min;
        public double Right => IntervalX.Max;


        public BoundingBox(double x1, double x2, double y1, double y2, CoordinateSystem coordinateSystem)
        {
            IntervalX = new Interval(x1, x2);
            IntervalY = new Interval(y1, y2);
            CoordinateSystem = coordinateSystem;
        }

        /// <summary>
        /// Smallest bounding box containing p1 and p2
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <exception cref="CoordinateSystemNoneException">source or target coordinate system is none</exception>
        /// <exception cref="ArgumentException">failed to get projection info</exception>
        /// <exception cref="TransformException">failed to transform</exception>
        public BoundingBox(FlatPoint p1, FlatPoint p2)
        {
            FlatPoint p2Trans = Transformer.TransformToFlat(p2, p1.CoordinateSystem);
            CoordinateSystem = p1.CoordinateSystem;
            IntervalX = new Interval(p1.X, p2Trans.X);
            IntervalY = new Interval(p1.Y, p2Trans.Y);
        }

        /// <summary>
        /// Smallest bounding box containing all the points.
        /// </summary>
        /// <param name="points"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CoordinateSystemNoneException">source or target coordinate system is none</exception>
        /// <exception cref="ArgumentException">failed to get projection info</exception>
        /// <exception cref="TransformException">failed to transform</exception>
        public BoundingBox(IEnumerable<FlatPoint> points)
        {
            if (points == null || points.Count() == 0)
            {
                throw new ArgumentNullException(nameof(points));
            }

            double xmin = points.First().X;
            double ymin = points.First().Y;
            double xmax = xmin;
            double ymax = ymin;

            CoordinateSystem = points.First().CoordinateSystem;

            foreach (FlatPoint p in points)
            {
                FlatPoint pTrans = Transformer.TransformToFlat(p, CoordinateSystem);
                xmin = Math.Min(xmin, pTrans.X);
                ymin = Math.Min(ymin, pTrans.Y);

                xmax = Math.Max(xmax, pTrans.X);
                ymax = Math.Max(ymax, pTrans.Y);
            }

            IntervalX = new Interval(xmin, xmax);
            IntervalY = new Interval(ymin, ymax);
        }


        public BoundingBox(GeoPoint p1, GeoPoint p2)
        {
            CoordinateSystem = CoordinateSystem.Epsg4326;
            IntervalX = new Interval(p1.Longitude, p2.Longitude);
            IntervalY = new Interval(p1.Latitude, p2.Latitude);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BoundingBox(IEnumerable<GeoPoint> points)
        {
            if (points == null || points.Count() == 0)
            {
                throw new ArgumentNullException(nameof(points));
            }

            double xmin = points.First().Longitude;
            double ymin = points.First().Latitude;
            double xmax = xmin;
            double ymax = ymin;

            CoordinateSystem = points.First().CoordinateSystem;

            foreach (GeoPoint p in points)
            {
                xmin = Math.Min(xmin, p.Longitude);
                ymin = Math.Min(ymin, p.Latitude);

                xmax = Math.Max(xmax, p.Longitude);
                ymax = Math.Max(ymax, p.Latitude);
            }

            IntervalX = new Interval(xmin, xmax);
            IntervalY = new Interval(ymin, ymax);
        }


        private BoundingBox(Interval intervalX, Interval intervalY, CoordinateSystem coordinateSystem)
        {
            IntervalX = intervalX;
            IntervalY = intervalY;
            CoordinateSystem = coordinateSystem;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="b"></param>
        public BoundingBox(BoundingBox b)
        {
            CoordinateSystem = b.CoordinateSystem;
            IntervalX = b.IntervalX;
            IntervalY = b.IntervalY;
        }


        public BoundingBox GetPaddedCopy(double xPadding, double yPadding)
        {
            FlatPoint min = new(this.Left - xPadding, this.Bottom - yPadding);
            FlatPoint max = new(this.Right + xPadding, this.Top + yPadding);

            return new BoundingBox(min, max);
        }


        /// <summary>
        /// Determine whether p is in this bounding box.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        /// <exception cref="CoordinateSystemNoneException">source or target coordinate system is none</exception>
        /// <exception cref="ArgumentException">failed to get projection info</exception>
        /// <exception cref="TransformException">failed to transform</exception>
        public bool Contains(FlatPoint p)
        {
            FlatPoint pTrans = Transformer.TransformToFlat(p, CoordinateSystem);
            return Contains(pTrans.X, pTrans.Y);
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
        public bool Intersects(BoundingBox b)
        {
            if (b == null)
            {
                throw new ArgumentNullException(nameof(b));
            }

            return IntervalX.Intersects(b.IntervalX)
                && IntervalY.Intersects(b.IntervalY);
        }

        /// <summary>
        /// Gets smallest bounding box containing this bounding box and b.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">coordinate system error</exception>
        public BoundingBox Union(BoundingBox b)
        {
            if (b == null)
            {
                throw new ArgumentNullException(nameof(b));
            }
            else if (b.CoordinateSystem != CoordinateSystem)
            {
                throw new ArgumentException();
            }
            else
            {
                return new BoundingBox(IntervalX.Union(b.IntervalX), IntervalY.Union(b.IntervalY), CoordinateSystem);
            }
        }


        public double Width()
        {
            return IntervalX.Length;
        }
        public double Height()
        {
            return IntervalY.Length;
        }
        public double Area()
        {
            return Width() * Height();
        }

        // return a string representation
        public override string ToString()
        {
            return $"[ X:{IntervalX}, Y:{IntervalY} ]";
        }
    }
}
