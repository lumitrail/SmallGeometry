using SmallGeometry.Exceptions;

namespace SmallGeometry.Euclidean
{
    /// <summary>
    /// Closed FlatLine Polygon, whose FlatPolygon.Last() == FlatPolygon.First();
    /// </summary>
    public class FlatPolygon
    {
        /// <summary>
        /// 
        /// </summary>
        public CoordinateSystem CoordinateSystem => Points[0].CoordinateSystem;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public FlatPoint this[int idx] => Points[idx];

        /// <summary>
        /// Number of points not including Last().
        /// </summary>
        public int Count => Points.Count -1;

        /// <summary>
        /// 
        /// </summary>
        public FlatBoundingBox BoundingBox { get; }

        /// <summary>
        /// 
        /// </summary>
        private IReadOnlyList<FlatPoint> Points { get; }
        private double _edgeLength = -1;
        private double _area = -1;


        /// <summary>
        /// <inheritdoc cref="FlatPolygon"/>
        /// </summary>
        /// <param name="flatPoints"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CoordinateSystemDiscordanceException"></exception>
        /// <exception cref="NotSupportedException">Polygon has self intersection.</exception>
        public FlatPolygon(IEnumerable<FlatPoint> flatPoints)
        {
            ArgumentNullException.ThrowIfNull(flatPoints);
            if (flatPoints.Count() < 3)
            {
                throw new ArgumentException(ExceptionMessages.PointsCountAtLeast(3));
            }

            var tempLine = new FlatLine(flatPoints);

            if (tempLine.HasSelfIntersection())
            {
                throw new NotSupportedException(ExceptionMessages.HasSelfIntersection);
            }

            var tempList = new List<FlatPoint>(flatPoints);
            if (tempList.First() != tempList.Last())
            {
                tempList.Add(tempList.First());
            }

            BoundingBox = new FlatBoundingBox(flatPoints);
            Points = tempList;
        }

        /// <inheritdoc cref="FlatPolygon.FlatPolygon(IEnumerable{FlatPoint})"/>
        public FlatPolygon(params FlatPoint[] flatPoints)
            : this(flatPoints.AsEnumerable())
        {
        }


        #region Polygon alone
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double GetEdgeLength()
        {
            if (_edgeLength < 0)
            {
                _edgeLength = 0;
                for (int i=1; i<Points.Count; ++i)
                {
                    _edgeLength += Points[i - 1].GetDistance(Points[i]);
                }
            }

            return _edgeLength;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double GetArea()
        {
            if (_area < 0)
            {
                _area = 0;
                for (int i=1; i<Points.Count; ++i)
                {
                    _area += (Points[i - 1].X * Points[i].Y) - (Points[i - 1].Y * Points[i].X);
                }

                _area = Math.Abs(_area / 2);
            }

            return _area;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FlatPoint GetCentroid()
        {
            GetArea();
            double cx = 0;
            double cy = 0;
            for (int i = 1; i < Points.Count; i++)
            {
                cx += (Points[i-1].X + Points[i].X) * (Points[i-1].Y * Points[i].X - Points[i-1].X * Points[i].Y);
                cy += (Points[i-1].Y + Points[i].Y) * (Points[i-1].Y * Points[i].X - Points[i-1].X * Points[i].Y);
            }
            cx /= (6 * _area);
            cy /= (6 * _area);
            return new FlatPoint(cx, cy, CoordinateSystem);
        }

        /// <summary>
        /// Picks a random point in this polygon.
        /// </summary>
        /// <returns></returns>
        public FlatPoint PickRandomPoint()
        {
            FlatPoint p = BoundingBox.PickRandomPoint();
            while (!this.Contains(p))
            {
                p = BoundingBox.PickRandomPoint();
            }
            return p;
        }
        #endregion

        #region Polygon and Point
        /// <summary>
        /// See if p is inside the polygon.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        /// <remarks>http://exaflop.org/docs/cgafaq/cga2.html</remarks>
        public bool Contains(FlatPoint p)
        {
            int crossings = 0;
            for (int i = 0; i < Count; i++)
            {
                double slope = (Points[i + 1].X - Points[i].X) / (Points[i + 1].Y - Points[i].Y);
                bool cond1 = (Points[i].Y <= p.Y) && (p.Y < Points[i + 1].Y);
                bool cond2 = (Points[i + 1].Y <= p.Y) && (p.Y < Points[i].Y);
                bool cond3 = p.X < slope * (p.Y - Points[i].Y) + Points[i].X;
                if ((cond1 || cond2) && cond3)
                {
                    crossings++;
                }
            }
            return (crossings % 2 != 0);
        }
        #endregion
    }
}
