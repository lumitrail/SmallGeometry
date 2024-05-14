using System.Diagnostics;

using SmallGeometry.Euclidean;
using SmallGeometry.Exceptions;
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
        /// <exception cref="CoordinateSystemNoneException">source coordinate system is none</exception>
        /// <exception cref="TransformException">failed to transform</exception>
        public static GeoPoint TransformToGeoPoint(FlatPoint sourcePoint)
        {
            if (sourcePoint.CoordinateSystem == CoordinateSystem.None)
            {
                throw new CoordinateSystemNoneException(nameof(sourcePoint));
            }
            else
            {
                DotSpatial.Projections.ProjectionInfo? sourceProjection;

                try
                {
                    sourceProjection = ProjectionRepository.GetProjectionInfo(sourcePoint.CoordinateSystem);
                }
                catch (Exception E)
                {
                    throw new TransformException(sourcePoint.CoordinateSystem, CoordinateSystem.Epsg4326, E.Message);
                }

                (double x, double y) = Transform(sourcePoint.X, sourcePoint.Y, sourceProjection, ProjectionRepository.Proj4326);
                try
                {
                    return new GeoPoint(x, y);
                }
                catch (Exception E)
                {
                    throw new TransformException(sourcePoint.CoordinateSystem, CoordinateSystem.Epsg4326, E.Message);
                }
            }
        }

        /// <summary>
        /// FlatPoint to GeoPoint
        /// </summary>
        /// <param name="sourcePoints"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">sourcePoints is empty</exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CoordinateSystemNoneException">sourcePoints has elemets of coordinate system none</exception>
        /// <exception cref="TransformException">failed to transform</exception>
        public static List<GeoPoint> TransformToGeoPoint(IEnumerable<FlatPoint> sourcePoints)
        {
            ArgumentNullException.ThrowIfNull(sourcePoints);
            if (!sourcePoints.Any())
            {
                throw new ArgumentException(ExceptionMessages.PointsCountZero, nameof(sourcePoints));
            }

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
        /// <exception cref="CoordinateSystemNoneException">target coordinate system is none</exception>
        /// <exception cref="TransformException">failed to transform</exception>
        public static FlatPoint TransformToFlat(GeoPoint sourcePoint, CoordinateSystem targetCoordinateSystem)
        {
            if (targetCoordinateSystem == CoordinateSystem.None)
            {
                throw new CoordinateSystemNoneException(nameof(targetCoordinateSystem));
            }
            else
            {
                DotSpatial.Projections.ProjectionInfo? sourceProjection;
                DotSpatial.Projections.ProjectionInfo? targetProjection;

                try
                {
                    sourceProjection = ProjectionRepository.GetProjectionInfo(sourcePoint.CoordinateSystem);
                    targetProjection = ProjectionRepository.GetProjectionInfo(targetCoordinateSystem);
                }
                catch (Exception E)
                {
                    throw new TransformException(sourcePoint.CoordinateSystem, targetCoordinateSystem, E.Message);
                }

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
        /// <exception cref="ArgumentException">sourcePoints is empty</exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CoordinateSystemNoneException">target coordinate system is none</exception>
        /// <exception cref="TransformException">failed to transform</exception>
        public static List<FlatPoint> TransformToFlat(IEnumerable<GeoPoint> sourcePoints, CoordinateSystem targetCoordinateSystem)
        {
            ArgumentNullException.ThrowIfNull(sourcePoints);
            if (!sourcePoints.Any())
            {
                throw new ArgumentException(ExceptionMessages.PointsCountZero, nameof(sourcePoints));
            }

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
        /// <exception cref="CoordinateSystemNoneException">source XOR target coordinate system is none</exception>
        /// <exception cref="TransformException">failed to transform</exception>
        public static FlatPoint TransformToFlat(FlatPoint sourcePoint, CoordinateSystem targetCoordinateSystem)
        {
            if (sourcePoint.CoordinateSystem == targetCoordinateSystem)
            {
                return sourcePoint;
            }
            else if (targetCoordinateSystem == CoordinateSystem.None)
            {
                throw new CoordinateSystemNoneException(nameof(targetCoordinateSystem));
            }
            else if (sourcePoint.CoordinateSystem == CoordinateSystem.None)
            {
                throw new CoordinateSystemNoneException(nameof(sourcePoint));
            }
            else
            {
                DotSpatial.Projections.ProjectionInfo? sourceProjection;
                DotSpatial.Projections.ProjectionInfo? targetProjection;

                try
                {
                    sourceProjection = ProjectionRepository.GetProjectionInfo(sourcePoint.CoordinateSystem);
                    targetProjection = ProjectionRepository.GetProjectionInfo(targetCoordinateSystem);
                }
                catch (Exception E)
                {
                    throw new TransformException(sourcePoint.CoordinateSystem, targetCoordinateSystem, E.Message);
                }

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
        /// <exception cref="ArgumentException">sourcePoints is empty</exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CoordinateSystemNoneException">source or target coordinate system is none</exception>
        /// <exception cref="TransformException">failed to transform</exception>
        public static List<FlatPoint> TransformToFlat(IEnumerable<FlatPoint> sourcePoints, CoordinateSystem targetCoordinateSystem)
        {
            ArgumentNullException.ThrowIfNull(sourcePoints);

            if (!sourcePoints.Any())
            {
                throw new ArgumentException(ExceptionMessages.PointsCountZero, nameof(sourcePoints));
            }

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
        /// <exception cref="TransformException"></exception>
        private static (double x, double y) Transform(double sourceX, double sourceY,
            DotSpatial.Projections.ProjectionInfo sourceProjection,
            DotSpatial.Projections.ProjectionInfo targetProjection)
        {
            Debug.Assert(sourceProjection != null);
            Debug.Assert(targetProjection != null);

            try
            {
                double[] resultXY = [ sourceX, sourceY ];
                double[] z = [ 0 ];

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
