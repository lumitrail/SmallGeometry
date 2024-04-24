namespace SmallGeometry.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPosition2D
    {
        /// <summary>
        /// 
        /// </summary>
        double X { get; }
        /// <summary>
        /// 
        /// </summary>
        double Y { get; }


        /// <summary>
        /// CRC32
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int GetHashCode(double x, double y)
        {
            return (int)CRC32.Get(new double[] { x, y });
        }

        /// <summary>
        /// [X,Y]
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>"[X,Y]"</returns>
        public static string ToString(double x, double y)
        {
            return $"[{x},{y}]";
        }
    }
}
