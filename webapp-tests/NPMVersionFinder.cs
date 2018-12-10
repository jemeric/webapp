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
        public void TestSemanticVersionLookup()
        {
            JObject package = JObject.Parse(File.ReadAllText("./Json/package.json"));

            Assert.Equal("^16.4.2", NPMManagerService.GetSemanticVersion("react", package));
            Assert.Equal("4.3.5", NPMManagerService.GetSemanticVersion("react-hot-loader", package));
            Assert.Equal("0.4.3", NPMManagerService.GetSemanticVersion("mini-css-extract-plugin", package));
        }

        [Fact]
        public void TestMaxVersionDetection()
        {
            // from http://registry.npmjs.org/lodash
            JObject registry = JObject.Parse(File.ReadAllText("./Json/lodash-registry.json"));

            // see https://semver.npmjs.com/
            Assert.Equal("1.2.0", NPMManagerService.GetMaxVersion("1.0.0 - 1.2.0", registry));
            Assert.Equal("1.3.1", NPMManagerService.GetMaxVersion("1.x", registry));
            Assert.Equal("1.0.0-rc.1", NPMManagerService.GetMaxVersion("1.0.0-rc.1", registry));
            Assert.Equal("4.17.11", NPMManagerService.GetMaxVersion(">2.1", registry));
            Assert.Equal("4.17.11", NPMManagerService.GetMaxVersion("latest", registry));
        }

    }
}
