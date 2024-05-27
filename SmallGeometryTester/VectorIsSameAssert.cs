using SmallGeometry.Primitives;

namespace SmallGeometryTester
{
    static class VectorIsSameAssert
    {
        private static readonly SmallGeometryComparer VectorComparer = new SmallGeometryComparer(0.01, 0.001, 0.1);


        public static void IsSameAssert(Vector2D a, Vector2D b)
        {
            Assert.True(VectorComparer.IsSame(a, b));
        }
    }
}
