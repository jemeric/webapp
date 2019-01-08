using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using webapp.Services;
using webapp.Util.Dto;

namespace webapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly NPMManagerService npmManagerService;

        public HomeController(NPMManagerService npmManagerService)
        {
            this.npmManagerService = npmManagerService;
        }

        public async Task<IActionResult> Index()
        {
            List<NPMExternal> externals = await npmManagerService.GetExternals();
            List<Task<string>> externalModulePaths = new List<Task<string>>();
            foreach(NPMExternal external in externals)
            {
                foreach(NPMAsset asset in external.Assets)
                {
                    externalModulePaths.Add(npmManagerService.GetNPMModule(external, asset.ProductionPath, asset.DevelopmentPath));
                }
            }
            return View(await Task.WhenAll(externalModulePaths));
        }
    }
}