using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.AsyncInitialization;

namespace webapp.Services.Initialization
{
    public class WebAppInitializer : IAsyncInitializer
    {
        private readonly NPMManagerService npmManagerService;

        public WebAppInitializer(NPMManagerService npmManagerService)
        {
            this.npmManagerService = npmManagerService;
        }

        public async Task InitializeAsync()
        {
            // begin long running initialization tasks
            await npmManagerService.InitializePackageVersions();
        }
    }

}
