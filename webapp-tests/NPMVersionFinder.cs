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
        public void TestNPMVersionDetection()
        {
            // from http://registry.npmjs.org/lodash
            JObject registry = JObject.Parse(File.ReadAllText("./Json/lodash-registry.json"));

            // see https://semver.npmjs.com/
            NPMManagerService.GetNPMVersion("1.0.0 - 1.2.0", registry); // TO-TEST
            Assert.Equal("1.2.0", NPMManagerService.GetNPMVersion("1.0.0 - 1.2.0", registry));
            Assert.Equal("1.3.1", NPMManagerService.GetNPMVersion("1.x", registry));
            Assert.Equal("1.0.0-rc.1", NPMManagerService.GetNPMVersion("1.0.0-rc.1", registry));
            Assert.Equal("4.17.11", NPMManagerService.GetNPMVersion(">2.1", registry));
            Assert.Equal("4.17.11", NPMManagerService.GetNPMVersion("latest", registry));
        }
    }
}
