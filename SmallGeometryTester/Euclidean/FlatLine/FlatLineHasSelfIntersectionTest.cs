using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallGeometryTester
{
    public class FlatLineHasSelfIntersectionTest
    {
        private readonly Random RNG = new Random((int)DateTime.Now.Ticks);

        [Fact]
        public void NoIntersection()
        {
            var points = new List<FlatPoint>()
            {
                new FlatPoint(
                    RNG.NextDouble(),
                    RNG.NextDouble(),
                    CoordinateSystem.Epsg5179)
            };

            for (int i = 0; i < 10; ++i)
            {
                var vector = new Vector2D(RNG.NextDouble(), RNG.NextDouble());
                points.Add(points.Last() + vector);
            }

            var line = new FlatLine(points);
            bool hasSelfIntersection = line.HasSelfIntersection();

            Assert.False(hasSelfIntersection);
        }
    }
}
