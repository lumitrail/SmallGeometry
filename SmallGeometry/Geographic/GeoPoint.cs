﻿using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using SmallGeometry.Exceptions;
using SmallGeometry.Interfaces;

namespace SmallGeometry.Geographic
{
    /// <summary>
    /// Epsg4326(=WGS84) Point
    /// </summary>
    public readonly struct GeoPoint : IPosition2D, ISridCoordinate
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public readonly CoordinateSystem CoordinateSystem => CoordinateSystem.Epsg4326;
        /// <summary>
        /// East-West(X), [-180,180]
        /// </summary>
        public readonly double Longitude => X;
        /// <summary>
        /// North-South(Y), [-90,90]
        /// </summary>
        public readonly double Latitude => Y;

        /// <summary>
        /// Same to longitude
        /// </summary>
        [JsonIgnore]
        public readonly double X { get; }
        /// <summary>
        /// Same to latitude
        /// </summary>
        [JsonIgnore]
        public readonly double Y { get; }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public GeoPoint(double longitude, double latitude)
        {
            bool longitudeOut =
                longitude < -180
                || 180 < longitude;

            bool latitudeOut =
                latitude < -90
                || 90 < latitude;

            if (longitudeOut && latitudeOut)
            {
                throw new ArgumentOutOfRangeException($"{nameof(longitude)},{nameof(latitude)}",
                    $"Longitude and Latitude are out of range: {longitude},{latitude}");
            }
            else if (longitudeOut)
            {
                throw new ArgumentOutOfRangeException(nameof(longitude),
                    $"Longitude is out of range: {longitude},{latitude}");
            }
            else if (latitudeOut)
            {
                throw new ArgumentOutOfRangeException(nameof(latitude),
                    $"Latitude is out of range: {longitude},{latitude}");
            }
            else
            {
                X = longitude;
                Y = latitude;
            }
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="source"></param>
        public GeoPoint(GeoPoint source)
        {
            X = source.X;
            Y = source.Y;
        }



        /// <summary>
        /// Tries to parse string like "(x, y)", "[x, y]"
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string point, out GeoPoint result)
        {
            try
            {
                char[] trimChar = new char[] { '(', ')', '[', ']', ',' };
                string[] xy = point.Split(',');

                if (xy.Length == 2)
                {
                    if (double.TryParse(xy[0].Trim().Trim(trimChar), out double x)
                        && double.TryParse(xy[1].Trim().Trim(trimChar), out double y))
                    {
                        result = new GeoPoint(x, y);
                        return true;
                    }
                }
            }
            catch
            {
            }

            result = new GeoPoint(0, 0);
            return false;
        }


        /// <summary>
        /// Points are exactly the same.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(GeoPoint a, GeoPoint b)
        {
            return (a.Longitude == b.Longitude)
                && (a.Latitude == b.Latitude);
        }

        /// <summary>
        /// Points are not exactly the same.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(GeoPoint a, GeoPoint b)
        {
            return !(a == b);
        }


        /// <summary>
        /// Gets distance with Haversine formula.
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns>distance in meter</returns>
        /// <remarks>https://gist.github.com/jammin77/033a332542aa24889452</remarks>
        public static double GetDistanceInMeter(GeoPoint pos1, GeoPoint pos2)
        {
            double R = 6371000; // earth radius in meter

            double dLat = UnitConverter.DegreeToRadian(pos2.Latitude - pos1.Latitude);
            double dLon = UnitConverter.DegreeToRadian(pos2.Longitude - pos1.Longitude);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                + Math.Cos(UnitConverter.DegreeToRadian(pos1.Latitude))
                * Math.Cos(UnitConverter.DegreeToRadian(pos2.Latitude))
                * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));

            return R * c;
        }

        /// <summary>
        /// Transforms to FlatPoint
        /// </summary>
        /// <param name="coordinateSystem"></param>
        /// <returns></returns>
        /// <exception cref="CoordinateSystemNoneException">source or target coordinate system is none</exception>
        /// <exception cref="NotSupportedException">coordinateSystem must be flat</exception>
        /// <exception cref="TransformException">failed to transform</exception>
        public readonly Euclidean.FlatPoint Transform(CoordinateSystem coordinateSystem)
        {
            if (!CoordinateSystemUtil.IsCoordinateSystemFlat(coordinateSystem))
            {
                throw new NotSupportedException(ExceptionMessages.CoordinateSystemMustBeFlat + coordinateSystem);
            }
            return Transformer.TransformToFlat(this, coordinateSystem);
        }

        /// <summary>
        /// Gets distance with Haversine.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public readonly double GetDistanceInMeter(GeoPoint b)
        {
            return GetDistanceInMeter(this, b);
        }

        /// <summary>
        /// Gets wkt string.
        /// </summary>
        /// <returns>"ST_Point(Longitude,Latitude,4326)"</returns>
        /// <remarks>Postgis 3.2.0 or above</remarks>
        public readonly string GetWkt()
        {
            return $"ST_Point({Longitude},{Latitude},4326)";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public readonly override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj is GeoPoint b)
            {
                return this == b;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc cref="IPosition2D.GetHashCode(double, double)"/>
        public readonly override int GetHashCode()
        {
            return IPosition2D.GetHashCode(X, Y);
        }

        /// <inheritdoc cref="IPosition2D.ToString(double, double)"/>
        public readonly override string ToString()
        {
            return IPosition2D.ToString(X, Y);
        }
    }
}
