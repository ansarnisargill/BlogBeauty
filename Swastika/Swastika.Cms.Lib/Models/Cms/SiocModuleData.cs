﻿using System;
using System.Collections.Generic;

namespace Swastika.Cms.Lib.Models.Cms
{
    public partial class SiocModuleData
    {
        public string Id { get; set; }
        public int ModuleId { get; set; }
        public string Specificulture { get; set; }
        public string ArticleId { get; set; }
        public int? CategoryId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string Fields { get; set; }
        public int Priority { get; set; }
        public string ProductId { get; set; }
        public int Status { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public string Value { get; set; }

        public SiocArticleModule SiocArticleModule { get; set; }
        public SiocCategoryModule SiocCategoryModule { get; set; }
        public SiocModule SiocModule { get; set; }
    }
}
