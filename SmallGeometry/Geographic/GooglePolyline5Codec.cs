using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private const int MASK = 0b11111;

        #region Encode
        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static string Encode(IEnumerable<GeoPoint> points)
        {
            ArgumentNullException.ThrowIfNull(points);
            if (points.Count() == 0)
            {
                throw new ArgumentException("Argument has no points.", nameof(points));
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

            while (playingValue > MASK)
            {
                bit5Chunks.Append((char)(((playingValue & MASK) | 0x20) + 63));
                playingValue >>>= 5;
            }
            bit5Chunks.Append((char)((playingValue & MASK) + 63));

            return bit5Chunks.ToString();
        }
        #endregion

        #region Decode
        /// <summary>
        /// 
        /// </summary>
        /// <param name="googlePolyline5"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static List<GeoPoint> Decode(string googlePolyline5)
        {
            {
                ArgumentNullException.ThrowIfNull(googlePolyline5);
                char[] errorChars = GetErrorCharacters(googlePolyline5);
                if (errorChars.Length != 0)
                {
                    throw new ArgumentException($"Parameter has one or more characters out of range: {string.Join(',', errorChars)}", nameof(googlePolyline5));
                }
            }

            var result = new List<GeoPoint>(googlePolyline5.Length / 2);

            List<List<int>> reversed5BitChunks = GetReversed5BitChunkedDiffs(googlePolyline5);

            double latitude = 0;
            double longitude = 0;

            for (int i=0; i<reversed5BitChunks.Count; i+=2)
            {
                int latitudeDiffE5 = GetDiffE5(reversed5BitChunks[i]);
                int longitudeDiffE5 = GetDiffE5(reversed5BitChunks[i + 1]);

                latitude += (double)latitudeDiffE5 / 1E5;
                longitude += (double)longitudeDiffE5 / 1E5;

                Console.WriteLine($"{longitude},{latitude}");

                result.Add(new GeoPoint(longitude, latitude));
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="googlePolyline5"></param>
        /// <returns></returns>
        private static List<List<int>> GetReversed5BitChunkedDiffs(string googlePolyline5)
        {
            Debug.Assert(googlePolyline5 != null);
            Debug.Assert(googlePolyline5.Length >= 2);
            Debug.Assert(GetErrorCharacters(googlePolyline5).Length == 0);

            var segments = new List<List<int>>(googlePolyline5.Length / 2);
            var chunk = new List<int>(5);

            foreach (char c in googlePolyline5)
            {
                int cMinus63 = (c - 63);
                if (cMinus63 > MASK)
                {
                    chunk.Add(cMinus63 & MASK);
                }
                else
                {
                    chunk.Add(cMinus63);
                    segments.Add(chunk);
                    chunk = new List<int>(5);

                    Debug.Assert(chunk.Count == 0);
                }
            }

            Debug.Assert(segments.Count >= 2);
            return segments;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="googlePolyline5"></param>
        /// <returns></returns>
        private static char[] GetErrorCharacters(string googlePolyline5)
        {
            return googlePolyline5.Where(c => c < 63 || c > 0b1111110).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fiveBitChunkedDiff"></param>
        /// <returns></returns>
        private static int GetDiffE5(List<int> fiveBitChunkedDiff)
        {
            Debug.Assert(fiveBitChunkedDiff != null);
            Debug.Assert(fiveBitChunkedDiff.Count(c => c > 0x11111 || c < 0) == 0);
            
            int result = 0;

            for (int i=fiveBitChunkedDiff.Count-1; i>=0; --i)
            {
                result <<= 5;
                result += fiveBitChunkedDiff[i];
            }

            if (result %2 == 1)
            {
                result = ~result;
            }

            result >>= 1;

            return result;
        }
        #endregion
    }
}
