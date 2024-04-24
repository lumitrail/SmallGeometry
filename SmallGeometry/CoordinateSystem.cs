namespace SmallGeometry
{
    /// <summary>
    /// SRID of supported coordinate systems.
    /// </summary>
    public enum CoordinateSystem
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// WGS84
        /// </summary>
        Epsg4326 = 4326,
        /// <summary>
        /// UTM-K (GRS80)
        /// </summary>
        Epsg5179 = 5179,
        /// <summary>
        /// Korea (GRS80)
        /// </summary>
        Epsg5186 = 5186,
    }
}
