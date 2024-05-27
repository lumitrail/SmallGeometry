using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static SmallGeometryTester.RandomGenerator;

namespace SmallGeometryTester
{
    public class FlatLineSegmentIntersectionTest
    {
        private static readonly SmallGeometryComparer comparer = new SmallGeometryComparer(0.001, 0.01, 0.1);

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
    }
}
