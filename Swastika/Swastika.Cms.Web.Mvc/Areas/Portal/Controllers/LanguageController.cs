﻿// Licensed to the Swastika I/O Foundation under one or more agreements.
// The Swastika I/O Foundation licenses this file to you under the GNU General Public License v3.0 license.
// See the LICENSE file in the project root for more information.

// Licensed to the Swastika I/O Foundation under one or more agreements.
// The Swastika I/O Foundation licenses this file to you under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swastika.Cms.Lib.Models.Cms;
using Swastika.Cms.Lib.Services;
using Swastika.Cms.Lib.ViewModels.BackEnd;
using Swastika.Cms.Mvc.Controllers;
using Swastika.Domain.Core.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Swastika.Cms.Web.Mvc.Areas.Portal.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    [Area("Portal")]
    [Route("{culture}/Portal/language")]
    public class LanguageController : BaseController
    {
        public LanguageController(IHostingEnvironment env
            )
            : base(env)
        {
        }

        #region Languages

        [HttpGet]
        [Route("")]
        [Route("list")]
        public IActionResult Languages()
        {
            PaginationModel<BELanguageViewModel> pagingPages = new PaginationModel<BELanguageViewModel>()
            {
                Items = GlobalConfigurationService.Instance.CmsCulture.ListLanguage.Where(m => m.Specificulture == CurrentLanguage).ToList(),
                PageIndex = 0,
                PageSize = GlobalConfigurationService.Instance.CmsCulture.ListLanguage.Count(m => m.Specificulture == CurrentLanguage),
                TotalItems = GlobalConfigurationService.Instance.CmsCulture.ListLanguage.Count(m => m.Specificulture == CurrentLanguage),
                TotalPage = 1
            };
            //  await LanguageRepository.GetInstance().GetModelListByAsync(m=> m.Specificulture == _lang,
            //cate => cate.Description, "desc",
            //pageSize, pageIndex, Swastika.Cms.Lib.SWCmsConstants.ViewModelType.FrontEnd);
            return View(pagingPages);
        }

        // GET: Create
        [HttpGet]
        [Route("Create")]
        public IActionResult CreateLanguage()
        {
            BELanguageViewModel ttsLanguage = new BELanguageViewModel(
                new SiocLanguage()
                {
                    //Id = LanguageRepository.GetInstance().GetNextId()
                    Specificulture = CurrentLanguage
                });
            return View(ttsLanguage);
        }

        // POST: Language/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLanguage(BELanguageViewModel language)
        {
            if (ModelState.IsValid)
            {
                var result = await language.SaveModelAsync().ConfigureAwait(false);// BELanguageViewModel.Repository.CreateModelAsync(ttsLanguage);
                if (result.IsSucceed)
                {
                    GlobalConfigurationService.Instance.RefreshCultures();
                    return RedirectToAction("Languages");
                }
                else
                {
                    if (result.Exception != null)
                    {
                        ModelState.AddModelError(string.Empty, result.Exception?.Message);
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }

                    return View(language);
                }
            }
            else
            {
                return View(language);
            }
        }

        // GET: Language/Edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> EditLanguage(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ttsLanguage = await BELanguageViewModel.Repository.GetSingleModelAsync(
                m => m.Keyword == id && m.Specificulture == CurrentLanguage).ConfigureAwait(false);
            if (ttsLanguage == null)
            {
                return NotFound();
            }
            return View(ttsLanguage.Data);
        }

        // POST: Language/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLanguage(string id, BELanguageViewModel beLanguageViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await beLanguageViewModel.SaveModelAsync().ConfigureAwait(false); //_repo.EditModelAsync(ttsLanguage.ParseModel());
                    if (result.IsSucceed)
                    {
                        GlobalConfigurationService.Instance.RefreshCultures();
                    }
                    else
                    {
                        if (result.Exception != null)
                        {
                            ModelState.AddModelError(string.Empty, result.Exception?.Message);
                        }

                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error);
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BELanguageViewModel.Repository.CheckIsExists(c => c.Specificulture == beLanguageViewModel.Specificulture))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Languages");
            }
            return View(beLanguageViewModel);
        }

        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteLanguage(string id)
        {
            var result = await BELanguageViewModel.Repository.RemoveModelAsync(m => m.Keyword == id && m.Specificulture == CurrentLanguage).ConfigureAwait(false);
            if (result.IsSucceed)
            {
                GlobalConfigurationService.Instance.RefreshCultures();
            }
            return RedirectToAction("Languages");
        }

        #endregion Languages
    }
}
