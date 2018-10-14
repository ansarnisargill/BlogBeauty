﻿// Licensed to the Swastika I/O Foundation under one or more agreements.
// The Swastika I/O Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Swastika.Cms.Lib;
using Swastika.Cms.Lib.Models.Cms;
using Swastika.Cms.Lib.Repositories;
using Swastika.Cms.Lib.Services;
using Swastika.Cms.Lib.ViewModels;
using Swastika.Cms.Lib.ViewModels.Account;
using Swastika.Cms.Lib.ViewModels.Api;
using Swastika.Cms.Lib.ViewModels.Info;
using Swastika.Domain.Core.ViewModels;
using Swastika.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Swastika.Common.Utility.Enums;

namespace Swastka.Cms.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/portal")]
    public class ApiPortalController :
        BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ApiPortalController(
           UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           RoleManager<IdentityRole> roleManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        #region Get

        // GET api/category/id
        [HttpGet, HttpOptions]
        [Route("{culture}/settings")]
        [Route("settings")]
        public RepositoryResponse<SiteSettingsViewModel> Settings()
        {
            var cultures = CommonRepository.Instance.LoadCultures();
            var culture = cultures.FirstOrDefault(c => c.Specificulture == _lang);
            SiteSettingsViewModel settings = new SiteSettingsViewModel()
            {
                Lang = _lang,
                ThemeId = GlobalConfigurationService.Instance.GetLocalInt(SWCmsConstants.ConfigurationKeyword.ThemeId, _lang),
                Themes = InfoThemeViewModel.Repository.GetModelList().Data,
                Cultures = cultures,
                PageTypes = Enum.GetNames(typeof(SWCmsConstants.CateType)).ToList(),
                Statuses = Enum.GetNames(typeof(SWStatus)).ToList()

            };
            settings.LangIcon = culture?.Icon ?? GlobalConfigurationService.Instance.CmsConfigurations.Language;
            return new RepositoryResponse<SiteSettingsViewModel>()
            {
                IsSucceed = true,
                Data = settings
            };
        }
        
        // GET api/category/id
        [HttpGet, HttpOptions]
        [Route("{culture}/all-settings")]
        [Route("all-settings")]
        public async Task<RepositoryResponse<JObject>> AllSettingsAsync()
        {
            var cultures = CommonRepository.Instance.LoadCultures();
            var culture = cultures.FirstOrDefault(c => c.Specificulture == _lang);

            // Get Settings
            SiteSettingsViewModel settings = new SiteSettingsViewModel()
            {
                Lang = _lang,
                ThemeId = GlobalConfigurationService.Instance.GetLocalInt(SWCmsConstants.ConfigurationKeyword.ThemeId, _lang),
                Themes = InfoThemeViewModel.Repository.GetModelList().Data,
                Cultures = cultures,
                PageTypes = Enum.GetNames(typeof(SWCmsConstants.CateType)).ToList(),
                Statuses = Enum.GetNames(typeof(SWStatus)).ToList()

            };

            settings.LangIcon = culture?.Icon ?? GlobalConfigurationService.Instance.CmsConfigurations.Language;

            // Get translator
            var translator = GlobalConfigurationService.Instance.CmsCulture.Translator[_lang]?.ToObject<JObject>();

            // Get Configurations
            var configurations = await ApiConfigurationViewModel.Repository.GetModelListByAsync(a => a.Category == "Site" && a.Specificulture == _lang);
            JObject objConfigurations = new JObject();
            foreach (var cnf in configurations.Data)
            {
                JProperty l = new JProperty(cnf.Keyword, cnf.Value);
                objConfigurations.Add(l);
            }

            JObject result = new JObject()
            {
                new JProperty("settings", JObject.FromObject(settings)),
                new JProperty("translator", translator),
                new JProperty("configurations", JObject.FromObject(configurations))
            };

            return new RepositoryResponse<JObject>()
            {
                IsSucceed = true,
                Data = result
            };
        }

        // GET api/category/id
        [HttpGet, HttpOptions]
        [Route("{culture}/translator")]
        [Route("translator")]
        public RepositoryResponse<JObject> Languages()
        {
            return new RepositoryResponse<JObject>()
            {
                IsSucceed = true,
                Data = GlobalConfigurationService.Instance.CmsCulture.Translator[_lang]?.ToObject<JObject>()
            };
        }

        // GET api/configurations/id
        [HttpGet, HttpOptions]
        [Route("{culture}/configurations")]
        [Route("configurations")]
        public async Task<RepositoryResponse<JObject>> GetSiteConfigurations()
        {
            var result = await ApiConfigurationViewModel.Repository.GetModelListByAsync(a => a.Category == "Site" && a.Specificulture == _lang);
            JObject objResult = new JObject();
            foreach (var cnf in result.Data)
            {
                JProperty l = new JProperty(cnf.Keyword, cnf.Value);
                objResult.Add(l);
            }
            return new RepositoryResponse<JObject>()
            {
                IsSucceed = true,
                Data = objResult
            };
        }

        [HttpGet, HttpOptions]
        [Route("init-settings")]
        public RepositoryResponse<SiteSettingsViewModel> InitSettings()
        {
            SiteSettingsViewModel settings = new SiteSettingsViewModel()
            {
                Lang = _lang,
                ThemeId = 1,
                Cultures = new List<Swastika.Domain.Core.Models.SupportedCulture>()
                {
                    new Swastika.Domain.Core.Models.SupportedCulture()
                    {
                        Specificulture = "en-us",
                        FullName = "United States",
                        Description = "United States",
                        Icon = "flag-icon-us",
                        Alias = "US"
                    },
                    new Swastika.Domain.Core.Models.SupportedCulture()
                    {
                        Specificulture = "vi-vn",
                        FullName = "Vietnam",
                        Description = "Việt Nam",
                        Icon = "flag-icon-vn",
                        Alias = "VN"
                    }

                },
                PageTypes = Enum.GetNames(typeof(SWCmsConstants.CateType)).ToList()
            };
            return new RepositoryResponse<SiteSettingsViewModel>()
            {
                IsSucceed = true,
                Data = settings
            };
        }

        // GET api/category/id
        [HttpGet, HttpOptions]
        [Route("dashboard")]
        public RepositoryResponse<DashboardViewModel> Dashboard(int id)
        {
            return new RepositoryResponse<DashboardViewModel>()
            {
                IsSucceed = true,
                Data = new DashboardViewModel()
            };
        }

        #endregion Get

        #region Post

        // GET 
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SuperAdmin, Admin")]
        [HttpGet, HttpOptions]
        [Route("app-settings/details")]
        public RepositoryResponse<JObject> LoadAppSettings()
        {
            var settings = FileRepository.Instance.GetFile("appsettings", ".json", string.Empty, true, "{}");
            JObject jsonSettings = null;
            if (settings != null)
            {
                jsonSettings = JObject.Parse(settings.Content);
            }
            return new RepositoryResponse<JObject>() { IsSucceed = jsonSettings != null, Data = jsonSettings };
        }

        // POST api/category
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SuperAdmin, Admin")]
        [HttpPost, HttpOptions]
        [Route("app-settings/save")]
        public RepositoryResponse<JObject> SaveAppSettings([FromBody]JObject model)
        {
            var settings = FileRepository.Instance.GetFile("appsettings", ".json", string.Empty, true, "{}");
            if (model != null)
            {
                settings.Content = model.ToString();
                FileRepository.Instance.SaveFile(settings);
                GlobalConfigurationService.Instance.RefreshConfigurations();
            }
            return new RepositoryResponse<JObject>() { IsSucceed = model != null, Data = model };
        }

        // POST api/category
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SuperAdmin, Admin")]
        [HttpPost, HttpOptions]
        [Route("import/{type}")]
        [Route("{culture}/import/{type}")]
        public async Task<RepositoryResponse<bool>> SaveAppSettingsAsync(string type, [FromBody]FileViewModel model)
        {
            var result = FileRepository.Instance.SaveWebFile(model);
            if (result)
            {
                var fileContent = FileRepository.Instance.GetWebFile($"{model.Filename}{model.Extension}", model.FileFolder);
                var obj = JObject.Parse(fileContent.Content);
                switch (type)
                {
                    case "Language":
                        var arrLanguage = obj["data"].ToObject<List<SiocLanguage>>();
                        return await InfoLanguageViewModel.ImportLanguages(arrLanguage, _lang);
                    case "Configuration":
                        var arrConfiguration = obj["data"].ToObject<List<SiocConfiguration>>();
                        return await InfoConfigurationViewModel.ImportConfigurations(arrConfiguration, _lang);

                    default:
                        return new RepositoryResponse<bool>() { IsSucceed = false };
                }
            }
            return new RepositoryResponse<bool>();

        }

        // POST api/category
        [HttpPost, HttpOptions]
        [Route("init-cms")]
        public async Task<RepositoryResponse<bool>> Post([FromBody]ApiInitCmsViewModel model)
        {
            if (model != null)
            {
                var result = await InitCmsAsync(model).ConfigureAwait(false);
                if (result.IsSucceed)
                {
                    GlobalConfigurationService.Instance.RefreshAll();
                }
                return result;
            }
            return new RepositoryResponse<bool>();
        }

        #endregion Post

        #region Helpers
        private async Task<RepositoryResponse<bool>> InitCmsAsync(ApiInitCmsViewModel model)
        {
            var result = new RepositoryResponse<bool>();

            var settings = FileRepository.Instance.GetFile("appsettings", ".json", string.Empty, true, "{}");
            if (settings != null)
            {
                JObject jsonSettings = JObject.Parse(settings.Content);
                jsonSettings["ConnectionStrings"][SWCmsConstants.CONST_DEFAULT_CONNECTION] = model.ConnectionString;
                jsonSettings["ConnectionStrings"]["AccountConnection"] = model.ConnectionString;
                jsonSettings["IsSqlite"] = false;
                jsonSettings["Language"] = model.Culture.Specificulture;
                settings.Content = jsonSettings.ToString();
                FileRepository.Instance.SaveFile(settings);
            }

            GlobalConfigurationService.Instance.CmsConfigurations = new CmsConfiguration();
            var initResult = await GlobalConfigurationService.Instance.InitSWCms(model.Culture);
            if (initResult.IsSucceed)
            {
                await InitRolesAsync();
                result.IsSucceed = true;
                GlobalConfigurationService.Instance.RefreshConfigurations();
            }
            else
            {
                settings = FileRepository.Instance.GetFile("appsettings", ".json", string.Empty);
                JObject jsonSettings = JObject.Parse(settings.Content);
                jsonSettings["ConnectionStrings"][SWCmsConstants.CONST_DEFAULT_CONNECTION] = null;
                jsonSettings["ConnectionStrings"]["AccountConnection"] = null;
                jsonSettings["IsSqlite"] = false;
                jsonSettings["Language"] = "en-us";
                settings.Content = jsonSettings.ToString();
                FileRepository.Instance.SaveFile(settings);
                if (initResult.Exception != null)
                {
                    result.Errors.Add(initResult.Exception.Message);
                }
                foreach (var item in initResult.Errors)
                {
                    result.Errors.Add(item);
                }
            }
            return result;
        }

        private async Task<bool> InitRolesAsync()
        {
            bool isSucceed = true;
            var getRoles = await RoleViewModel.Repository.GetModelListAsync();
            if (getRoles.IsSucceed && getRoles.Data.Count == 0)
            {
                var saveResult = await _roleManager.CreateAsync(new IdentityRole()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "SuperAdmin"
                });
                isSucceed = saveResult.Succeeded;
            }
            return isSucceed;
        }

        #endregion
    }
}
