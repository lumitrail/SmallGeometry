namespace SmallGeometryTester
{
    public class Encode
    {

        [Fact]
        public void GoogleExample1()
        {
            var examplePoint = new GeoPoint(-179.9832104, 0);

            Assert.Equal("?`~oia@", GooglePolyline5Codec.Encode([examplePoint]));
        }

        [Fact]
        public void GoogleExample2()
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
