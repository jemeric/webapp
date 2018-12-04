using System;
using System.IO;
using Newtonsoft.Json.Linq;
using webapp.Services;
using Xunit;

namespace webapp_tests
{
    public class NPMVersionFinder
    {
        [Fact]
        public void Test1()
        {
            // from http://registry.npmjs.org/lodash

            JObject registry = JObject.Parse(File.ReadAllText(@"\Json\lodash-registry.json"));

            // see https://semver.npmjs.com/
            NPMManagerService.CalculateSemVer("latest", registry); // TO-TEST
            Assert.False(true, "This should be false");
        }
    }
}
