namespace SmallGeometryTester
{
    static class VectorIsSameAssert
    {
        private static readonly SmallGeometryComparer VectorComparer = new SmallGeometryComparer
        {
            DegreeErrorTolerance = 0.01,
            LengthAbsoluteErrorTolerance = 0.001,
            LengthPercentErrorTolerance = 0.1
        };


        public static void IsSameAssert(Vector a, Vector b)
        {
            Assert.True(VectorComparer.IsSame(a, b));
        }
    }
}
