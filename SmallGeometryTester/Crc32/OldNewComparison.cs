//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SmallGeometryTester
//{
//    public class OldNewComparison
//    {
//        [Fact]
//        public void Comparisons()
//        {
//            var rng = new Random(DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond);
//            double longitude = rng.NextDouble() * 180;
//            double latitude = rng.NextDouble() * 90;

//            if (rng.NextDouble() < 0.5)
//            {
//                longitude = -longitude;
//            }
//            if (rng.NextDouble() < 0.5)
//            {
//                latitude = -latitude;
//            }

//            int oldCrc32 = (int)CRC32.Get([longitude, latitude]);
//            int newCrc32 = SmallGeometry.Crc32.GetCrc32Hash(longitude, latitude);

//            Assert.Equal(oldCrc32, newCrc32);
//        }
//    }

//    /// <summary>
//    /// Performs 32-bit reversed cyclic redundancy checks.
//    /// </summary>
//    internal static class CRC32
//    {
//        #region Constants
//        /// <summary>
//        /// Generator polynomial (modulo 2) for the reversed CRC32 algorithm. 
//        /// </summary>
//        private const UInt32 s_generator = 0xEDB88320;
//        #endregion

//        #region Constructors
//        ///// <summary>
//        ///// Creates a new instance of the Crc32 class.
//        ///// </summary>
//        //public CRC32()
//        //{
//        //}
//        #endregion

//        /// <summary>
//        /// Constructs the checksum lookup table. Used to optimize the checksum.
//        /// </summary>
//        /// <returns></returns>
//        private static UInt32[] ConstructLookupTable()
//        {
//            Console.WriteLine("CRC32 lookup table init");
//            return Enumerable.Range(0, 256).Select(i =>
//            {
//                var tableEntry = (uint)i;
//                for (var j = 0; j < 8; ++j)
//                {
//                    tableEntry = ((tableEntry & 1) != 0)
//                        ? (s_generator ^ (tableEntry >> 1))
//                        : (tableEntry >> 1);
//                }
//                return tableEntry;
//            }).ToArray();
//        }

//        #region Methods
//        public static UInt32 Get(IEnumerable<uint> dataStream)
//        {
//            int ofSize = sizeof(uint);
//            List<byte> bytes = new(dataStream.Count() * ofSize);

//            foreach (uint data in dataStream)
//            {
//                byte[] bt = BitConverter.GetBytes(data);
//                bytes.AddRange(bt);
//            }

//            return Get(bytes);
//        }

//        public static UInt32 Get(IEnumerable<ushort> dataStream)
//        {
//            int ofSize = sizeof(ushort);
//            List<byte> bytes = new(dataStream.Count() * ofSize);

//            foreach (ushort data in dataStream)
//            {
//                byte[] bt = BitConverter.GetBytes(data);
//                bytes.AddRange(bt);
//            }

//            return Get(bytes);
//        }

//        public static UInt32 Get(IEnumerable<float> dataStream)
//        {
//            int ofSize = sizeof(float);
//            List<byte> bytes = new(dataStream.Count() * ofSize);

//            foreach (float data in dataStream)
//            {
//                byte[] bt = BitConverter.GetBytes(data);
//                bytes.AddRange(bt);
//            }

//            return Get(bytes);
//        }

//        public static UInt32 Get(IEnumerable<long> dataStream)
//        {
//            int ofSize = sizeof(long);
//            List<byte> bytes = new(dataStream.Count() * ofSize);

//            foreach (long data in dataStream)
//            {
//                byte[] bt = BitConverter.GetBytes(data);
//                bytes.AddRange(bt);
//            }

//            return Get(bytes);
//        }

//        public static UInt32 Get(IEnumerable<ulong> dataStream)
//        {
//            int ofSize = sizeof(ulong);
//            List<byte> bytes = new(dataStream.Count() * ofSize);

//            foreach (ulong data in dataStream)
//            {
//                byte[] bt = BitConverter.GetBytes(data);
//                bytes.AddRange(bt);
//            }

//            return Get(bytes);
//        }

//        public static UInt32 Get(IEnumerable<short> dataStream)
//        {
//            int ofSize = sizeof(short);
//            List<byte> bytes = new(dataStream.Count() * ofSize);

//            foreach (short data in dataStream)
//            {
//                byte[] bt = BitConverter.GetBytes(data);
//                bytes.AddRange(bt);
//            }

//            return Get(bytes);
//        }

//        public static UInt32 Get(IEnumerable<double> dataStream)
//        {
//            int ofSize = sizeof(double);
//            List<byte> bytes = new(dataStream.Count() * ofSize);

//            foreach (double data in dataStream)
//            {
//                byte[] bt = BitConverter.GetBytes(data);
//                bytes.AddRange(bt);
//            }

//            return Get(bytes);
//        }

//        public static UInt32 Get(IEnumerable<char> dataStream)
//        {
//            int ofSize = sizeof(char);
//            List<byte> bytes = new(dataStream.Count() * ofSize);

//            foreach (char data in dataStream)
//            {
//                byte[] bt = BitConverter.GetBytes(data);
//                bytes.AddRange(bt);
//            }

//            return Get(bytes);
//        }

//        public static UInt32 Get(IEnumerable<bool> dataStream)
//        {
//            int ofSize = sizeof(bool);
//            List<byte> bytes = new(dataStream.Count() * ofSize);

//            foreach (bool data in dataStream)
//            {
//                byte[] bt = BitConverter.GetBytes(data);
//                bytes.AddRange(bt);
//            }

//            return Get(bytes);
//        }

//        public static UInt32 Get(IEnumerable<int> dataStream)
//        {
//            int ofSize = sizeof(int);
//            List<byte> bytes = new(dataStream.Count() * ofSize);

//            foreach (int data in dataStream)
//            {
//                byte[] bt = BitConverter.GetBytes(data);
//                bytes.AddRange(bt);
//            }

//            return Get(bytes);
//        }


//        /// <summary>
//        /// Calculates the checksum of the byte stream.
//        /// </summary>
//        /// <param name="byteStream">The byte stream to calculate the checksum for.</param>
//        /// <returns>A 32-bit reversed checksum.</returns>
//        public static UInt32 Get(IEnumerable<byte> byteStream)
//        {
//            try
//            {
//                // Initialize checksumRegister to 0xFFFFFFFF and calculate the checksum.
//                return ~byteStream.Aggregate(0xFFFFFFFF, (checksumRegister, currentByte) =>
//                          (m_checksumTable[(checksumRegister & 0xFF) ^ Convert.ToByte(currentByte)] ^ (checksumRegister >> 8)));
//            }
//            catch (FormatException e)
//            {
//                throw new Exception("Could not read the stream out as bytes.", e);
//            }
//            catch (InvalidCastException e)
//            {
//                throw new Exception("Could not read the stream out as bytes.", e);
//            }
//            catch (OverflowException e)
//            {
//                throw new Exception("Could not read the stream out as bytes.", e);
//            }
//        }
//        #endregion

//        #region Fields
//        /// <summary>
//        /// Contains a cache of calculated checksum chunks.
//        /// </summary>
//        private static readonly UInt32[] m_checksumTable = ConstructLookupTable();

//        #endregion
//    }
//}
