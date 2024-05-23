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
        public CoordinateSystemDiscordanceException(CoordinateSystem a, CoordinateSystem b)
            : base()
        {
            Message = $"Coordinate system must be the same among objects, but now are {a}, {b}.";
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
