﻿using System.Threading.Tasks;
using Abp.Configuration.Startup;
using PearAdmin.AbpTemplate.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace PearAdmin.AbpTemplate.Admin.Views.Shared.Components.RightNavbarUserArea
{
    public class RightNavbarUserAreaViewComponent : AbpTemplateViewComponent
    {
        private readonly ISessionAppService _sessionAppService;
        private readonly IMultiTenancyConfig _multiTenancyConfig;

        public RightNavbarUserAreaViewComponent(
            ISessionAppService sessionAppService,
            IMultiTenancyConfig multiTenancyConfig)
        {
            _sessionAppService = sessionAppService;
            _multiTenancyConfig = multiTenancyConfig;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new RightNavbarUserAreaViewModel
            {
                LoginInformations = await _sessionAppService.GetCurrentLoginInformations(),
                IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled,
            };

            return View(model);
        }
    }
}

