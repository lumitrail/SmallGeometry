using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallGeometry.Euclidean
{
    /// <summary>
    /// Finite line segment.
    /// </summary>
    internal class FlatLineSegment
    {
        public CoordinateSystem CoordinateSystem => Start.CoordinateSystem;
        public FlatPoint Start { get; }
        public FlatPoint End { get; }


        /// <summary>
        /// A line segment starting at start, ending at end.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <exception cref="Exceptions.CoordinateSystemNoneException"></exception>
        /// <exception cref="Exceptions.TransformException"></exception>
        public FlatLineSegment(FlatPoint start, FlatPoint end)
        {
            Start = start;
            End = Transformer.TransformToFlat(end, start.CoordinateSystem);
        }




        /// <summary>
        /// Gets the nearest point on this to p.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public FlatPoint GetNearestPointTo(FlatPoint p)
        {
            FlatPoint pTrans = Transformer.TransformToFlat(p, CoordinateSystem);

            var startToP = new Vector(Start, pTrans);
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



        protected static FlatPoint? IntersectsInfiniteOrNull(FlatPoint aStart, Vector aVector, FlatPoint bStart, Vector bVector)
        {
            FlatPoint bStartTrans = bStart.Transform(aStart.CoordinateSystem);


        }
    }
}
