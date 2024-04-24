using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
