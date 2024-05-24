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
        /// Finds intersection with b
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CoordinateSystemDiscordanceException"></exception>
        public FlatPoint? FindIntersectingPointOrNull(FlatLineSegment b)
        {
            ArgumentNullException.ThrowIfNull(b);
            CoordinateSystemDiscordanceException.ThrowWhenDifferent(this, b);

            Vector A = this.GetVector();
            Vector B = b.GetVector();

            if (Vector.IsParallel(A, B))
            {
                return null;
            }

            double k = CalculateK(Start, A, b.Start, B);

            if (k > 1 || k < 0)
            {
                return null;
            }
            else
            {
                return Start + (k * A);
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
        /// <exception cref="Exceptions.CoordinateSystemNoneException">when aStart is none xor bStart is none</exception>
        /// <exception cref="Exceptions.TransformException"></exception>
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


        /// <summary>
        /// Finding k
        /// </summary>
        /// <param name="aStart"></param>
        /// <param name="A"></param>
        /// <param name="bStart"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Vector A and B are parallel</exception>
        /// <exception cref="CoordinateSystemDiscordanceException"></exception>
        /// <exception cref="Exceptions.CoordinateSystemNoneException">when aStart is none xor bStart is none</exception>
        /// <exception cref="Exceptions.TransformException"></exception>
        private static double CalculateK(FlatPoint aStart, Vector A, FlatPoint bStart, Vector B)
        {
            CoordinateSystemDiscordanceException.ThrowWhenDifferent(aStart, bStart);

            if (Vector.IsParallel(A, B))
            {
                throw new ArgumentException($"Vector A({nameof(A)} and B({nameof(B)}) are parallel.");
            }

            FlatPoint bStartTrans = bStart.Transform(aStart.CoordinateSystem);
            
            if (A.X != 0 && B.Y != 0)
            {
                double dx = aStart.X - bStartTrans.X;
                double dy = aStart.Y - bStartTrans.Y;

                return ((dx / B.X) - (dy / B.Y)) / ((A.Y / B.Y) - (A.X / B.X));
            }
            else if (B.X == 0) // line b is parallel to y axis.
            {
                // line a and b is not parallel => A.X is not 0
                Debug.Assert(A.X != 0);
                return (bStartTrans.X - aStart.X) / A.X;
            }
            else // if (bVector.Y == 0) // line b is parallel to x axis.
            {
                // line a and b is not parallel => A.Y is not 0
                Debug.Assert(A.Y != 0);
                return (bStartTrans.Y - aStart.Y) / A.Y;
            }
        }
    }
}
