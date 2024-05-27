using SmallGeometry.Primitives;

namespace SmallGeometryTester
{
    public class VectorEquals
    {
        Vector2D Vector1 => new Vector2D(1.05, -1.05);
        Vector2D Zero => new Vector2D(0, 0);

        [Fact]
        public void WithNull()
        {
            Assert.False(Vector1.Equals(null));
            Assert.False(Zero.Equals(null));
        }

        [Fact]
        public void WithOtherType()
        {
            Assert.False(Vector1.Equals('c'));
            Assert.False(Vector1.Equals(1));

            string hey = "hey";
            Assert.False(Vector1.Equals(hey));
            Assert.False(Vector1.Equals(new FlatPoint(0, 0)));
        }

        [Fact]
        public void WithSameVector()
        {
            Vector2D v1 = Vector1;
            Vector2D v2 = Vector1;

            Assert.True(v1.Equals(v1));
            Assert.True(v1.Equals(v2));
        }

        [Fact]
        public void WithDifferentVector()
        {
            Assert.False(Vector1.Equals(Zero));
        }
    }
}
