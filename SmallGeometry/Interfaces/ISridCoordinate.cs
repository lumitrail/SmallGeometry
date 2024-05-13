namespace SmallGeometry.Interfaces
{
    /// <summary>
    /// Interface which provides SRID
    /// </summary>
    public interface ISridCoordinate
    {
        /// <summary>
        /// EPSG
        /// </summary>
        public CoordinateSystem CoordinateSystem { get; }
    }
}
