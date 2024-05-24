namespace SmallGeometry.Primitives
{
    /// <summary>
    /// Interval including Min and Max.
    /// </summary>
    internal readonly struct Interval
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
        public Interval(IEnumerable<double> doubles)
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

        /// <inheritdoc cref="Interval(IEnumerable{double})"/>
        /// <param name="doubles"></param>
        public Interval(params double[] doubles)
            : this(doubles.AsEnumerable())
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="b"></param>
        public Interval(Interval b)
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
        public static bool operator ==(Interval a, Interval b)
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
        public static bool operator !=(Interval a, Interval b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Check if there is any point intersecting.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Intersects(Interval a, Interval b)
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
        public static Interval Union(Interval a, Interval b)
        {
            return new Interval(Math.Min(a.Min, b.Min), Math.Max(a.Max, b.Max));
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
        /// <inheritdoc cref="Intersects(Interval, Interval)"/>
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public readonly bool Intersects(Interval b)
        {
            return Intersects(this, b);
        }

        /// <summary>
        /// Returns the smallest Interval containing this Interval and b 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public readonly Interval Union(Interval b)
        {
            return new Interval(Math.Min(Min, b.Min), Math.Max(Max, b.Max));
        }

        /// <summary>
        /// Checks equality.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public readonly override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj is Interval b)
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
