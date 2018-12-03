using System;
using webapp.Services;
using Xunit;

namespace webapp_tests
{
    public class NPMVersionFinder
    {
        [Fact]
        public void Test1()
        {
            NPMManagerService.GetVersion(); // TO-TEST
            Assert.False(true, "This should be false");
        }
    }
}
