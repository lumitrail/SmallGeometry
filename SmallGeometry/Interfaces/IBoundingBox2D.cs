using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmallGeometry.Interfaces
{
    /// <summary>
    /// 2D bounding box
    /// </summary>
    public interface IBoundingBox2D
    {
        /// <summary>
        /// 
        /// </summary>
        public double Top { get; }
        /// <summary>
        /// 
        /// </summary>
        public double Bottom { get; }
        /// <summary>
        /// 
        /// </summary>
        public double Left { get; }
        /// <summary>
        /// 
        /// </summary>
        public double Right { get; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public double Width { get; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public double Height { get; }
    }
}
