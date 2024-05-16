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
            Vector a = new Vector(3, 4);
            Vector b = new Vector(3, 4);

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

            Vector va = new Vector(pa, pb);
            Vector vaa = new Vector(x, y);

            Assert.True(va == vaa);
            Assert.True(va.X == x);
            Assert.True(va.Y == y);
        }

        [Fact]
        public void HeadingReduction()
        {
            Vector h1 = new Vector(25);
            Vector h2 = new Vector(25 + 360);
            Vector h3 = new Vector(25 - 360);
            VectorComparisonWithLog(h1, h2);
            VectorComparisonWithLog(h2, h3);

            Vector h4 = new Vector(-25);
            Vector h5 = new Vector(-25 + 360);
            Vector h6 = new Vector(-25 - 360);
            VectorComparisonWithLog(h4, h5);
            VectorComparisonWithLog(h5, h6);
        }

        [Fact]
        public void WithHeadingAndCoordinate()
        {
            Vector eastValue = new Vector(1, 0);
            Vector eastHeading = new Vector(90);
            VectorComparisonWithLog(eastValue, eastHeading);

            Vector westValue = new Vector(-1, 0);
            Vector westHeading = new Vector(270);
            VectorComparisonWithLog(westValue, westHeading);

            Vector northValue = new Vector(0, 1);
            Vector northHeading = new Vector(0);
            VectorComparisonWithLog(northValue, northHeading);

            Vector southValue = new Vector(0, -1);
            Vector southHeading = new Vector(180);
            VectorComparisonWithLog(southValue, southHeading);

            Vector sec1Value = new Vector(1, 1).GetNormalizedVector();
            Vector sec1Heading = new Vector(45);
            VectorComparisonWithLog(sec1Value, sec1Heading);

            Vector sec2Value = new Vector(-1, 1).GetNormalizedVector();
            Vector sec2Heading = new Vector(-45);
            VectorComparisonWithLog(sec2Value, sec2Heading);

            Vector sec3Value = new Vector(-1, -1).GetNormalizedVector();
            Vector sec3Heading = new Vector(-45 - 90);
            VectorComparisonWithLog(sec3Value, sec3Heading);

            Vector sec4Value = new Vector(1, -1).GetNormalizedVector();
            Vector sec4Heading = new Vector(45 + 90);
            VectorComparisonWithLog(sec4Value, sec4Heading);
        }

        [Fact]
        public void WithOtherVector()
        {
            double x = Rnd.NextDouble();
            double y = Rnd.NextDouble();

            Vector a = new Vector(x, y);
            Vector b = new Vector(a);

            Assert.True(a == b);
        }

        void VectorComparisonWithLog(Vector a, Vector b)
        {
            Output.WriteLine($"{a} ?= {b}");
            IsSameAssert(a, b);
        }
    }
}
