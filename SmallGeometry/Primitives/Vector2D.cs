using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using SmallGeometry.Interfaces;

namespace SmallGeometry.Primitives
{
    /// <summary>
    /// Vector
    /// </summary>
    public readonly struct Vector2D : IPosition2D
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly double X { get; }
        /// <summary>
        /// 
        /// </summary>
        public readonly double Y { get; }
        /// <summary>
        /// Euclidean length.
        /// </summary>
        public readonly double Size { get; }


        /// <summary>
        /// Zero vector
        /// </summary>
        public static readonly Vector2D Zero = new Vector2D(0, 0);


        /// <summary>
        /// <inheritdoc cref="Vector2D"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
            Size = Math.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public Vector2D(IPosition2D from, IPosition2D to)
            : this(to.X - from.X, to.Y - from.Y)
        {
        }

        /// <summary>
        /// Get unit vector of heading.
        /// </summary>
        /// <param name="heading">360 degree clockwise, 0 = y+ axis</param>
        public Vector2D(double heading)
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
            Size = Math.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="source"></param>
        public Vector2D(Vector2D source)
            : this(source.X, source.Y)
        {
        }


        /// <summary>
        /// Tries to parse string like "(x, y)", "[x, y]"
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string vector, [NotNullWhen(true)] out Vector2D? result)
        {
            try
            {
                char[] trimChar = ['(', ')', '[', ']', ','];
                string[] xy = vector.Split(',');

                if (xy.Length == 2)
                {
                    if (double.TryParse(xy[0].Trim().Trim(trimChar), out double x)
                        && double.TryParse(xy[1].Trim().Trim(trimChar), out double y))
                    {
                        result = new Vector2D(x, y);
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
        public static bool operator ==(Vector2D a, Vector2D b)
        {
            return a.X == b.X
                && a.Y == b.Y;
        }

        /// <summary>
        /// Compares X and Y
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Vector2D a, Vector2D b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Positive sign
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Vector2D operator +(Vector2D a)
        {
            return a;
        }

        /// <summary>
        /// Negative sign
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Vector2D operator -(Vector2D a)
        {
            return new Vector2D(-a.X, -a.Y);
        }

        /// <summary>
        /// Elementwise sum
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector2D operator +(Vector2D a, Vector2D b)
        {
            return new Vector2D(a.X + b.X, a.Y + b.Y);
        }

        /// <summary>
        /// Elementwise subtraction
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector2D operator -(Vector2D a, Vector2D b)
        {
            return a + -b;
        }

        /// <summary>
        /// Scalar multiplelication
        /// </summary>
        /// <param name="a"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2D operator *(double a, Vector2D v)
        {
            return new Vector2D(a * v.X, a * v.Y);
        }

        /// <summary>
        /// Scalar multiplelication
        /// </summary>
        /// <param name="v"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Vector2D operator *(Vector2D v, double a)
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
        public static Vector2D operator /(Vector2D v, double a)
        {
            if (a == 0)
            {
                throw new DivideByZeroException();
            }

            return new Vector2D(v.X / a, v.Y / a);
        }


        /// <summary>
        /// Dot product
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double InnerProduct(Vector2D a, Vector2D b)
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
        public static double CrossProduct(Vector2D a, Vector2D b)
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
        /// <remarks>Zero vector results in true.</remarks>
        public static bool IsParallel(Vector2D a, Vector2D b, double toleranceInDegree)
        {
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
        /// <remarks>Zero vector results in true.</remarks>
        public static bool IsParallel(Vector2D a, Vector2D b)
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
        /// <remarks>Zero vector results in true.</remarks>
        public static bool IsOrthogonal(Vector2D a, Vector2D b, double toleranceInDegree)
        {
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
        /// <remarks>Zero vector results in true.</remarks>
        public static bool IsOrthogonal(Vector2D a, Vector2D b)
        {
            return IsOrthogonal(a, b, 0.001);
        }

        /// <summary>
        /// Gets smaller angle between two vectors.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>smaller angle between a and b</returns>
        /// <remarks>Zero vector results 0</remarks>
        public static double GetAngleDegree(Vector2D a, Vector2D b)
        {
            if (a == Zero || b == Zero)
            {
                return 0;
            }

            double cosRadian = InnerProduct(a, b) / a.Size / b.Size;
            double radian = Math.Acos(cosRadian);

            return UnitConverter.RadianToDegree(radian);
        }

        /// <inheritdoc cref="GetAngleDegree(Vector2D, Vector2D)"/>
        public readonly double GetAngleDegree(Vector2D b)
        {
            return GetAngleDegree(this, b);
        }

        /// <summary>
        /// Gets rotated vector by degree clockwise.
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public Vector2D GetRotatedVector(double degree)
        {
            double currentDegree = GetAngleDegree(new Vector2D(0, 1));
            return new Vector2D(currentDegree + degree);
        }

        /// <summary>
        /// True when Clockwise degree more or equal to 0 and less than 180.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Zero vector results in false.</remarks>
        public readonly bool IsRightwardDirected()
        {
            return X > 0
                || X == 0 && Y > 0;
        }

        /// <summary>
        /// Get a vector with size of 1.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Zero vector results in zero vector.</remarks>
        public readonly Vector2D GetNormalizedVector()
        {
            if (this == Zero)
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
        public readonly Vector2D GetRightwardFlippedVector()
        {
            if (IsRightwardDirected())
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
        public readonly double GetHeading()
        {
            var north = new Vector2D(0, 1);

            Vector2D norm = GetNormalizedVector();
            double northInnerProduct = InnerProduct(north, norm);
            Debug.Assert(northInnerProduct <= 1);
            Debug.Assert(northInnerProduct >= -1);

            double radian = Math.Acos(northInnerProduct);
            Debug.Assert(!double.IsNaN(radian));

            double degree = UnitConverter.RadianToDegree(radian);
            Debug.Assert(!double.IsNaN(degree));

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
        public readonly override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj is Vector2D b)
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
