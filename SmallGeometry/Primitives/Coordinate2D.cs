namespace SmallGeometry.Primitives
{
    internal struct Coordinate2D : Interfaces.IPosition2D
    {
        public double X { get; }
        public double Y { get; }

        public Coordinate2D(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
