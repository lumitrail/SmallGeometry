using System.Diagnostics.CodeAnalysis;

using DotSpatial.Projections;

namespace SmallGeometry
{
    /// <summary>
    /// Contains GIS projection info.
    /// </summary>
    internal static class ProjectionRepository
    {
        private static readonly Dictionary<int, ProjectionInfo> s_projections = new Dictionary<int, ProjectionInfo>();
        public static IReadOnlyDictionary<int, ProjectionInfo> Projections => s_projections;

        internal static readonly ProjectionInfo Proj4326 = ProjectionInfo.FromProj4String("+proj=longlat +datum=WGS84 +no_defs +type=crs");
        internal static readonly ProjectionInfo Proj5179 = ProjectionInfo.FromProj4String("+proj=tmerc +lat_0=38 +lon_0=127.5 +k=0.9996 +x_0=1000000 +y_0=2000000 +ellps=GRS80 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs +type=crs");
        internal static readonly ProjectionInfo Proj5186 = ProjectionInfo.FromProj4String("+proj=tmerc +lat_0=38 +lon_0=127 +k=1 +x_0=200000 +y_0=600000 +ellps=GRS80 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs +type=crs");


        static ProjectionRepository()
        {
            Proj4326.IsLatLon = true;
            s_projections.Add(4326, Proj4326);
            s_projections.Add(5179, Proj5179);
            s_projections.Add(5186, Proj5186);
        }


        public static bool Equals(ProjectionInfo a, ProjectionInfo b)
        {
            if (a == b)
            {
                return true;
            }
            else if (a == null || b == null)
            {
                return false;
            }
            else if (a.Equals(b))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to get SRID of projectionInfo when there is matching data.
        /// </summary>
        /// <param name="projectionInfo"></param>
        /// <param name="srid">0 when no projection</param>
        /// <returns></returns>
        public static bool TryGetSRID(ProjectionInfo projectionInfo, out int srid)
        {
            if (projectionInfo == null)
            {
                srid = 0;
                return true;
            }

            // shallow comparison
            foreach (KeyValuePair<int, ProjectionInfo> knownProjection in s_projections)
            {
                if (projectionInfo == knownProjection.Value)
                {
                    srid = knownProjection.Key;
                    return true;
                }
            }

            // deeper inspection
            foreach (KeyValuePair<int, ProjectionInfo> knownProjection in s_projections)
            {
                if (projectionInfo.Equals(knownProjection.Value))
                {
                    srid = knownProjection.Key;
                    return true;
                }
            }

            srid = 0;
            return false;
        }

        /// <summary>
        /// Tries to get ProjectionInfo from srid when there is matching data.
        /// </summary>
        /// <param name="srid"></param>
        /// <param name="projectionInfo"></param>
        /// <returns></returns>
        public static bool TryGetProjectionInfo(int srid, [NotNullWhen(true)] out ProjectionInfo? projectionInfo)
        {
            return s_projections.TryGetValue(srid, out projectionInfo);
        }

        /// <summary>
        /// Tries to get ProjectionInfo from coordinateSystem when there is matching data.
        /// </summary>
        /// <param name="coordinateSystem"></param>
        /// <param name="projectionInfo"></param>
        /// <returns></returns>
        public static bool TryGetProjectionInfo(CoordinateSystem coordinateSystem, [NotNullWhen(true)] out ProjectionInfo? projectionInfo)
        {
            return TryGetProjectionInfo((int)coordinateSystem, out projectionInfo);
        }

        /// <summary>
        /// Gets ProjectionInfo from coordinateSystem when there is matching data.
        /// </summary>
        /// <param name="coordinateSystem"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">no matching data</exception>
        public static ProjectionInfo GetProjectionInfo(CoordinateSystem coordinateSystem)
        {
            if (TryGetProjectionInfo(coordinateSystem, out ProjectionInfo? result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException($"No matching Projection info for CoordinateSystem({coordinateSystem})");
            }
        }
    }
}
