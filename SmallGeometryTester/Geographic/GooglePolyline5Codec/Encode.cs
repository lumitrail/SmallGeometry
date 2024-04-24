using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmallGeometry.Geographic;

namespace SmallGeometryTester
{
    public class Encode
    {
        [Fact]
        public void Test1()
        {
            var example = new List<GeoPoint>()
            {
                new GeoPoint(-120.2, 38.5),
                new GeoPoint(-120.95, 40.7),
                new GeoPoint(-126.453, 43.252)
            };

            Assert.Equal("_p~iF~ps|U_ulLnnqC_mqNvxq`@", GooglePolyline5Codec.Encode(example));
        }
    }
}
