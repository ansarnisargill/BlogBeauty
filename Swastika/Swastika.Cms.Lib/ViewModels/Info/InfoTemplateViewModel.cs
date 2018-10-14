﻿// Licensed to the Swastika I/O Foundation under one or more agreements.
// The Swastika I/O Foundation licenses this file to you under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Swastika.Cms.Lib.Models.Cms;
using Swastika.Cms.Lib.Services;
using Swastika.Common.Helper;
using Swastika.Domain.Core.ViewModels;
using Swastika.Domain.Data.ViewModels;
using System;

namespace Swastika.Cms.Lib.ViewModels.Info
{
    public class InfoTemplateViewModel
       : ViewModelBase<SiocCmsContext, SiocTemplate, InfoTemplateViewModel>
    {
        #region Properties

        #region Models

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("templateId")]
        public int TemplateId { get; set; }

        [JsonProperty("templateName")]
        public string TemplateName { get; set; }

        [JsonProperty("folderType")]
        public string FolderType { get; set; }

        [JsonProperty("fileFolder")]
        public string FileFolder { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("extension")]
        public string Extension { get; set; }

        [JsonProperty("scripts")]
        public string Scripts { get; set; }

        [JsonProperty("styles")]
        public string Styles { get; set; }

        [JsonProperty("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }

        [JsonProperty("lastModified")]
        public DateTime? LastModified { get; set; }

        [JsonProperty("modifiedBy")]
        public string ModifiedBy { get; set; }

        #endregion Models

        #region Views

        [JsonProperty("assetFolder")]
        public string AssetFolder {
            get {
                return CommonHelper.GetFullPath(new string[] {
                    SWCmsConstants.Parameters.FileFolder,
                    SWCmsConstants.Parameters.TemplatesAssetFolder,
                    TemplateName });
            }
        }

        [JsonProperty("templateFolder")]
        public string TemplateFolder {
            get {
                return CommonHelper.GetFullPath(new string[] { SWCmsConstants.Parameters.TemplatesFolder, TemplateName });
            }
        }

        #endregion Views

        #endregion Properties

        #region Contructors

        public InfoTemplateViewModel()
            : base()
        {
        }

        public InfoTemplateViewModel(SiocTemplate model, SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
            : base(model, _context, _transaction)
        {
        }

        #endregion Contructors

        #region Expands

        /// <summary>
        /// Gets the template by path.
        /// </summary>
        /// <param name="path">The path.</param> Ex: "Pages/_Home"
        /// <returns></returns>
        public static RepositoryResponse<InfoTemplateViewModel> GetTemplateByPath(string path, string culture
            , SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            RepositoryResponse<InfoTemplateViewModel> result = new RepositoryResponse<InfoTemplateViewModel>();
            string[] temp = path.Split('/');
            if (temp.Length < 2)
            {
                result.IsSucceed = false;
                result.Errors.Add("Template Not Found");
            }
            else
            {
                int activeThemeId = GlobalConfigurationService.Instance.GetLocalInt(
                    SWCmsConstants.ConfigurationKeyword.ThemeId, culture, 0);

                result = Repository.GetSingleModel(t => t.FolderType == temp[0] && t.FileName == temp[1].Split('.')[0] && t.TemplateId == activeThemeId
                    , _context, _transaction);
            }
            return result;
        }

        #endregion Expands
    }
}
