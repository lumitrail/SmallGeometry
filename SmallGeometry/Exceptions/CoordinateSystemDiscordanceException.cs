namespace SmallGeometry.Exceptions
{
    /// <summary>
    /// Coordinate system must be the same among objects.
    /// </summary>
    public class CoordinateSystemDiscordanceException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public override string Message { get; }


        /// <inheritdoc cref="CoordinateSystemDiscordanceException"/>
        public CoordinateSystemDiscordanceException()
            : base()
        {
            Message = "Coordinate system must be the same among objects.";
        }

        /// <inheritdoc cref="CoordinateSystemDiscordanceException"/>
        /// <param name="coordinateSystems"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CoordinateSystemDiscordanceException(IEnumerable<CoordinateSystem> coordinateSystems)
            : base()
        {
            ArgumentNullException.ThrowIfNull(coordinateSystems);
            Message = $"Coordinate system must be the same among objects, but now are {string.Join(',', coordinateSystems)}.";
        }

        /// <inheritdoc cref="CoordinateSystemDiscordanceException"/>
        /// <param name="coordinateSystems"></param>
        public CoordinateSystemDiscordanceException(params CoordinateSystem[] coordinateSystems)
            : this(coordinateSystems.AsEnumerable())
        {
        }

        /// <inheritdoc cref="CoordinateSystemDiscordanceException"/>
        /// <param name="objectsWithCoordinateSystem"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CoordinateSystemDiscordanceException(IEnumerable<Interfaces.ISridCoordinate> objectsWithCoordinateSystem)
            : this(objectsWithCoordinateSystem.Select(o => o.CoordinateSystem))
        {
        }

        /// <inheritdoc cref="CoordinateSystemDiscordanceException"/>
        /// <param name="objectsWithCoordinateSystem"></param>
        public CoordinateSystemDiscordanceException(params Interfaces.ISridCoordinate[] objectsWithCoordinateSystem)
            : this(objectsWithCoordinateSystem.Select(o => o.CoordinateSystem))
        {
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void ThrowWhenDifferent(Interfaces.ISridCoordinate a, Interfaces.ISridCoordinate b)
        {
            if (a.CoordinateSystem != b.CoordinateSystem)
            {
                throw new CoordinateSystemDiscordanceException(a.CoordinateSystem, b.CoordinateSystem);
            }
        }
    }
}
