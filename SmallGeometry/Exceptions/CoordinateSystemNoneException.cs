namespace SmallGeometry.Exceptions
{
    /// <summary>
    /// Coordinate system must be provided but was None.
    /// </summary>
    public class CoordinateSystemNoneException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public override string Message { get; }
        /// <summary>
        /// 
        /// </summary>
        public string? ParamName { get; }

        /// <inheritdoc cref="CoordinateSystemNoneException"/>
        public CoordinateSystemNoneException()
            : base()
        {
            Message = "Coordinate system is none.";
        }

        /// <summary>
        /// <inheritdoc cref="CoordinateSystemNoneException"/>
        /// </summary>
        /// <param name="paramName"></param>
        public CoordinateSystemNoneException(string? paramName)
            : this()
        {
            ParamName = ParamName;
        }
    }
}
