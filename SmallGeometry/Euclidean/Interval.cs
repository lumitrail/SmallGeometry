using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallGeometry.Euclidean
{
    public class Interval
    {
        /// <summary>
        /// Min endpoint
        /// </summary>
        public double Min { get; }
        /// <summary>
        /// Max endpoint
        /// </summary>
        public double Max { get; }
        public double Length { get; }


        public Interval(double x1, double x2)
        {
            Min = Math.Min(x1, x2);
            Max = Math.Max(x1, x2);
            Length = Max - Min;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="b"></param>
        public Interval(Interval b)
        {
            Min = b.Min;
            Max = b.Max;
            Length = b.Length;
        }


        public static bool operator ==(Interval a, Interval b)
        {
            return a.Min == b.Min
                && a.Max == b.Max;
        }

        public static bool operator !=(Interval a, Interval b)
        {
            return !(a == b);
        }

        /// <summary>
        /// 
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
        /// x is in [Min, Max]?
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public bool Contains(double x)
        {
            return Min <= x
                && x <= Max;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool Intersects(Interval b)
        {
            return Intersects(this, b);
        }

        /// <summary>
        /// Returns the smallest Interval containing this Interval and b 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Interval Union(Interval b)
        {
            return new Interval(Math.Min(Min, b.Min), Math.Max(Max, b.Max));
        }


        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj is Interval b)
            {
                return this == b;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return (int)CRC32.Get(new double[] { Min, Max });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>"[Min,Max]"</returns>
        public override string ToString()
        {
            return $"[{Min},{Max}]";
        }
    }
}
