using SmallGeometry.Primitives;

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
            Vector2D v1 = new Vector2D(x, y);
            Vector2D v2 = new Vector2D(x, y);

            Assert.True(v1.GetHashCode() == v1.GetHashCode());
            Assert.True(v1.GetHashCode() == v2.GetHashCode());
        }

        [Fact]
        public void Different()
        {
            Vector2D v1 = new Vector2D(RNG.NextDouble(), RNG.NextDouble());

            double degree = RNG.NextDouble() * 360;
            while (degree == 0)
            {
                degree += RNG.NextDouble() * 360;
            }

            Vector2D v2 = v1.GetRotatedVector(degree);

            Assert.False(v1.GetHashCode() == v2.GetHashCode());
        }
    }
}
