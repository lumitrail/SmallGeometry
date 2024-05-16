namespace SmallGeometryTester
{
    public class VectorGetHashCode
    {
        private static readonly Random RNG = new Random((int)DateTime.Now.Ticks);

        [Fact]
        public void Same()
        {
            double x = RNG.NextDouble();
            double y = RNG.NextDouble();
            Vector v1 = new Vector(x, y);
            Vector v2 = new Vector(x, y);

            Assert.True(v1.GetHashCode() == v1.GetHashCode());
            Assert.True(v1.GetHashCode() == v2.GetHashCode());
        }

        [Fact]
        public void Different()
        {
            Vector v1 = new Vector(RNG.NextDouble(), RNG.NextDouble());

            double degree = RNG.NextDouble() * 360;
            while (degree == 0)
            {
                degree += RNG.NextDouble() * 360;
            }

            Vector v2 = v1.GetRotatedVector(degree);

            Assert.False(v1.GetHashCode() == v2.GetHashCode());
        }
    }
}
