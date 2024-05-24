using SmallGeometry.Primitives;
using static SmallGeometryTester.VectorIsSameAssert;

namespace SmallGeometryTester
{
    public class VectorConstructor
    {
        private readonly ITestOutputHelper Output;
        private static readonly Random Rnd = new Random((int)DateTime.Now.Ticks);


        public VectorConstructor(ITestOutputHelper outputHelper)
        {
            Output = outputHelper;
        }

        [Fact]
        public void WithXY()
        {
            double x = 3;
            double y = 4;
            double size = 5;
            Vector2D a = new Vector2D(3, 4);
            Vector2D b = new Vector2D(3, 4);

            Assert.True(a == b);
            Assert.True(a.X == x);
            Assert.True(a.Y == y);
            Assert.True(a.Size == size);
        }

        [Fact]
        public void WithPointFromTo()
        {
            FlatPoint pa = new FlatPoint(3, 4);
            FlatPoint pb = new FlatPoint(5, 6);

            double x = pb.X - pa.X;
            double y = pb.Y - pa.Y;

            Vector2D va = new Vector2D(pa, pb);
            Vector2D vaa = new Vector2D(x, y);

            Assert.True(va == vaa);
            Assert.True(va.X == x);
            Assert.True(va.Y == y);
        }

        [Fact]
        public void HeadingReduction()
        {
            Vector2D h1 = new Vector2D(25);
            Vector2D h2 = new Vector2D(25 + 360);
            Vector2D h3 = new Vector2D(25 - 360);
            VectorComparisonWithLog(h1, h2);
            VectorComparisonWithLog(h2, h3);

            Vector2D h4 = new Vector2D(-25);
            Vector2D h5 = new Vector2D(-25 + 360);
            Vector2D h6 = new Vector2D(-25 - 360);
            VectorComparisonWithLog(h4, h5);
            VectorComparisonWithLog(h5, h6);
        }

        [Fact]
        public void WithHeadingAndCoordinate()
        {
            Vector2D eastValue = new Vector2D(1, 0);
            Vector2D eastHeading = new Vector2D(90);
            VectorComparisonWithLog(eastValue, eastHeading);

            Vector2D westValue = new Vector2D(-1, 0);
            Vector2D westHeading = new Vector2D(270);
            VectorComparisonWithLog(westValue, westHeading);

            Vector2D northValue = new Vector2D(0, 1);
            Vector2D northHeading = new Vector2D(0);
            VectorComparisonWithLog(northValue, northHeading);

            Vector2D southValue = new Vector2D(0, -1);
            Vector2D southHeading = new Vector2D(180);
            VectorComparisonWithLog(southValue, southHeading);

            Vector2D sec1Value = new Vector2D(1, 1).GetNormalizedVector();
            Vector2D sec1Heading = new Vector2D(45);
            VectorComparisonWithLog(sec1Value, sec1Heading);

            Vector2D sec2Value = new Vector2D(-1, 1).GetNormalizedVector();
            Vector2D sec2Heading = new Vector2D(-45);
            VectorComparisonWithLog(sec2Value, sec2Heading);

            Vector2D sec3Value = new Vector2D(-1, -1).GetNormalizedVector();
            Vector2D sec3Heading = new Vector2D(-45 - 90);
            VectorComparisonWithLog(sec3Value, sec3Heading);

            Vector2D sec4Value = new Vector2D(1, -1).GetNormalizedVector();
            Vector2D sec4Heading = new Vector2D(45 + 90);
            VectorComparisonWithLog(sec4Value, sec4Heading);
        }

        [Fact]
        public void WithOtherVector()
        {
            double x = Rnd.NextDouble();
            double y = Rnd.NextDouble();

            Vector2D a = new Vector2D(x, y);
            Vector2D b = new Vector2D(a);

            Assert.True(a == b);
        }

        void VectorComparisonWithLog(Vector2D a, Vector2D b)
        {
            Output.WriteLine($"{a} ?= {b}");
            IsSameAssert(a, b);
        }
    }
}
