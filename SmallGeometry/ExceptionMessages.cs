namespace SmallGeometry
{
    internal static class ExceptionMessages
    {
        public static readonly string PointsCountZero = "Points list is empty.";
        public static readonly string CoordinateSystemUnsupported = "This data type doesn't support the coordinate system.";
        public static readonly string CoordinateSystemMustBeFlat = "Coordinate system must be flat.";


        #region Polygon
        public static readonly string HasSelfIntersection = "Points list has self intersection.";
        public static   string PointsCountAtLeast(int count)
        {
            return $"Points list has at least {count} points.";
        }
        #endregion
    }
}
