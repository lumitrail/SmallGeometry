namespace SmallGeometry
{
    internal class CoordinateSystemUtil
    {
        /// <summary>
        /// Determines whether coordinateSystem is ellipsoidal or not.
        /// </summary>
        /// <param name="coordinateSystem"></param>
        /// <returns></returns>
        public static bool IsCoordinateSystemEllipsoidal(CoordinateSystem coordinateSystem)
        {
            return coordinateSystem == CoordinateSystem.Epsg4326;
        }

        /// <summary>
        /// Determines whether coordinateSystem is flat or not.
        /// </summary>
        /// <param name="coordinateSystem"></param>
        /// <returns></returns>
        public static bool IsCoordinateSystemFlat(CoordinateSystem coordinateSystem)
        {
            return coordinateSystem == CoordinateSystem.Epsg5179
                || coordinateSystem == CoordinateSystem.Epsg5186;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="coordinateSystemObjects"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsCoordinateSystemMixed(params Interfaces.ISridCoordinate[] coordinateSystemObjects)
        {
            ArgumentNullException.ThrowIfNull(coordinateSystemObjects);
            if (coordinateSystemObjects.Count() == 0)
            {
                return false;
            }

            return coordinateSystemObjects.DistinctBy(cso => cso.CoordinateSystem).Count() > 1;
        }
    }
}
