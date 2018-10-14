﻿// Licensed to the Swastika I/O Foundation under one or more agreements.
// The Swastika I/O Foundation licenses this file to you under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Swastika.Cms.Lib.Models.Cms;
using Swastika.Cms.Lib.Services;
using Swastika.Cms.Lib.ViewModels.Info;
using Swastika.Cms.Lib.ViewModels.Navigation;
using Swastika.Common.Helper;
using Swastika.Domain.Core.ViewModels;
using Swastika.Domain.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swastika.Cms.Lib.ViewModels.Info
{
    public class InfoCategoryViewModel
       : ViewModelBase<SiocCmsContext, SiocCategory, InfoCategoryViewModel>
    {
        #region Properties

        #region Models

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("layout")]
        public string Layout { get; set; }

        [JsonProperty("template")]
        public string Template { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("fields")]
        public string Fields { get; set; }

        [JsonProperty("type")]
        public SWCmsConstants.CateType Type { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("cssClass")]
        public string CssClass { get; set; }

        [JsonProperty("staticUrl")]
        public string StaticUrl { get; set; }

        [JsonProperty("excerpt")]
        public string Excerpt { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("seoName")]
        public string SeoName { get; set; }

        [JsonProperty("seoDescription")]
        public string SeoDescription { get; set; }

        [JsonProperty("seoKeywords")]
        public string SeoKeywords { get; set; }

        [JsonProperty("seoTitle")]
        public string SeoTitle { get; set; }

        [JsonProperty("level")]
        public int? Level { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("lastModified")]
        public DateTime LastModified { get; set; }

        [JsonProperty("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }

        [JsonProperty("updatedDateTime")]
        public DateTime? UpdatedDateTime { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("updatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty("modifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        #endregion Models

        #region Views

        [JsonProperty("urlAlias")]
        public InfoUrlAliasViewModel UrlAlias { get; set; }

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

        [JsonProperty("childs")]
        public List<InfoCategoryViewModel> Childs { get; set; }

        [JsonProperty("totalArticle")]
        public int TotalArticle { get; set; }

        [JsonProperty("totalProduct")]
        public int TotalProduct { get; set; }

        [JsonProperty("isActived")]
        public bool IsActived { get; set; }

        [JsonProperty("detailsUrl")]
        public string DetailsUrl { get; set; }

        #endregion Views

        #endregion Properties

        #region Contructors

        public InfoCategoryViewModel() : base()
        {
        }

        public InfoCategoryViewModel(SiocCategory model, SiocCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
        {
        }

        #endregion Contructors

        #region Overrides

        public override void ExpandView(SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            var getAlias = InfoUrlAliasViewModel.Repository.GetSingleModel(u => u.Specificulture == Specificulture && u.SourceId == Id.ToString()
            , _context, _transaction);
            UrlAlias = getAlias.Data;
            if (UrlAlias == null)
            {
                UrlAlias = new InfoUrlAliasViewModel()
                {
                    Specificulture = Specificulture,
                    Alias = SeoName
                };
            }
            var getChilds = Repository.GetModelListBy
                (p => p.SiocCategoryCategorySiocCategory.Any(c => c.ParentId == Id
                && c.Specificulture == Specificulture)
                );
            if (getChilds.IsSucceed)
            {
                Childs = getChilds.Data;
            }
            var countArticle = NavCategoryArticleViewModel.Repository.Count(c => c.CategoryId == Id && c.Specificulture == Specificulture
                , _context: _context, _transaction: _transaction);

            if (countArticle.IsSucceed)
            {
                TotalArticle = countArticle.Data;
            }

            var countProduct = NavCategoryProductViewModel.Repository.Count(c => c.CategoryId == Id && c.Specificulture == Specificulture
               , _context: _context, _transaction: _transaction);

            if (countProduct.IsSucceed)
            {
                TotalProduct = countProduct.Data;
            }
        }

        #endregion Overrides

        #region Expands

        public static async System.Threading.Tasks.Task<RepositoryResponse<List<InfoCategoryViewModel>>> UpdateInfosAsync(List<InfoCategoryViewModel> cates)
        {
            SiocCmsContext context = new SiocCmsContext();
            var transaction = context.Database.BeginTransaction();
            var result = new RepositoryResponse<List<InfoCategoryViewModel>>();
            try
            {
                
                foreach (var item in cates)
                {
                    item.LastModified = DateTime.UtcNow;
                    var saveResult = await item.SaveModelAsync(false, context, transaction);
                    result.IsSucceed = saveResult.IsSucceed;
                    if (!result.IsSucceed)
                    {
                        result.Errors.AddRange(saveResult.Errors);
                        result.Exception = saveResult.Exception;
                        break;
                    }
                }
                UnitOfWorkHelper<SiocCmsContext>.HandleTransaction(result.IsSucceed, true, transaction);
                return result;
            }
            catch (Exception ex) // TODO: Add more specific exeption types instead of Exception only
            {
                UnitOfWorkHelper<SiocCmsContext>.HandleException<InfoCategoryViewModel>(ex, true, transaction);
                return new RepositoryResponse<List<InfoCategoryViewModel>>()
                {
                    IsSucceed = false,
                    Data = null,
                    Exception = ex
                };
            }
            finally
            {
                //if current Context is Root
                transaction.Dispose();
                context.Dispose();
            }
        }
        #endregion
    }
}
