using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallGeometry.Geographic
{
    /// <summary>
    /// Util class to encode/decode google polyline 5
    /// </summary>
    public static class GooglePolyline5Codec
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string Encode(IEnumerable<GeoPoint> points)
        {
            ArgumentNullException.ThrowIfNull(points);
            if (points.Count() == 0)
            {
                return "??";
            }

            var resultSb = new System.Text.StringBuilder(points.Count() * 10);

            double previousLongitude = 0;
            double previousLatitude = 0;

            foreach (var point in points)
            {
                resultSb.Append(GetAsciiElement(previousLatitude, point.Latitude));
                resultSb.Append(GetAsciiElement(previousLongitude, point.Longitude));

                previousLongitude = point.Longitude;
                previousLatitude = point.Latitude;
            }

            return resultSb.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        private static string GetAsciiElement(double previous, double next)
        {
            int diff = (int)Math.Round((next - previous) * 1E5);

            int playingValue = diff << 1;

            playingValue = diff < 0 ?
                ~playingValue
                : playingValue;

            var bit5Chunks = new System.Text.StringBuilder(8);
            const int mask = 0b11111;

            while (playingValue > mask)
            {
                bit5Chunks.Append((char)(((playingValue & mask) | 0x20) + 63));
                playingValue = playingValue >> 5;
            }
            bit5Chunks.Append((char)((playingValue & mask) + 63));

            return bit5Chunks.ToString();
        }
    }
}
