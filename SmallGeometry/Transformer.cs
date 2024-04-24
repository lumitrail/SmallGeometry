using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DotSpatial.Projections;

using SmallGeometry.Exceptions;
using SmallGeometry.Euclidean;
using SmallGeometry.Geographic;

namespace SmallGeometry
{
    /// <summary>
    /// Coordinate system transformer.
    /// </summary>
    internal static class Transformer
    {
        /// <summary>
        /// FlatPoint to GeoPoint
        /// </summary>
        /// <param name="sourcePoint"></param>
        /// <returns></returns>
        /// <exception cref="CoordinateSystemNoneException">source or target coordinate system is none</exception>
        /// <exception cref="ArgumentException">failed to get projection info</exception>
        /// <exception cref="TransformException">failed to transform</exception>
        public static GeoPoint TransformToGeoPoint(FlatPoint sourcePoint)
        {
            if (sourcePoint.CoordinateSystem == CoordinateSystem.None)
            {
                throw new CoordinateSystemNoneException(nameof(sourcePoint));
            }
            else
            {
                var sourceProjection = ProjectionRepository.GetProjectionInfo(sourcePoint.CoordinateSystem);

                (double x, double y) = Transform(sourcePoint.X, sourcePoint.Y, sourceProjection, ProjectionRepository.Proj4326);
                return new GeoPoint(x, y);
            }
        }

        /// <summary>
        /// FlatPoint to GeoPoint
        /// </summary>
        /// <param name="sourcePoints"></param>
        /// <returns></returns>
        /// <exception cref="CoordinateSystemNoneException">source or target coordinate system is none</exception>
        /// <exception cref="ArgumentException">failed to get projection info</exception>
        /// <exception cref="TransformException">failed to transform</exception>
        public static List<GeoPoint> TransformToGeoPoint(IEnumerable<FlatPoint> sourcePoints)
        {
            var result = new List<GeoPoint>(sourcePoints.Count());

            foreach (var gp in sourcePoints)
            {
                result.Add(TransformToGeoPoint(gp));
            }

            return result;
        }

        /// <summary>
        /// GeoPoint to FlatPoint
        /// </summary>
        /// <param name="sourcePoint"></param>
        /// <param name="targetCoordinateSystem"></param>
        /// <returns></returns>
        /// <exception cref="CoordinateSystemNoneException">source or target coordinate system is none</exception>
        /// <exception cref="ArgumentException">failed to get projection info</exception>
        /// <exception cref="TransformException">failed to transform</exception>
        public static FlatPoint TransformToFlat(GeoPoint sourcePoint, CoordinateSystem targetCoordinateSystem)
        {
            if (targetCoordinateSystem == CoordinateSystem.None)
            {
                throw new CoordinateSystemNoneException(nameof(targetCoordinateSystem));
            }
            else
            {
                var sourceProjection = ProjectionRepository.GetProjectionInfo(sourcePoint.CoordinateSystem);
                var targetProjection = ProjectionRepository.GetProjectionInfo(targetCoordinateSystem);

                (double x, double y) = Transform(sourcePoint.Longitude, sourcePoint.Latitude, sourceProjection, targetProjection);
                return new FlatPoint(x, y, targetCoordinateSystem);
            }
        }

        /// <summary>
        /// GeoPoint to FlatPoint
        /// </summary>
        /// <param name="sourcePoints"></param>
        /// <param name="targetCoordinateSystem"></param>
        /// <returns></returns>
        /// <exception cref="CoordinateSystemNoneException">source or target coordinate system is none</exception>
        /// <exception cref="ArgumentException">failed to get projection info</exception>
        /// <exception cref="TransformException">failed to transform</exception>
        public static List<FlatPoint> TransformToFlat(IEnumerable<GeoPoint> sourcePoints, CoordinateSystem targetCoordinateSystem)
        {
            var result = new List<FlatPoint>(sourcePoints.Count());

            foreach (var gp in sourcePoints)
            {
                result.Add(TransformToFlat(gp, targetCoordinateSystem));
            }

            return result;
        }


        /// <summary>
        /// FlatPoint to FlatPoint
        /// </summary>
        /// <param name="sourcePoint"></param>
        /// <param name="targetCoordinateSystem"></param>
        /// <returns></returns>
        /// <exception cref="CoordinateSystemNoneException">source or target coordinate system is none</exception>
        /// <exception cref="ArgumentException">failed to get projection info</exception>
        /// <exception cref="TransformException">failed to transform</exception>
        public static FlatPoint TransformToFlat(FlatPoint sourcePoint, CoordinateSystem targetCoordinateSystem)
        {
            if (targetCoordinateSystem == CoordinateSystem.None)
            {
                throw new CoordinateSystemNoneException(nameof(targetCoordinateSystem));
            }
            else if (sourcePoint.CoordinateSystem == CoordinateSystem.None)
            {
                throw new CoordinateSystemNoneException(nameof(sourcePoint));
            }
            else if (sourcePoint.CoordinateSystem == targetCoordinateSystem)
            {
                return sourcePoint;
            }
            else
            {
                var sourceProjection = ProjectionRepository.GetProjectionInfo(sourcePoint.CoordinateSystem);
                var targetProjection = ProjectionRepository.GetProjectionInfo(targetCoordinateSystem);

                (double x, double y) = Transform(sourcePoint.X, sourcePoint.Y, sourceProjection, targetProjection);
                return new FlatPoint(x, y, targetCoordinateSystem);
            }
        }

        /// <summary>
        /// FlatPoint to FlatPoint
        /// </summary>
        /// <param name="sourcePoints"></param>
        /// <param name="targetCoordinateSystem"></param>
        /// <returns></returns>
        /// <exception cref="CoordinateSystemNoneException">source or target coordinate system is none</exception>
        /// <exception cref="ArgumentException">failed to get projection info</exception>
        /// <exception cref="TransformException">failed to transform</exception>
        public static List<FlatPoint> TransformToFlat(IEnumerable<FlatPoint> sourcePoints, CoordinateSystem targetCoordinateSystem)
        {
            var result = new List<FlatPoint>(sourcePoints.Count());

            foreach (var fp in sourcePoints)
            {
                result.Add(TransformToFlat(fp, targetCoordinateSystem));
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceX"></param>
        /// <param name="sourceY"></param>
        /// <param name="sourceProjection"></param>
        /// <param name="targetProjection"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="TransformException"></exception>
        private static (double x, double y) Transform(double sourceX, double sourceY,
            DotSpatial.Projections.ProjectionInfo sourceProjection,
            DotSpatial.Projections.ProjectionInfo targetProjection)
        {
            if (sourceProjection == null)
            {
                throw new ArgumentNullException(nameof(sourceProjection));
            }
            if (targetProjection == null)
            {
                throw new ArgumentNullException(nameof(targetProjection));
            }

            try
            {
                double[] resultXY = new double[] { sourceX, sourceY };
                double[] z = new double[] { 0 };

                DotSpatial.Projections.Reproject.ReprojectPoints(resultXY, z, sourceProjection, targetProjection, 0, 1);

                return (resultXY[0], resultXY[1]);
            }
            catch
            {
                throw new TransformException();
            }
        }
    }
}
