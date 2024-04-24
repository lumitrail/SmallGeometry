namespace SmallGeometry
{
    /// <summary>
    /// Simple unit converters
    /// </summary>
    public static class UnitConverter
    {
        /// <summary>
        /// 360 degree to 2PI radian
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static double DegreeToRadian(double degree)
        {
            return (Math.PI / 180) * degree;
        }

        /// <summary>
        /// 2PI radian to 360 degree
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static double RadianToDegree(double radian)
        {
            return (180 / Math.PI) * radian;
        }

        /// <summary>
        /// km/h를 m/s로 변환
        /// </summary>
        /// <param name="kmph"></param>
        /// <returns></returns>
        public static double KmphToMps(double kmph)
        {
            return kmph / 3.6;
        }

        /// <summary>
        /// m/s를 km/h로 변환
        /// </summary>
        /// <param name="mps"></param>
        /// <returns></returns>
        public static double MpsToKmph(double mps)
        {
            return mps * 3.6;
        }

        /// <summary>
        /// distance meter의 거리를 km/h의 속도로 가면 몇 분이 걸리는지
        /// </summary>
        /// <param name="distanceInMeter"></param>
        /// <param name="kmph"></param>
        /// <returns></returns>
        public static double GetTravelTimeMinutes(double distanceInMeter, double kmph)
        {
            double mps = KmphToMps(kmph);
            double seconds = distanceInMeter / mps;
            return seconds / 60;
        }
    }
}
