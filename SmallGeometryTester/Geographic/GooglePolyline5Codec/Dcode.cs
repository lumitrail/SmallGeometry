using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmallGeometry.Geographic;

namespace SmallGeometryTester
{
    public class Dcode
    {
        [Fact]
        public void GoogleExample1()
        {
            List<GeoPoint> example = GooglePolyline5Codec.Decode("?`~oia@");

            Assert.Single(example);

            GeoPoint examplePoint = example[0];
            GeoPoint truePoint = new GeoPoint(-179.9832104, 0);

            double distance = truePoint.GetDistanceInMeter(examplePoint);

            Assert.True(distance < 1);
        }

        [Fact]
        public void GoogleExample2()
        {
            List<GeoPoint> example = GooglePolyline5Codec.Decode("_p~iF~ps|U_ulLnnqC_mqNvxq`@");

            var exampleTruth = new List<GeoPoint>()
            {
                new GeoPoint(-120.2, 38.5),
                new GeoPoint(-120.95, 40.7),
                new GeoPoint(-126.453, 43.252)
            };

            Assert.Equal(example.Count(), exampleTruth.Count);

            for (int i=0; i<example.Count(); ++i)
            {
                double distance = exampleTruth[i].GetDistanceInMeter(example[i]);
                Assert.True(distance < 1);
            }
        }
    }
}
