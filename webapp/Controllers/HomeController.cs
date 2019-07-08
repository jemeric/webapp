using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using webapp.Models.Settings.Assets;
using webapp.Services;
using webapp.Services.Assets;
using webapp.Services.Authorization;
using webapp.Util;
using webapp.Util.Dto;
using webapp.Util.Dto.Views;

namespace webapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly NPMManagerService npmManagerService;
        private readonly IAssetsService assetsService;
        private readonly AuthorizationService authorizationService;

        public HomeController(NPMManagerService npmManagerService, IAssetsService assetsService, AuthorizationService authorizationService)
        {
            this.npmManagerService = npmManagerService;
            this.assetsService = assetsService;
            this.authorizationService = authorizationService;
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
            var context = authorizationService.GetAuthorizationContext();
            return View(new AppData(await Task.WhenAll(externalModulePaths), version, context));
        }
    }
}