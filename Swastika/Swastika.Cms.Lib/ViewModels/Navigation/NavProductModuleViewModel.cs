﻿// Licensed to the Swastika I/O Foundation under one or more agreements.
// The Swastika I/O Foundation licenses this file to you under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Swastika.Cms.Lib.Models.Cms;
using Swastika.Cms.Lib.ViewModels.BackEnd;
using Swastika.Cms.Lib.ViewModels.Info;
using Swastika.Domain.Core.ViewModels;
using Swastika.Domain.Data.ViewModels;
using System.Threading.Tasks;

namespace Swastika.Cms.Lib.ViewModels.Navigation
{
    public class NavProductModuleViewModel
        : ViewModelBase<SiocCmsContext, SiocProductModule, NavProductModuleViewModel>
    {
        #region Properties

        #region Models

        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("moduleId")]
        public int ModuleId { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        #endregion Models

        #region Views

        [JsonProperty("isActived")]
        public bool IsActived { get; set; }

        [JsonProperty("module")]
        public BEModuleViewModel Module { get; set; }

        #endregion Views

        #endregion Properties

        #region Contructors

        public NavProductModuleViewModel() : base()
        {
        }

        public NavProductModuleViewModel(SiocProductModule model, SiocCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
        {
        }

        #endregion Contructors

        #region Overrides

        public override void ExpandView(SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            var getModuleResult = BEModuleViewModel.GetBy(
                m => m.Id == ModuleId && m.Specificulture == Specificulture
                , productId: ProductId
                , _context: _context, _transaction: _transaction);
            if (getModuleResult.IsSucceed)
            {
                this.Module = getModuleResult.Data;
            }
        }

        public override RepositoryResponse<bool> RemoveRelatedModels(NavProductModuleViewModel view, SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            var result= InfoModuleDataViewModel.Repository.RemoveListModel(d => d.ProductId == view.ProductId
                && d.ModuleId == view.ModuleId && d.Specificulture == view.Specificulture, _context, _transaction);
            return new RepositoryResponse<bool>()
            {
                IsSucceed = result.IsSucceed,
                Errors = result.Errors,
                Exception = result.Exception
            };
        }

        public override RepositoryResponse<bool> SaveSubModels(SiocProductModule parent, SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            RepositoryResponse<bool> result = new RepositoryResponse<bool>() { IsSucceed = true };
            foreach (var data in Module.Data.Items)
            {
                data.ProductId = parent.ProductId;
                data.ModuleId = parent.ModuleId;
                var saveResult = data.SaveModel(false, _context, _transaction);
                if (!saveResult.IsSucceed)
                {
                    result.Errors.AddRange(saveResult.Errors);
                    result.Exception = saveResult.Exception;
                }
                result.Data = result.IsSucceed = result.IsSucceed && saveResult.IsSucceed;
            }
            return result;
        }

        #region Async

        public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(NavProductModuleViewModel view, SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            var result = await InfoModuleDataViewModel.Repository.RemoveListModelAsync(d => d.ProductId == view.ProductId
                && d.ModuleId == view.ModuleId && d.Specificulture == view.Specificulture, _context, _transaction);
            return new RepositoryResponse<bool>()
            {
                IsSucceed = result.IsSucceed,
                Errors = result.Errors,
                Exception = result.Exception
            };
        }

        public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(SiocProductModule parent, SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            RepositoryResponse<bool> result = new RepositoryResponse<bool>() { IsSucceed = true };
            foreach (var data in Module.Data.Items)
            {
                data.ProductId = parent.ProductId;
                data.ModuleId = parent.ModuleId;
                var saveResult = await data.SaveModelAsync(false, _context, _transaction);
                if (!saveResult.IsSucceed)
                {
                    result.Errors.AddRange(saveResult.Errors);
                    result.Exception = saveResult.Exception;
                }
                result.Data = result.IsSucceed = result.IsSucceed && saveResult.IsSucceed;
            }
            return result;
        }

        #endregion Async

        #endregion Overrides
    }
}
