using System.Diagnostics.CodeAnalysis;

using SmallGeometry.Interfaces;
using SmallGeometry.Exceptions;
using SmallGeometry.Primitives;

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
        /// <exception cref="NotSupportedException">Coordinate system must be flat.</exception>
        public FlatPoint(double x, double y, CoordinateSystem coordinateSystem)
            : this(x, y)
        {
            if (!CoordinateSystemUtil.IsCoordinateSystemFlat(coordinateSystem))
            {
                throw new NotSupportedException("Coordinate system must be flat.");
            }
            CoordinateSystem = coordinateSystem;
        }

        /// <summary>
        /// Copy constructor.
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
        /// <exception cref="NotSupportedException">Coordinate system must be flat.</exception>
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
        /// Points are not the including coordinate system.
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
        public static FlatPoint operator +(FlatPoint p, Vector2D v)
        {
            return new FlatPoint(p.X + v.X, p.Y + v.Y, p.CoordinateSystem);
        }

        /// <summary>
        /// Moving point with vector.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static FlatPoint operator -(FlatPoint p, Vector2D v)
        {
            return p + (-v);
        }


        /// <summary>
        /// Gets Euclidean distance.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="CoordinateSystemDiscordanceException"></exception>
        public static double GetDistance(FlatPoint a, FlatPoint b)
        {
            CoordinateSystemDiscordanceException.ThrowWhenDifferent(a, b);

            double diffX = a.X - b.X;
            double diffY = a.Y - b.Y;

            return Math.Sqrt((diffX * diffX) + (diffY * diffY));
        }

        /// <inheritdoc cref="Transformer.TransformToFlat(FlatPoint, CoordinateSystem)"/>
        public readonly FlatPoint Transform(CoordinateSystem targetCoordinateSystem)
        {
            return Transformer.TransformToFlat(this, targetCoordinateSystem);
        }

        /// <inheritdoc cref="Transformer.TransformToGeoPoint(FlatPoint)"/>
        public readonly Geographic.GeoPoint TransformToGeoPoint()
        {
            return Transformer.TransformToGeoPoint(this);
        }

        /// <inheritdoc cref=" GetDistance(FlatPoint, FlatPoint)"/>
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
        public readonly override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is FlatPoint b)
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
