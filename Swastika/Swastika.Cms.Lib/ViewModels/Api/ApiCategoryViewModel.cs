﻿// Licensed to the Swastika I/O Foundation under one or more agreements.
// The Swastika I/O Foundation licenses this file to you under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swastika.Cms.Lib.Models.Cms;
using Swastika.Cms.Lib.Repositories;
using Swastika.Cms.Lib.Services;
using Swastika.Cms.Lib.ViewModels.FrontEnd;
using Swastika.Cms.Lib.ViewModels.Info;
using Swastika.Cms.Lib.ViewModels.Navigation;
using Swastika.Common.Helper;
using Swastika.Domain.Core.ViewModels;
using Swastika.Domain.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Swastika.Common.Utility.Enums;

namespace Swastika.Cms.Lib.ViewModels.Api
{
    public class ApiCategoryViewModel : ViewModelBase<SiocCmsContext, SiocCategory, ApiCategoryViewModel>
    {
        #region Properties

        #region Models

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("template")]
        public string Template { get; set; }

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("cssClass")]
        public string CssClass { get; set; }

        [JsonProperty("layout")]
        public string Layout { get; set; }

        [Required]
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("excerpt")]
        public string Excerpt { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("seoName")]
        public string SeoName { get; set; }

        [JsonProperty("seoTitle")]
        public string SeoTitle { get; set; }

        [JsonProperty("seoDescription")]
        public string SeoDescription { get; set; }

        [JsonProperty("seoKeywords")]
        public string SeoKeywords { get; set; }

        [JsonProperty("views")]
        public int? Views { get; set; }

        [JsonProperty("type")]
        public SWCmsConstants.CateType Type { get; set; }

        [JsonProperty("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }

        [JsonProperty("updatedDateTime")]
        public DateTime? UpdatedDateTime { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("staticUrl")]
        public string StaticUrl { get; set; }

        [JsonProperty("level")]
        public int? Level { get; set; }

        [JsonProperty("lastModified")]
        public DateTime? LastModified { get; set; }

        [JsonProperty("modifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonProperty("pageSize")]
        public int? PageSize { get; set; }
        #endregion Models

        #region Views

        [JsonProperty("urlAlias")]
        public ApiUrlAliasViewModel UrlAlias { get; set; }

        [JsonProperty("details")]
        public string DetailsUrl { get; set; }

        [JsonProperty("moduleNavs")]
        public List<CategoryModuleViewModel> ModuleNavs { get; set; } // Parent to Modules

        [JsonProperty("positionNavs")]
        public List<CategoryPositionViewModel> PositionNavs { get; set; } // Parent to Modules

        [JsonProperty("parentNavs")]
        public List<NavCategoryCategoryViewModel> ParentNavs { get; set; } // Parent to  Parent

        [JsonProperty("childNavs")]
        public List<NavCategoryCategoryViewModel> ChildNavs { get; set; } // Parent to  Parent

        [JsonProperty("listTag")]
        public JArray ListTag { get; set; } = new JArray();

        [JsonProperty("imageFileStream")]
        public FileStreamViewModel ImageFileStream { get; set; }

        [JsonProperty("domain")]
        public string Domain { get { return GlobalConfigurationService.Instance.GetLocalString("Domain", Specificulture, "/"); } }
        [JsonProperty("imageUrl")]
        public string ImageUrl
        {
            get
            {
                if (Image != null && (Image.IndexOf("http") == -1 && Image[0] != '/'))
                {
                    return SwCmsHelper.GetFullPath(new string[] {
                    Domain,  Image
                });
                }
                else
                {
                    return Image;
                }
            }
        }

        [JsonProperty("thumbnailUrl")]
        public string ThumbnailUrl
        {
            get
            {
                if (Thumbnail != null && Thumbnail.IndexOf("http") == -1 && Thumbnail[0] != '/')
                {
                    return SwCmsHelper.GetFullPath(new string[] {
                    Domain,  Thumbnail
                });
                }
                else
                {
                    return Thumbnail;
                }
            }
        }

        #region Template

        [JsonProperty("view")]
        public ApiTemplateViewModel View { get; set; }

        [JsonProperty("templates")]
        public List<ApiTemplateViewModel> Templates { get; set; }// Article Templates

        [JsonIgnore]
        public string ActivedTemplate
        {
            get
            {
                return GlobalConfigurationService.Instance.GetLocalString(SWCmsConstants.ConfigurationKeyword.Theme, Specificulture, SWCmsConstants.Default.DefaultTemplateFolder);
            }
        }

        [JsonIgnore]
        public string TemplateFolderType
        {
            get
            {
                return SWCmsConstants.EnumTemplateFolder.Pages.ToString();
            }
        }

        [JsonProperty("templateFolder")]
        public string TemplateFolder
        {
            get
            {
                return SwCmsHelper.GetFullPath(new string[]
                {
                    SWCmsConstants.Parameters.TemplatesFolder
                    , ActivedTemplate
                    , TemplateFolderType
                }
            );
            }
        }

        #endregion Template

        #endregion Views

        #endregion Properties

        #region Contructors

        public ApiCategoryViewModel() : base()
        {
        }

        public ApiCategoryViewModel(SiocCategory model, SiocCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
        {
        }

        #endregion Contructors

        #region Overrides

        public override SiocCategory ParseModel(SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            GenerateSEO();

            var navParent = ParentNavs?.FirstOrDefault(p => p.IsActived);

            if (navParent != null)
            {
                Level = InfoCategoryViewModel.Repository.GetSingleModel(c => c.Id == navParent.ParentId, _context, _transaction).Data.Level + 1;
            }
            else
            {
                Level = 0;
            }

            Template = View != null ? string.Format(@"{0}/{1}{2}", View.FolderType, View.FileName, View.Extension) : Template;
            if (Id == 0)
            {
                Id = FECategoryViewModel.Repository.Max(c => c.Id, _context, _transaction).Data + 1;
                CreatedDateTime = DateTime.UtcNow;
            }
            return base.ParseModel(_context, _transaction);
        }

        public override void ExpandView(SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            var getAlias = ApiUrlAliasViewModel.Repository.GetSingleModel(
                u => u.Specificulture == Specificulture && u.SourceId == Id.ToString() && u.Type == (int)SWCmsConstants.UrlAliasType.Page);
            UrlAlias = getAlias.Data;
            if (UrlAlias == null)
            {
                UrlAlias = new ApiUrlAliasViewModel()
                {
                    Type = SWCmsConstants.UrlAliasType.Page,
                    Specificulture = Specificulture,
                    Alias = SeoName
                };
            }
            Cultures = Cultures ?? CommonRepository.Instance.LoadCultures(Specificulture, _context, _transaction);
            Cultures.ForEach(c => c.IsSupported = _context.SiocCategory.Any(m => m.Id == Id && m.Specificulture == c.Specificulture));
            if (!string.IsNullOrEmpty(this.Tags))
            {
                ListTag = JArray.Parse(this.Tags);
            }

            int themeId = GlobalConfigurationService.Instance.GetLocalInt(SWCmsConstants.ConfigurationKeyword.ThemeId, Specificulture, 0);
            View = ApiTemplateViewModel.GetTemplateByPath(themeId, Template, SWCmsConstants.TemplateFolder.Pages, _context, _transaction);

            if (this.View == null)
            {
                this.View = new ApiTemplateViewModel(new SiocTemplate()
                {
                    Extension = SWCmsConstants.Parameters.TemplateExtension,
                    TemplateId = GlobalConfigurationService.Instance.GetLocalInt(SWCmsConstants.ConfigurationKeyword.ThemeId, Specificulture, 0),
                    TemplateName = ActivedTemplate,
                    FolderType = TemplateFolderType,
                    FileFolder = this.TemplateFolder,
                    FileName = SWCmsConstants.Default.DefaultTemplate,
                    ModifiedBy = ModifiedBy,
                    Content = "<div></div>"
                });
            }
            this.Template = SwCmsHelper.GetFullPath(new string[]
               {
                    this.View?.FileFolder
                    , this.View?.FileName
               });

            this.ModuleNavs = GetModuleNavs(_context, _transaction);
            this.ParentNavs = GetParentNavs(_context, _transaction);
            this.ChildNavs = GetChildNavs(_context, _transaction);
            this.PositionNavs = GetPositionNavs(_context, _transaction);
        }

        #region Sync

        public override RepositoryResponse<bool> SaveSubModels(SiocCategory parent, SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            var result = new RepositoryResponse<bool> { IsSucceed = true };
            var saveTemplate = View.SaveModel(true, _context, _transaction);
            result.IsSucceed = result.IsSucceed && saveTemplate.IsSucceed;
            if (saveTemplate.IsSucceed)
            {
                result.Errors.AddRange(saveTemplate.Errors);
                result.Exception = saveTemplate.Exception;
            }
            // Save url alias
            if (result.IsSucceed)
            {
                UrlAlias.IsClone = IsClone;
                UrlAlias.Cultures = Cultures;
                UrlAlias.Type = SWCmsConstants.UrlAliasType.Page;
                UrlAlias.SourceId = parent.Id.ToString();
                var saveUrl = UrlAlias.SaveModel(false, _context, _transaction);
                result.Errors.AddRange(saveUrl.Errors);
                result.Exception = saveUrl.Exception;
                result.IsSucceed = saveUrl.IsSucceed;
            }

            if (result.IsSucceed)
            {
                foreach (var item in ModuleNavs)
                {
                    item.CategoryId = parent.Id;
                    if (item.IsActived)
                    {
                        var saveResult = item.SaveModel(false, _context, _transaction);
                        result.IsSucceed = saveResult.IsSucceed;
                        if (!result.IsSucceed)
                        {
                            result.Exception = saveResult.Exception;
                            Errors.AddRange(saveResult.Errors);
                        }
                    }
                    else
                    {
                        var saveResult = item.RemoveModel(false, _context, _transaction);
                        result.IsSucceed = saveResult.IsSucceed;
                        if (!result.IsSucceed)
                        {
                            result.Exception = saveResult.Exception;
                            Errors.AddRange(saveResult.Errors);
                        }
                    }
                }
            }

            if (result.IsSucceed)
            {
                foreach (var item in PositionNavs)
                {
                    item.CategoryId = parent.Id;
                    if (item.IsActived)
                    {
                        var saveResult = item.SaveModel(false, _context, _transaction);
                        result.IsSucceed = saveResult.IsSucceed;
                        if (!result.IsSucceed)
                        {
                            result.Exception = saveResult.Exception;
                            Errors.AddRange(saveResult.Errors);
                        }
                    }
                    else
                    {
                        var saveResult = item.RemoveModel(false, _context, _transaction);
                        result.IsSucceed = saveResult.IsSucceed;
                        if (!result.IsSucceed)
                        {
                            result.Exception = saveResult.Exception;
                            Errors.AddRange(saveResult.Errors);
                        }
                    }
                }
            }

            if (result.IsSucceed)
            {
                foreach (var item in ParentNavs)
                {
                    item.Id = parent.Id;
                    if (item.IsActived)
                    {
                        var saveResult = item.SaveModel(false, _context, _transaction);
                        result.IsSucceed = saveResult.IsSucceed;
                        if (!result.IsSucceed)
                        {
                            result.Exception = saveResult.Exception;
                            Errors.AddRange(saveResult.Errors);
                        }
                    }
                    else
                    {
                        var saveResult = item.RemoveModel(false, _context, _transaction);
                        result.IsSucceed = saveResult.IsSucceed;
                        if (!result.IsSucceed)
                        {
                            result.Exception = saveResult.Exception;
                            Errors.AddRange(saveResult.Errors);
                        }
                    }
                }
            }

            if (result.IsSucceed)
            {
                foreach (var item in ChildNavs)
                {
                    item.ParentId = parent.Id;
                    if (item.IsActived)
                    {
                        var saveResult = item.SaveModel(false, _context, _transaction);
                        result.IsSucceed = saveResult.IsSucceed;
                        if (!result.IsSucceed)
                        {
                            result.Exception = saveResult.Exception;
                            Errors.AddRange(saveResult.Errors);
                        }
                    }
                    else
                    {
                        var saveResult = item.RemoveModel(false, _context, _transaction);
                        result.IsSucceed = saveResult.IsSucceed;
                        if (!result.IsSucceed)
                        {
                            result.Exception = saveResult.Exception;
                            Errors.AddRange(saveResult.Errors);
                        }
                    }
                }
            }
            return result;
        }

        #endregion Sync

        #region Async

        public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(SiocCategory parent, SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            var result = new RepositoryResponse<bool> { IsSucceed = true };
            var saveTemplate = await View.SaveModelAsync(true, _context, _transaction);

            result.IsSucceed = result.IsSucceed && saveTemplate.IsSucceed;
            if (saveTemplate.IsSucceed)
            {
                result.Errors.AddRange(saveTemplate.Errors);
                result.Exception = saveTemplate.Exception;
            }

            // Save url alias
            if (result.IsSucceed)
            {
                UrlAlias.IsClone = IsClone;
                UrlAlias.Cultures = Cultures;
                UrlAlias.Type = SWCmsConstants.UrlAliasType.Page;
                UrlAlias.SourceId = parent.Id.ToString();
                var saveUrl = await UrlAlias.SaveModelAsync(false, _context, _transaction);
                result.Errors.AddRange(saveUrl.Errors);
                result.Exception = saveUrl.Exception;
                result.IsSucceed = saveUrl.IsSucceed;
            }

            if (result.IsSucceed)
            {
                foreach (var item in ModuleNavs)
                {
                    item.CategoryId = parent.Id;

                    if (item.IsActived)
                    {
                        var saveResult = await item.SaveModelAsync(false, _context, _transaction);
                        result.IsSucceed = saveResult.IsSucceed;
                        if (!result.IsSucceed)
                        {
                            result.Exception = saveResult.Exception;
                            Errors.AddRange(saveResult.Errors);
                        }
                    }
                    else
                    {
                        var saveResult = await item.RemoveModelAsync(false, _context, _transaction);
                        result.IsSucceed = saveResult.IsSucceed;
                        if (!result.IsSucceed)
                        {
                            result.Exception = saveResult.Exception;
                            Errors.AddRange(saveResult.Errors);
                        }
                    }
                }
            }

            if (result.IsSucceed)
            {
                foreach (var item in PositionNavs)
                {
                    item.CategoryId = parent.Id;
                    if (item.IsActived)
                    {
                        var saveResult = await item.SaveModelAsync(false, _context, _transaction);
                        result.IsSucceed = saveResult.IsSucceed;
                        if (!result.IsSucceed)
                        {
                            result.Exception = saveResult.Exception;
                            Errors.AddRange(saveResult.Errors);
                        }
                    }
                    else
                    {
                        var saveResult = await item.RemoveModelAsync(false, _context, _transaction);
                        result.IsSucceed = saveResult.IsSucceed;
                        if (!result.IsSucceed)
                        {
                            result.Exception = saveResult.Exception;
                            Errors.AddRange(saveResult.Errors);
                        }
                    }
                }
            }

            if (result.IsSucceed)
            {
                foreach (var item in ParentNavs)
                {
                    item.Id = parent.Id;
                    if (item.IsActived)
                    {
                        var saveResult = await item.SaveModelAsync(false, _context, _transaction);
                        result.IsSucceed = saveResult.IsSucceed;
                        if (!result.IsSucceed)
                        {
                            result.Exception = saveResult.Exception;
                            Errors.AddRange(saveResult.Errors);
                        }
                    }
                    else
                    {
                        var saveResult = await item.RemoveModelAsync(false, _context, _transaction);
                        result.IsSucceed = saveResult.IsSucceed;
                        if (!result.IsSucceed)
                        {
                            result.Exception = saveResult.Exception;
                            Errors.AddRange(saveResult.Errors);
                        }
                    }
                }
            }

            if (result.IsSucceed)
            {
                foreach (var item in ChildNavs)
                {
                    item.ParentId = parent.Id;
                    if (item.IsActived)
                    {
                        var saveResult = await item.SaveModelAsync(false, _context, _transaction);
                        result.IsSucceed = saveResult.IsSucceed;
                        if (!result.IsSucceed)
                        {
                            result.Exception = saveResult.Exception;
                            Errors.AddRange(saveResult.Errors);
                        }
                    }
                    else
                    {
                        var saveResult = await item.RemoveModelAsync(false, _context, _transaction);
                        result.IsSucceed = saveResult.IsSucceed;
                        if (!result.IsSucceed)
                        {
                            result.Exception = saveResult.Exception;
                            Errors.AddRange(saveResult.Errors);
                        }
                    }
                }
            }
            return result;
        }

        #endregion Async

        #endregion Overrides

        #region Expands

        private void GenerateSEO()
        {
            if (string.IsNullOrEmpty(this.SeoName))
            {
                this.SeoName = SeoHelper.GetSEOString(this.Title);
            }
            int i = 1;
            string name = SeoName;
            while (Repository.CheckIsExists(a => a.SeoName == name && a.Specificulture == Specificulture && a.Id != Id))
            {
                name = SeoName + "_" + i;
                i++;
            }
            SeoName = name;

            if (string.IsNullOrEmpty(this.SeoTitle))
            {
                this.SeoTitle = SeoHelper.GetSEOString(this.Title);
            }

            if (string.IsNullOrEmpty(this.SeoDescription))
            {
                this.SeoDescription = SeoHelper.GetSEOString(this.Title);
            }

            if (string.IsNullOrEmpty(this.SeoKeywords))
            {
                this.SeoKeywords = SeoHelper.GetSEOString(this.Title);
            }
        }

        public List<CategoryPositionViewModel> GetPositionNavs(SiocCmsContext context, IDbContextTransaction transaction)
        {
            var query = context.SiocPosition
                  .Include(cp => cp.SiocCategoryPosition)
                  //.Where(p => p.Specificulture == Specificulture)
                  .Select(p => new CategoryPositionViewModel()
                  {
                      CategoryId = Id,
                      PositionId = p.Id,
                      Specificulture = Specificulture,
                      Description = p.Description,
                      IsActived = context.SiocCategoryPosition.Count(m => m.CategoryId == Id && m.PositionId == p.Id && m.Specificulture == Specificulture) > 0
                  });

            return query.OrderBy(m => m.Priority).ToList();
        }

        public List<CategoryModuleViewModel> GetModuleNavs(SiocCmsContext context, IDbContextTransaction transaction)
        {
            var query = context.SiocModule
                .Include(cp => cp.SiocCategoryModule)
                .Where(module => module.Status == (int)SWStatus.Published && module.Specificulture == Specificulture)
                .Select(module => new CategoryModuleViewModel()
                {
                    CategoryId = Id,
                    ModuleId = module.Id,
                    Specificulture = Specificulture,
                    Description = module.Title,
                    Image = module.Image
                });

            var result = query.ToList();
            result.ForEach(nav =>
            {
                var currentNav = context.SiocCategoryModule.FirstOrDefault(
                        m => m.ModuleId == nav.ModuleId && m.CategoryId == Id && m.Specificulture == Specificulture);
                nav.Priority = currentNav?.Priority;
                nav.IsActived = currentNav != null;
            });
            return result.OrderBy(m => m.Priority).ToList();
        }

        public List<NavCategoryCategoryViewModel> GetParentNavs(SiocCmsContext context, IDbContextTransaction transaction)
        {
            var query = context.SiocCategory
                .Include(cp => cp.SiocCategoryCategorySiocCategory)
                .Where(Category => Category.Status == (int)SWStatus.Published && Category.Specificulture == Specificulture && Category.Id != Id)
                .Select(Category =>
                    new NavCategoryCategoryViewModel()
                    {
                        Id = Id,
                        ParentId = Category.Id,
                        Specificulture = Specificulture,
                        Description = Category.Title,
                    }
                );

            var result = query.ToList();
            result.ForEach(nav =>
            {
                nav.IsActived = context.SiocCategoryCategory.Any(
                        m => m.ParentId == nav.ParentId && m.Id == Id && m.Specificulture == Specificulture);
            });
            return result.OrderBy(m => m.Priority).ToList();
        }

        public List<NavCategoryCategoryViewModel> GetChildNavs(SiocCmsContext context, IDbContextTransaction transaction)
        {
            // Get other category
            var query = context.SiocCategory
                .Include(cp => cp.SiocCategoryCategorySiocCategory)
                .Where(Category => Category.Status == (int)SWStatus.Published && Category.Specificulture == Specificulture && Category.Id != Id)
                .Select(Category =>
                new NavCategoryCategoryViewModel(
                      new SiocCategoryCategory()
                      {
                          Id = Category.Id,
                          ParentId = Id,
                          Specificulture = Specificulture,
                          Description = Category.Title,
                      }, context, transaction));

            var result = query.ToList();
            result.ForEach(nav =>
            {
                var currentNav = context.SiocCategoryCategory.FirstOrDefault(
                        m => m.ParentId == Id && m.Id == nav.Id && m.Specificulture == Specificulture);
                nav.Priority = currentNav?.Priority;
                nav.IsActived = currentNav != null;
            });
            return result.OrderBy(m => m.Priority).ToList();
        }

        #endregion Expands
    }
}
