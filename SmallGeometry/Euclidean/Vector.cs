using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using SmallGeometry.Interfaces;

namespace SmallGeometry.Euclidean
{
    /// <summary>
    /// Vector on flat plane space.
    /// </summary>
    public struct Vector : IPosition2D
    {
        /// <summary>
        /// 
        /// </summary>
        public double X { get; }
        /// <summary>
        /// 
        /// </summary>
        public double Y { get; }
        /// <summary>
        /// Euclidean length.
        /// </summary>
        public double Size { get; }


        /// <summary>
        /// <inheritdoc cref="Vector"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector(double x, double y)
        {
            X = x;
            Y = y;
            Size = Math.Sqrt((x * x) + (y * y));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public Vector(FlatPoint from, FlatPoint to)
            : this(to.X - from.X, to.Y - from.Y)
        {
        }

        /// <summary>
        /// Get unit vector of heading.
        /// </summary>
        /// <param name="heading">360 degree clockwise, 0 = y+ axis</param>
        public Vector(double heading)
        {
            double headingInRange = heading;

            while (headingInRange < 0)
            {
                headingInRange += 360;
            }
            while (headingInRange >= 360)
            {
                headingInRange -= 360;
            }

            double headingRadian = UnitConverter.DegreeToRadian(headingInRange);

            double x = Math.Sin(headingRadian);
            double y = Math.Cos(headingRadian);

            X = x;
            Y = y;
            Size = Math.Sqrt((x * x) + (y * y));
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="source"></param>
        public Vector(Vector source)
            : this(source.X, source.Y)
        {
        }


        /// <summary>
        /// Tries to parse string like "(x, y)", "[x, y]"
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string vector, [NotNullWhen(true)] out Vector? result)
        {
            try
            {
                char[] trimChar = [ '(', ')', '[', ']', ',' ];
                string[] xy = vector.Split(',');

                if (xy.Length == 2)
                {
                    if (double.TryParse(xy[0].Trim().Trim(trimChar), out double x)
                        && double.TryParse(xy[1].Trim().Trim(trimChar), out double y))
                    {
                        result = new Vector(x, y);
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
        /// Compares X and Y
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Vector a, Vector b)
        {
            return (a.X == b.X)
                && (a.Y == b.Y);
        }

        /// <summary>
        /// Compares X and Y
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Vector a, Vector b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Positive sign
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Vector operator +(Vector a)
        {
            return a;
        }

        /// <summary>
        /// Negative sign
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Vector operator -(Vector a)
        {
            return new Vector(-a.X, -a.Y);
        }

        /// <summary>
        /// Elementwise sum
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }

        /// <summary>
        /// Elementwise subtraction
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector operator -(Vector a, Vector b)
        {
            return a + (-b);
        }

        /// <summary>
        /// Scalar multiplelication
        /// </summary>
        /// <param name="a"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector operator *(double a, Vector v)
        {
            return new Vector(a * v.X, a * v.Y);
        }

        /// <summary>
        /// Scalar multiplelication
        /// </summary>
        /// <param name="v"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Vector operator *(Vector v, double a)
        {
            return a * v;
        }

        /// <summary>
        /// Scalar division
        /// </summary>
        /// <param name="v"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        /// <exception cref="DivideByZeroException"></exception>
        public static Vector operator /(Vector v, double a)
        {
            return new Vector(v.X / a, v.Y / a);
        }


        /// <summary>
        /// Dot product
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double InnerProduct(Vector a, Vector b)
        {
            double term1 = a.X * b.X;
            double term2 = a.Y * b.Y;
            return term1 + term2;
        }

        /// <summary>
        /// Z of vector resulted in with cross product.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double CrossProduct(Vector a, Vector b)
        {
            double term1 = a.X * b.Y;
            double term2 = a.Y * b.X;
            return term1 - term2;
        }

        /// <summary>
        /// Is approximately parallel?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="toleranceInDegree"></param>
        /// <returns></returns>
        /// <remarks>Zero vector results in false.</remarks>
        public static bool IsParallel(Vector a, Vector b, double toleranceInDegree)
        {
            if (a.Size == 0 || b.Size == 0)
            {
                return false;
            }
            double cp = CrossProduct(a, b); // going 0 when parallel
            double sm = a.Size * b.Size;
            double sinAB = Math.Abs(cp / sm);

            double toleranceInRadian = UnitConverter.DegreeToRadian(Math.Abs(toleranceInDegree));

            return sinAB < Math.Sin(toleranceInRadian);
        }

        /// <summary>
        /// Is approximately parallel?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <remarks>Zero vector results in false.</remarks>
        public static bool IsParallel(Vector a, Vector b)
        {
            return IsParallel(a, b, 0.001);
        }

        /// <summary>
        /// Is approximately orthogonal?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="toleranceInDegree"></param>
        /// <returns></returns>
        /// <remarks>Zero vector results in false.</remarks>
        public static bool IsOrthogonal(Vector a, Vector b, double toleranceInDegree)
        {
            if (a.Size == 0 || b.Size == 0)
            {
                return false;
            }
            double ip = InnerProduct(a, b); // going 0 when orthogonal
            double sm = a.Size * b.Size;
            double cosAB = Math.Abs(ip / sm);

            double toleranceInRadian = UnitConverter.DegreeToRadian(Math.Abs(toleranceInDegree));

            return cosAB < Math.Sin(toleranceInRadian);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <remarks>Zero vector results in false.</remarks>
        public static bool IsOrthogonal(Vector a, Vector b)
        {
            return IsOrthogonal(a, b, 0.001);
        }


        /// <summary>
        /// True when Clockwise degree more or equal to 0 and less than 180.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Zero vector results in false.</remarks>
        public bool IsRightwardDirected()
        {
            return (X > 0)
                || (X == 0 && Y > 0);
        }

        /// <summary>
        /// Get a vector with size of 1.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Zero vector results in zero vector.</remarks>
        public Vector GetNormalizedVector()
        {
            if (Size == 0)
            {
                return this;
            }
            else
            {
                return this / Size;
            }
        }

        /// <summary>
        /// Get a vector with direction of rightward = true.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Zero vector results in zero vector.</remarks>
        public Vector GetRightwardFlippedVector()
        {
            if (this.IsRightwardDirected())
            {
                return this;
            }
            else
            {
                return -this;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>[0, 360)</returns>
        /// <remarks>Zero vector returns 0</remarks>
        public double GetHeading()
        {
            var north = new Vector(0, 1);

            Vector norm = this.GetNormalizedVector();
            double northInnerProduct = InnerProduct(north, norm);
            Debug.Assert(northInnerProduct <= 1);
            Debug.Assert(northInnerProduct >= -1);

            double radian = Math.Acos(northInnerProduct);
            Debug.Assert(radian != double.NaN);

            double degree = UnitConverter.RadianToDegree(radian);
            Debug.Assert(degree != double.NaN);

            if (X >= 0)
            {
                return degree;
            }
            else
            {
                return 360 - degree;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj is Vector b)
            {
                return this == b;
            }
            else
            {
                return false;
            }
        }


        /// <inheritdoc cref="IPosition2D.GetHashCode(double, double)"/>
        public override int GetHashCode()
        {
            return IPosition2D.GetHashCode(X, Y);
        }

        /// <inheritdoc cref="IPosition2D.ToString(double, double)"/>
        public override string ToString()
        {
            return IPosition2D.ToString(X, Y);
        }
    }
}
