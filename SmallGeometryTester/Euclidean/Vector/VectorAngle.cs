namespace SmallGeometryTester
{
    public class VectorAngle
    {
        private readonly ITestOutputHelper Output;

        private static readonly Vector North = new Vector(0);
        private static readonly Vector South = new Vector(180);
        private static readonly Vector East = new Vector(90);
        private static readonly Vector West = new Vector(-90);

        private static readonly Random RNG = new Random((int)DateTime.Now.Ticks);

        public VectorAngle(ITestOutputHelper outputHelper)
        {
            Output = outputHelper;
        }

        [Fact]
        public void VectorGetAngleDegreeTest()
        {
            int testCount = 100;

            for (int i = 0; i < testCount; ++i)
            {
                double degree = RNG.NextDouble() * 360;

                Vector vector = new Vector(degree);

                double calculatedDegree = vector.GetAngleDegree(North);

                double diff = Math.Abs(degree - calculatedDegree);

                Assert.True(diff < 0.001);
            }
        }

        [Fact]
        public void VectorRotation()
        {
            int testCount = 100;

            for (int i=0; i<testCount; ++i)
            {
                double degree = RNG.NextDouble() * 360;

                VectorRotationTest(North, degree);
                VectorRotationTest(North, -degree);

                VectorRotationTest(South, degree);
                VectorRotationTest(South, -degree);

                VectorRotationTest(East, degree);
                VectorRotationTest(East, -degree);

                VectorRotationTest(West, degree);
                VectorRotationTest(West, -degree);
            }
        }

        private static void VectorRotationTest(Vector v, double degreeCW)
        {
            double originalDegree = v.GetAngleDegree(North);

            Vector rotatedCw = v.GetRotatedVector(degreeCW);
            double rotatedCwDegree = rotatedCw.GetAngleDegree(North);

            Assert.True(Math.Abs(rotatedCwDegree - originalDegree - degreeCW) < 0.001);
        }


        private static bool IsDegreeApproximatelySame(Vector a, Vector b, double degree)
        {
            double calculatedAngleDegree = Vector.GetAngleDegree(a, b);
            bool ok = IsApproximatelySame(calculatedAngleDegree, degree);

            return ok;
        }

        private static bool IsApproximatelySame(double a, double b)
        {
            return (Math.Abs(a - b) < 0.01);
        }
    }
}
