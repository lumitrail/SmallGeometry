namespace SmallGeometry
{
    /// <summary>
    /// CRC-32 using System.IO.Hashing.Crc32
    /// </summary>
    internal static class Crc32Wrapper
    {
        /// <summary>
        /// <inheritdoc cref="Crc32Wrapper"/>
        /// </summary>
        /// <param name="doubles"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static int GetCrc32Hash(params double[] doubles)
        {
            ArgumentNullException.ThrowIfNull(doubles);
            if (doubles.Length == 0)
            {
                throw new ArgumentException("Array is empty", nameof(doubles));
            }

            byte[] bytes = new byte[doubles.Length * sizeof(double)];
            for (int i=0; i<doubles.Length; ++i)
            {
                int startIdx = i * sizeof(double);
                byte[] currentBytes = BitConverter.GetBytes(doubles[i]);

                System.Diagnostics.Debug.Assert(currentBytes.Length == sizeof(double));

                for (int j=0; j<sizeof(double); j++)
                {
                    bytes[startIdx + j] = currentBytes[j];
                }
            }

            return (int)System.IO.Hashing.Crc32.HashToUInt32(bytes);
        }
    }
}
