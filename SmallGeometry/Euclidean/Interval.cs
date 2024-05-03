﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallGeometry.Euclidean
{
    /// <summary>
    /// Interval
    /// </summary>
    public struct Interval
    {
        /// <summary>
        /// Min endpoint
        /// </summary>
        public double Min { get; }
        /// <summary>
        /// Max endpoint
        /// </summary>
        public double Max { get; }
        /// <summary>
        /// Length between Min and Max
        /// </summary>
        public double Length => Max - Min;


        /// <summary>
        /// <inheritdoc cref="Interval"/>
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        public Interval(double x1, double x2)
        {
            Min = Math.Min(x1, x2);
            Max = Math.Max(x1, x2);
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
        /// <inheritdoc cref="Intersects(Interval, Interval)"/>
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

        /// <summary>
        /// Checks equality.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj is Interval b)
            {
                return this.Min == b.Min
                    && this.Max == b.Max;
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
        public override int GetHashCode()
        {
            return Crc32Wrapper.GetCrc32Hash(Min, Max);
        }

        /// <summary>
        /// Gets string output.
        /// </summary>
        /// <returns>"[Min,Max]"</returns>
        public override string ToString()
        {
            return $"[{Min},{Max}]";
        }
    }
}