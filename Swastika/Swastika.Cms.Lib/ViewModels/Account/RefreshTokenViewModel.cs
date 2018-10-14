﻿// Licensed to the Swastika I/O Foundation under one or more agreements.
// The Swastika I/O Foundation licenses this file to you under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Swastika.Cms.Lib.Models.Account;
using Swastika.Domain.Data.ViewModels;
using System;

namespace Swastika.Cms.Lib.ViewModels.Account
{
    public class RefreshTokenViewModel
        : ViewModelBase<SiocCmsAccountContext, RefreshTokens, RefreshTokenViewModel>
    {
        #region Properties

        #region Models

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("expiresUtc")]
        public DateTime ExpiresUtc { get; set; }

        [JsonProperty("issuedUtc")]
        public DateTime IssuedUtc { get; set; }

        #endregion Models

        #endregion Properties

        #region Contructors

        public RefreshTokenViewModel() : base()
        {
        }

        public RefreshTokenViewModel(RefreshTokens model, SiocCmsAccountContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
        {
        }

        #endregion Contructors
    }
}
