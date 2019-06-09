using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using webapp.Models.Settings.Assets;
using webapp.Services;
using webapp.Services.Assets;
using webapp.Util;
using webapp.Util.Dto;
using webapp.Util.Dto.Views;

namespace webapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly NPMManagerService npmManagerService;
        private readonly IAssetsService assetsService;

        public HomeController(NPMManagerService npmManagerService, IAssetsService assetsService)
        {
            this.npmManagerService = npmManagerService;
            this.assetsService = assetsService;
        }

        public async Task<IActionResult> Index()
        {
            Task<List<NPMExternal>> externals = npmManagerService.GetExternals();
            Task<AssetVersion> publishedVersionTask = assetsService.GetPublishedVersion();
            await Task.WhenAll(externals, publishedVersionTask);

            List<Task<string>> externalModulePaths = new List<Task<string>>();
            foreach(NPMExternal external in externals.Result)
            {
                externalModulePaths.AddRange(external.Assets.Select(asset => npmManagerService.GetNPMModule(external, asset.ProductionPath, asset.DevelopmentPath)));
            }

            string version = publishedVersionTask.Result != null ? publishedVersionTask.Result.Version : AppConstants.Assets.defaultAssetVersion;
            return View(new AppData(await Task.WhenAll(externalModulePaths), version));
        }
    }
}