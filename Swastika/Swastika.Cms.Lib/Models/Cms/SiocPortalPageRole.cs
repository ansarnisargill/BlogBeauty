﻿using System;
using System.Collections.Generic;

namespace Swastika.Cms.Lib.Models.Cms
{
    public partial class SiocPortalPageRole
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int PageId { get; set; }
        public int Priority { get; set; }
        public string RoleId { get; set; }
        public int Status { get; set; }

        public SiocPortalPage Page { get; set; }
    }
}
