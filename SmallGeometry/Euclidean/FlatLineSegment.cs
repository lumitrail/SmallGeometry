using System.Diagnostics;

using SmallGeometry.Exceptions;

namespace SmallGeometry.Euclidean
{
    /// <summary>
    /// Finite line segment.
    /// </summary>
    public class FlatLineSegment : Interfaces.ISridCoordinate
    {
        /// <summary>
        /// Coordinate system of this segment.
        /// </summary>
        public CoordinateSystem CoordinateSystem { get; }
        /// <summary>
        /// Start coordinate of this segment.
        /// </summary>
        public FlatPoint Start => new FlatPoint(StartCoordinate, CoordinateSystem);
        /// <summary>
        /// End coordinate of this segment.
        /// </summary>
        public FlatPoint End => new FlatPoint(EndCoordinate, CoordinateSystem);


        private Primitives.Coordinate2D StartCoordinate { get; }
        private Primitives.Coordinate2D EndCoordinate { get; }


        /// <summary>
        /// A line segment starting at start, ending at end.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <exception cref="CoordinateSystemDiscordanceException"></exception>
        public FlatLineSegment(FlatPoint start, FlatPoint end)
        {
            CoordinateSystemDiscordanceException.ThrowWhenDifferent(start, end);
            CoordinateSystem = start.CoordinateSystem;

            StartCoordinate = start.Coordinate2D;
            EndCoordinate = end.Coordinate2D;
        }


        /// <summary>
        /// Gets a vector from Start to End.
        /// </summary>
        /// <returns></returns>
        public Vector GetVector()
        {
            return new Vector(Start, End);
        }

        /// <summary>
        /// Gets the nearest point on this to p.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        /// <exception cref="CoordinateSystemDiscordanceException"></exception>
        public FlatPoint GetNearestPointTo(FlatPoint p)
        {
            CoordinateSystemDiscordanceException.ThrowWhenDifferent(this, p);

            var startToP = new Vector(Start, p);
            var startToEnd = new Vector(Start, End);

            // Distance from Start to the perpendicular foot.
            double h = Vector.InnerProduct(startToP, startToEnd) / startToEnd.Size;

            if (h <= 0)
            {
                return Start;
            }
            else if (h < startToEnd.Size)
            {
                Vector startToH = startToEnd.GetNormalizedVector() * h;
                FlatPoint H = Start + startToH;
                return H;
            }
            else
            {
                return End;
            }
        }


        /// <summary>
        /// Finds intersection between a and b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CoordinateSystemDiscordanceException"></exception>
        public static FlatPoint? FindIntersectingPointOrNull(FlatLineSegment a, FlatLineSegment b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            CoordinateSystemDiscordanceException.ThrowWhenDifferent(a, b);

            Vector A = a.GetVector();
            Vector B = b.GetVector();

            if (Vector.IsParallel(A, B))
            {
                return null;
            }

            double k = CalculateK(a.Start, A, b.Start, B);

            if (k > 1 || k < 0)
            {
                return null;
            }
            else
            {
                return a.Start + (k * A);
            }
        }

        /// <summary>
        /// Finds intersection of two infinite line, if they don't intersects, returns null.
        /// </summary>
        /// <param name="aStart"></param>
        /// <param name="A"></param>
        /// <param name="bStart"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Line vector is zero</exception>
        /// <exception cref="CoordinateSystemDiscordanceException"></exception>
        public static FlatPoint? FindIntersectingPointOrNull(
            FlatPoint aStart, Vector A,
            FlatPoint bStart, Vector B)
        {
            if (Vector.IsParallel(A, B))
            {
                return null;
            }

            // intersection: aStart + k*aVector = bStart + l*bVector
            // finding k
            double k = CalculateK(aStart, A, bStart, B);

            return aStart + (k * A);
        }

        /// <inheritdoc cref="FindIntersectingPointOrNull(FlatPoint, Vector, FlatPoint, Vector)"/>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        public static FlatPoint? FindIntersectingPointOrNull(
            FlatPoint a1, FlatPoint a2,
            FlatPoint b1, FlatPoint b2)
        {
            Vector A = new Vector(a1, a2);
            Vector B = new Vector(b1, b2);

            return FindIntersectingPointOrNull(a1, A, b1, B);
        }


        /// <summary>
        /// Finding k
        /// </summary>
        /// <param name="aStart"></param>
        /// <param name="A"></param>
        /// <param name="bStart"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Vector A and B are parallel(including Zero vector condition).</exception>
        /// <exception cref="CoordinateSystemDiscordanceException"></exception>
        private static double CalculateK(FlatPoint aStart, Vector A, FlatPoint bStart, Vector B)
        {
            CoordinateSystemDiscordanceException.ThrowWhenDifferent(aStart, bStart);

            if (Vector.IsParallel(A, B))
            {
                throw new ArgumentException($"Vector A({nameof(A)} and B({nameof(B)}) are parallel.");
            }
            
            if (A.X != 0 && B.Y != 0)
            {
                double dx = aStart.X - bStart.X;
                double dy = aStart.Y - bStart.Y;

                return ((dx / B.X) - (dy / B.Y)) / ((A.Y / B.Y) - (A.X / B.X));
            }
            else if (B.X == 0) // line b is parallel to y axis.
            {
                // line a and b is not parallel => A.X is not 0
                Debug.Assert(A.X != 0);
                return (bStart.X - aStart.X) / A.X;
            }
            else // if (bVector.Y == 0) // line b is parallel to x axis.
            {
                // line a and b is not parallel => A.Y is not 0
                Debug.Assert(A.Y != 0);
                return (bStart.Y - aStart.Y) / A.Y;
            }
        }
    }
}
