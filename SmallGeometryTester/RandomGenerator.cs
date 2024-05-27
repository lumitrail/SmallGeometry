namespace SmallGeometryTester
{
    /// <summary>
    /// double 범위는 -10 ~ 10
    /// </summary>
    internal static class RandomGenerator
    {
        private static readonly Random RNG = new Random((int)DateTime.Now.Ticks);

        /// <summary>
        /// <inheritdoc cref="RandomGenerator"/>
        /// </summary>
        /// <returns></returns>
        public static Vector2D GetRandomNonzeroVector()
        {
            var v = new Vector2D(0, 0);
            while (v == Vector2D.Zero)
            {
                v = new Vector2D(GetRandomDouble(), GetRandomDouble());
            }
            return v;
        }

        /// <summary>
        /// <inheritdoc cref="RandomGenerator"/>
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
        public static FlatPoint GetRandomPoint(CoordinateSystem cs)
        {
            return new FlatPoint(GetRandomDouble(), GetRandomDouble(), cs);
        }

        /// <summary>
        /// <inheritdoc cref="RandomGenerator"/>
        /// </summary>
        /// <returns></returns>
        public static double GetRandomDouble()
        {
            bool xNegative = RNG.NextDouble() < 0.5;
            double x = xNegative ? RNG.NextDouble() * 10 : -RNG.NextDouble() * 10;

            return x;
        }

        /// <summary>
        /// 5:5 = true:false
        /// </summary>
        /// <returns></returns>
        public static bool TossCoin()
        {
            return RNG.NextDouble() < 0.5;
        }
    }
}
