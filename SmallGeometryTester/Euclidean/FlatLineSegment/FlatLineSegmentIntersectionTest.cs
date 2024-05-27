using static SmallGeometryTester.RandomGenerator;

namespace SmallGeometryTester
{
    public class FlatLineSegmentIntersectionTest
    {
        private static readonly SmallGeometryComparer comparer = new SmallGeometryComparer(0.001, 0.001, 0.1);

        [Fact]
        public void ParallelCase()
        {
            Vector2D v = GetRandomNonzeroVector();

            FlatPoint a0 = GetRandomPoint(CoordinateSystem.Epsg5179);
            FlatPoint b0 = GetRandomPoint(CoordinateSystem.Epsg5179);

            var a = new FlatLineSegment(a0, a0 + v);
            var b = new FlatLineSegment(b0, b0 + v);

            Assert.False(FlatLineSegment.FindIntersectingPointOrNull(a, b).HasValue);
        }

        [Fact]
        public void CrossCase()
        {
            FlatPoint intersection = GetRandomPoint(CoordinateSystem.Epsg5179);

            Vector2D A = GetRandomNonzeroVector();
            Vector2D B = GetRandomNonzeroVector();
            while (Vector2D.IsParallel(A, B))
            {
                B = GetRandomNonzeroVector();
            }

            var a = new FlatLineSegment(intersection + A, intersection - A);
            var b = new FlatLineSegment(intersection + B, intersection - B);

            FlatPoint? intersectionCalculated = FlatLineSegment.FindIntersectingPointOrNull(a, b);

            Assert.True(intersectionCalculated.HasValue);
            Assert.True(comparer.IsSame(intersection, intersectionCalculated.Value));
        }

        [Fact]
        public void ParallelToAxisCase()
        {
            FlatPoint intersection = GetRandomPoint(CoordinateSystem.Epsg5179);
            Vector2D A = GetRandomNonzeroVector();
            Vector2D B = GetRandomNonzeroVector();
            while (Vector2D.IsParallel(A, B))
            {
                B = GetRandomNonzeroVector();
            }

            bool AorB = TossCoin();
            bool XorY = TossCoin();

            if (AorB)
            {
                if (XorY)
                {
                    A = new Vector2D(0, A.Y);
                }
                else
                {
                    A = new Vector2D(A.X, 0);
                }
            }
            else
            {
                if (XorY)
                {
                    B = new Vector2D(0, B.Y);
                }
                else
                {
                    B = new Vector2D(B.X, 0);
                }
            }

            if (A == Vector2D.Zero || B == Vector2D.Zero)
            {
                return;
            }

            var a = new FlatLineSegment(intersection + A, intersection - A);
            var b = new FlatLineSegment(intersection + B, intersection - B);

            FlatPoint? intersectionCalculated = FlatLineSegment.FindIntersectingPointOrNull(a, b);

            Assert.True(intersectionCalculated.HasValue);
            Assert.True(comparer.IsSame(intersection, intersectionCalculated.Value));
        }
    }
}
