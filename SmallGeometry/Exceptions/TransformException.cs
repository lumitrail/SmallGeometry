namespace SmallGeometry.Exceptions
{
    /// <summary>
    /// Transform failed.
    /// </summary>
    public class TransformException :Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public override string Message { get; }
        /// <summary>
        /// Coordinate system transformed from
        /// </summary>
        public string? From { get; }
        /// <summary>
        /// Coordinate system transforming to
        /// </summary>
        public string? To { get; }

        /// <inheritdoc cref="TransformException"/>
        public TransformException()
            : base()
        {
            Message = "Unable to transform.";
        }

        /// <summary>
        /// <inheritdoc cref="TransformException"/>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public TransformException(CoordinateSystem from, CoordinateSystem to)
            : this()
        {
            From = from.ToString();
            To = to.ToString();
        }

        /// <summary>
        /// <inheritdoc cref="TransformException"/>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        public TransformException(CoordinateSystem from, CoordinateSystem to, string? message)
            : this(from, to)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Message = message;
            }
        }
    }
}
