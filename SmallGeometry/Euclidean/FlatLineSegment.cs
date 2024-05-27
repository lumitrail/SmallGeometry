using System.Diagnostics;

using SmallGeometry.Exceptions;
using SmallGeometry.Primitives;

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
        public FlatPoint Start { get; }
        /// <summary>
        /// End coordinate of this segment.
        /// </summary>
        public FlatPoint End { get; }



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

            Start = start;
            End = end;
        }


        /// <summary>
        /// Gets a vector from Start to End.
        /// </summary>
        /// <returns></returns>
        public Vector2D GetVector()
        {
            return new Vector2D(Start, End);
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

            var startToP = new Vector2D(Start, p);
            var startToEnd = new Vector2D(Start, End);

            // Distance from Start to the perpendicular foot.
            double h = Vector2D.InnerProduct(startToP, startToEnd) / startToEnd.Size;

            if (h <= 0)
            {
                return Start;
            }
            else if (h < startToEnd.Size)
            {
                Vector2D startToH = startToEnd.GetNormalizedVector() * h;
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

            Vector2D A = a.GetVector();
            Vector2D B = b.GetVector();

            if (Vector2D.IsParallel(A, B))
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
            FlatPoint aStart, Vector2D A,
            FlatPoint bStart, Vector2D B)
        {
            if (Vector2D.IsParallel(A, B))
            {
                return null;
            }

            // intersection: aStart + k*aVector = bStart + l*bVector
            // finding k
            double k = CalculateK(aStart, A, bStart, B);

            return aStart + (k * A);
        }

        /// <inheritdoc cref="FindIntersectingPointOrNull(FlatPoint, Vector2D, FlatPoint, Vector2D)"/>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        public static FlatPoint? FindIntersectingPointOrNull(
            FlatPoint a1, FlatPoint a2,
            FlatPoint b1, FlatPoint b2)
        {
            Vector2D A = new Vector2D(a1, a2);
            Vector2D B = new Vector2D(b1, b2);

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
        private static double CalculateK(FlatPoint aStart, Vector2D A, FlatPoint bStart, Vector2D B)
        {
            CoordinateSystemDiscordanceException.ThrowWhenDifferent(aStart, bStart);

            if (Vector2D.IsParallel(A, B))
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
