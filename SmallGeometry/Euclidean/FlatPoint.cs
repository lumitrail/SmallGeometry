using System.Diagnostics.CodeAnalysis;

using SmallGeometry.Interfaces;

namespace SmallGeometry.Euclidean
{
    /// <summary>
    /// Euclidean flat-plane point.
    /// </summary>
    public readonly struct FlatPoint : IPosition2D, ISridCoordinate
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly CoordinateSystem CoordinateSystem { get; }
        /// <summary>
        /// 
        /// </summary>
        public readonly double X { get; }
        /// <summary>
        /// 
        /// </summary>
        public readonly double Y { get; }


        /// <summary>
        /// Without coordinate system info.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public FlatPoint(double x, double y)
        {
            X = x;
            Y = y;
            CoordinateSystem = CoordinateSystem.None;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="coordinateSystem"></param>
        /// <exception cref="ArgumentException">coordinateSystem is invalid</exception>
        public FlatPoint(double x, double y, CoordinateSystem coordinateSystem)
            : this(x, y)
        {
            if (!CoordinateSystemUtil.IsCoordinateSystemFlat(coordinateSystem))
            {
                throw new ArgumentException("coordinateSystem must be flat.");
            }
            CoordinateSystem = coordinateSystem;
        }

        /// <summary>
        /// Copy
        /// </summary>
        /// <param name="a"></param>
        public FlatPoint(FlatPoint a)
            : this(a.X, a.Y, a.CoordinateSystem)
        {
        }


        /// <summary>
        /// Tries to parse string like "(x, y)", "[x, y]"
        /// </summary>
        /// <param name="point"></param>
        /// <param name="coordinateSystem"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string point, CoordinateSystem coordinateSystem, [NotNullWhen(true)] out FlatPoint? result)
        {
            try
            {
                char[] trimChar = [ '(', ')', '[', ']', ',' ];
                string[] xy = point.Split(',');

                if (xy.Length == 2)
                {
                    if (double.TryParse(xy[0].Trim().Trim(trimChar), out double x)
                        && double.TryParse(xy[1].Trim().Trim(trimChar), out double y))
                    {
                        result = new FlatPoint(x, y, coordinateSystem);
                        return true;
                    }
                }
            }
            catch
            {
            }

            result = null;
            return false;
        }


        /// <summary>
        /// Points are exactly the same including coordinate system.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <remarks>comparison without transformation</remarks>
        public static bool operator ==(FlatPoint a, FlatPoint b)
        {
            return (a.CoordinateSystem == b.CoordinateSystem)
                && (a.X == b.X)
                && (a.Y == b.Y);
        }

        /// <summary>
        /// Points are not the same.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <remarks>comparison without transformation</remarks>
        public static bool operator !=(FlatPoint a, FlatPoint b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Moving point with vector.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static FlatPoint operator +(FlatPoint p, Vector v)
        {
            return new FlatPoint(p.X + v.X, p.Y + v.Y, p.CoordinateSystem);
        }

        /// <summary>
        /// Moving point with vector.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static FlatPoint operator -(FlatPoint p, Vector v)
        {
            return p + (-v);
        }


        /// <summary>
        /// Gets Euclidean distance.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="Exceptions.CoordinateSystemNoneException">source or target coordinate system is none</exception>
        /// <exception cref="ArgumentException">failed to get projection info</exception>
        /// <exception cref="Exceptions.TransformException">failed to transform</exception>
        public static double GetDistance(FlatPoint a, FlatPoint b)
        {
            double diffX;
            double diffY;
            if (a.CoordinateSystem == b.CoordinateSystem)
            {
                diffX = a.X - b.X;
                diffY = a.Y - b.Y;
            }
            else
            {
                FlatPoint bTrans = Transformer.TransformToFlat(b, a.CoordinateSystem);
                diffX = a.X - bTrans.X;
                diffY = a.Y - bTrans.Y;
            }

            return Math.Sqrt((diffX * diffX) + (diffY * diffY));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetCoordinateSystem"></param>
        /// <returns></returns>
        /// <exception cref="Exceptions.CoordinateSystemNoneException"></exception>
        /// <exception cref="Exceptions.TransformException"></exception>
        public readonly FlatPoint Transform(CoordinateSystem targetCoordinateSystem)
        {
            return Transformer.TransformToFlat(this, targetCoordinateSystem);
        }

        /// <summary>
        /// Transforms to GeoPoint
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exceptions.CoordinateSystemNoneException">source or target coordinate system is none</exception>
        /// <exception cref="Exceptions.TransformException">failed to transform</exception>
        public readonly Geographic.GeoPoint TransformToGeoPoint()
        {
            return Transformer.TransformToGeoPoint(this);
        }

        /// <summary>
        /// Gets Euclidean distance.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="Exceptions.CoordinateSystemNoneException">source or target coordinate system is none</exception>
        /// <exception cref="ArgumentException">failed to get projection info</exception>
        /// <exception cref="Exceptions.TransformException">failed to transform</exception>
        public readonly double GetDistance(FlatPoint b)
        {
            return GetDistance(this, b);
        }

        /// <summary>
        /// Gets wkt string.
        /// </summary>
        /// <returns>"ST_Point(X,Y,srid)" or "ST_Point(X,Y)"</returns>
        /// <remarks>Postgis 3.2.0 or above</remarks>
        public readonly string GetWkt()
        {
            if (CoordinateSystem != CoordinateSystem.None)
            {
                return $"ST_Point({X},{Y},{(int)CoordinateSystem})";
            }
            else
            {
                return $"ST_Point({X},{Y})";
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public readonly override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj is FlatPoint b)
            {
                return this == b;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc cref="IPosition2D.GetHashCode(double, double)"/>
        public readonly override int GetHashCode()
        {
            return IPosition2D.GetHashCode(X, Y);
        }

        /// <inheritdoc cref="IPosition2D.ToString(double, double)"/>
        public readonly override string ToString()
        {
            return IPosition2D.ToString(X, Y);
        }
    }
}
