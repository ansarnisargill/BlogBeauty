﻿// Licensed to the Swastika I/O Foundation under one or more agreements.
// The Swastika I/O Foundation licenses this file to you under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using Microsoft.Data.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Swastika.Cms.Lib.Models.Cms;
using Swastika.Cms.Lib.Services;
using Swastika.Domain.Core.ViewModels;
using Swastika.Domain.Data.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Swastika.Common.Utility.Enums;

namespace Swastika.Cms.Lib.ViewModels.Info
{
    public class InfoArticleViewModel
       : ViewModelBase<SiocCmsContext, SiocArticle, InfoArticleViewModel>
    {
        #region Properties

        #region Models

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("template")]
        public string Template { get; set; }

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [Required]
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("excerpt")]
        public string Excerpt { get; set; }

        [JsonProperty("seoName")]
        public string SeoName { get; set; }

        [JsonProperty("views")]
        public int? Views { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("lastModified")]
        public DateTime? LastModified { get; set; }

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
        public string ImageUrl {
            get {
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

        public string ThumbnailUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(Thumbnail))
                {
                    if (Thumbnail.IndexOf("http") == -1)
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
                else
                {
                    return ImageUrl;
                }
            }
        }

        [JsonProperty("detailsUrl")]
        public string DetailsUrl { get; set; }

        #endregion Views

        #endregion Properties

        #region Contructors

        public InfoArticleViewModel() : base()
        {
        }

        public InfoArticleViewModel(SiocArticle model, SiocCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
        {
        }

        #endregion Contructors

        #region Expands

        public static async Task<RepositoryResponse<PaginationModel<InfoArticleViewModel>>> GetModelListByCategoryAsync(
            int categoryId, string specificulture
            , string orderByPropertyName, int direction
            , int? pageSize = 1, int? pageIndex = 0
            , SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            SiocCmsContext context = _context ?? new SiocCmsContext();
            var transaction = _transaction ?? context.Database.BeginTransaction();
            try
            {
                var query = context.SiocCategoryArticle.Include(ac => ac.SiocArticle)
                    .Where(ac =>
                    ac.CategoryId == categoryId && ac.Specificulture == specificulture
                    && ac.Status == (int)SWStatus.Published).Select(ac => ac.SiocArticle);
                PaginationModel<InfoArticleViewModel> result = await Repository.ParsePagingQueryAsync(
                    query, orderByPropertyName
                    , direction,
                    pageSize, pageIndex, context, transaction
                    );
                return new RepositoryResponse<PaginationModel<InfoArticleViewModel>>()
                {
                    IsSucceed = true,
                    Data = result
                };
            }
            catch (Exception ex) // TODO: Add more specific exeption types instead of Exception only
            {
                Repository.LogErrorMessage(ex);
                if (_transaction == null)
                {
                    //if current transaction is root transaction
                    transaction.Rollback();
                }

                return new RepositoryResponse<PaginationModel<InfoArticleViewModel>>()
                {
                    IsSucceed = false,
                    Data = null,
                    Exception = ex
                };
            }
            finally
            {
                if (_context == null)
                {
                    //if current Context is Root
                    context.Dispose();
                }
            }
        }

        #region Sync

        public static RepositoryResponse<PaginationModel<InfoArticleViewModel>> GetModelListByCategory(
           int categoryId, string specificulture
           , string orderByPropertyName, int direction
           , int? pageSize = 1, int? pageIndex = 0
           , SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            SiocCmsContext context = _context ?? new SiocCmsContext();
            var transaction = _transaction ?? context.Database.BeginTransaction();
            try
            {
                var query = context.SiocCategoryArticle.Include(ac => ac.SiocArticle)
                    .Where(ac =>
                    ac.CategoryId == categoryId && ac.Specificulture == specificulture
                    && ac.Status == (int)SWStatus.Published).Select(ac => ac.SiocArticle);
                PaginationModel<InfoArticleViewModel> result = Repository.ParsePagingQuery(
                    query, orderByPropertyName
                    , direction,
                    pageSize, pageIndex, context, transaction
                    );
                return new RepositoryResponse<PaginationModel<InfoArticleViewModel>>()
                {
                    IsSucceed = true,
                    Data = result
                };
            }
            catch (Exception ex) // TODO: Add more specific exeption types instead of Exception only
            {
                Repository.LogErrorMessage(ex);
                if (_transaction == null)
                {
                    //if current transaction is root transaction
                    transaction.Rollback();
                }

                return new RepositoryResponse<PaginationModel<InfoArticleViewModel>>()
                {
                    IsSucceed = false,
                    Data = null,
                    Exception = ex
                };
            }
            finally
            {
                if (_context == null)
                {
                    //if current Context is Root
                    context.Dispose();
                }
            }
        }

        public static RepositoryResponse<PaginationModel<InfoArticleViewModel>> GetModelListByModule(
          int ModuleId, string specificulture
          , string orderByPropertyName, int direction
          , int? pageSize = 1, int? pageIndex = 0
          , SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            SiocCmsContext context = _context ?? new SiocCmsContext();
            var transaction = _transaction ?? context.Database.BeginTransaction();
            try
            {
                var query = context.SiocModuleArticle.Include(ac => ac.SiocArticle)
                    .Where(ac =>
                    ac.ModuleId == ModuleId && ac.Specificulture == specificulture
                    && (ac.Status == (int)SWStatus.Published || ac.Status == (int)SWStatus.Preview)).Select(ac => ac.SiocArticle);
                PaginationModel<InfoArticleViewModel> result = Repository.ParsePagingQuery(
                    query, orderByPropertyName
                    , direction,
                    pageSize, pageIndex, context, transaction
                    );
                return new RepositoryResponse<PaginationModel<InfoArticleViewModel>>()
                {
                    IsSucceed = true,
                    Data = result
                };
            }
            catch (Exception ex) // TODO: Add more specific exeption types instead of Exception only
            {
                Repository.LogErrorMessage(ex);
                if (_transaction == null)
                {
                    //if current transaction is root transaction
                    transaction.Rollback();
                }

                return new RepositoryResponse<PaginationModel<InfoArticleViewModel>>()
                {
                    IsSucceed = false,
                    Data = null,
                    Exception = ex
                };
            }
            finally
            {
                if (_context == null)
                {
                    //if current Context is Root
                    context.Dispose();
                }
            }
        }

        #endregion Sync

        #endregion Expands

        #region Overrides

        public override void ExpandView(SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            UrlAlias = InfoUrlAliasViewModel.Repository.GetSingleModel(u => u.Specificulture == Specificulture && u.SourceId == Id.ToString()).Data;
            if (UrlAlias == null)
            {
                UrlAlias = new InfoUrlAliasViewModel()
                {
                    Type = SWCmsConstants.UrlAliasType.Article,
                    Specificulture = Specificulture,
                    Alias = SeoName
                };
            }
        }

        #endregion
    }
}
