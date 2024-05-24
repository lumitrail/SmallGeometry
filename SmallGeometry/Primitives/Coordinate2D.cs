//using System.Diagnostics.CodeAnalysis;

//using SmallGeometry.Interfaces;

//namespace SmallGeometry.Primitives
//{
//    /// <summary>
//    /// 2D coordinate primitive.
//    /// </summary>
//    internal readonly struct Coordinate2D : IPosition2D
//    {
//        public double X { get; }
//        public double Y { get; }

//        public Coordinate2D(double x, double y)
//        {
//            X = x;
//            Y = y;
//        }

//        public static bool operator ==(Coordinate2D a, Coordinate2D b)
//        {
//            return a.X == b.X
//                && a.Y == b.Y;
//        }

//        public static bool operator !=(Coordinate2D a, Coordinate2D b)
//        {
//            return !(a == b);
//        }

//        public static Coordinate2D operator +(Coordinate2D p, Vector2D v)
//        {
//            return new Coordinate2D(
//                p.X + v.X,
//                p.Y + v.Y);
//        }

//        public static Coordinate2D operator +(Vector2D v, Coordinate2D p)
//        {
//            return p + v;
//        }
        

//        public static Coordinate2D operator -(Coordinate2D p, Vector2D v)
//        {
//            return p + (-v);
//        }


//        public override bool Equals([NotNullWhen(true)] object? obj)
//        {
//            if (obj == null)
//            {
//                return false;
//            }
//            else if (obj is Coordinate2D b)
//            {
//                return this == b;
//            }
//            else
//            {
//                return false;
//            }
//        }

//        public override int GetHashCode()
//        {
//            return Crc32Wrapper.GetCrc32Hash(X, Y);
//        }
//    }
//}
