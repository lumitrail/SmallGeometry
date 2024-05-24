using System.Diagnostics.CodeAnalysis;

namespace SmallGeometry.Primitives
{
    /// <summary>
    /// Interval including Min and Max.
    /// </summary>
    internal readonly struct Interval2D
    {
        private static readonly Random Rng = new Random((int)DateTime.Now.Ticks);

        /// <summary>
        /// Min endpoint
        /// </summary>
        public readonly double Min { get; }
        /// <summary>
        /// Max endpoint
        /// </summary>
        public readonly double Max { get; }
        /// <summary>
        /// Length between Min and Max
        /// </summary>
        public readonly double Length => Max - Min;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="doubles"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public Interval2D(IEnumerable<double> doubles)
        {
            ArgumentNullException.ThrowIfNull(doubles);
            if (!doubles.Any())
            {
                throw new ArgumentException(ExceptionMessages.PointsCountZero, nameof(doubles));
            }

            Min = doubles.First();
            Max = doubles.First();

            foreach (double d in doubles)
            {
                Min = Math.Min(Min, d);
                Max = Math.Max(Max, d);
            }
        }

        /// <inheritdoc cref="Interval2D(IEnumerable{double})"/>
        /// <param name="doubles"></param>
        public Interval2D(params double[] doubles)
            : this(doubles.AsEnumerable())
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="b"></param>
        public Interval2D(Interval2D b)
        {
            Min = b.Min;
            Max = b.Max;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Interval2D a, Interval2D b)
        {
            return a.Min == b.Min
                && a.Max == b.Max;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Interval2D a, Interval2D b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Check if there is any point intersecting.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Intersects(Interval2D a, Interval2D b)
        {
            if (a.Max < b.Min)
            {
                return false;
            }
            else if (b.Max < a.Min)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Returns the smallest Interval containing a and b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Interval2D Union(Interval2D a, Interval2D b)
        {
            return new Interval2D(Math.Min(a.Min, b.Min), Math.Max(a.Max, b.Max));
        }

        /// <summary>
        /// Picks a random value between Min and Max.
        /// </summary>
        /// <returns></returns>
        public readonly double Random()
        {
            return Min + Length * Rng.NextDouble();
        }

        /// <summary>
        /// x is in [Min, Max]?
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public readonly bool Contains(double x)
        {
            return Min <= x
                && x <= Max;
        }

        /// <summary>
        /// <inheritdoc cref="Intersects(Interval2D, Interval2D)"/>
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public readonly bool Intersects(Interval2D b)
        {
            return Intersects(this, b);
        }

        /// <summary>
        /// Returns the smallest Interval containing this Interval and b 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public readonly Interval2D Union(Interval2D b)
        {
            return new Interval2D(Math.Min(Min, b.Min), Math.Max(Max, b.Max));
        }

        /// <summary>
        /// Checks equality.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public readonly override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj is Interval2D b)
            {
                return Min == b.Min
                    && Max == b.Max;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets crc32 hashcode.
        /// </summary>
        /// <returns></returns>
        public readonly override int GetHashCode()
        {
            return Crc32Wrapper.GetCrc32Hash(Min, Max);
        }

        /// <summary>
        /// Gets string output.
        /// </summary>
        /// <returns>"[Min,Max]"</returns>
        public readonly override string ToString()
        {
            return $"[{Min},{Max}]";
        }
    }
}
