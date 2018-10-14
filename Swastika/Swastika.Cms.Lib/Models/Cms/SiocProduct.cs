﻿using System;
using System.Collections.Generic;

namespace Swastika.Cms.Lib.Models.Cms
{
    public partial class SiocProduct
    {
        public SiocProduct()
        {
            SiocCategoryProduct = new HashSet<SiocCategoryProduct>();
            SiocComment = new HashSet<SiocComment>();
            SiocModuleProduct = new HashSet<SiocModuleProduct>();
            SiocOrderItem = new HashSet<SiocOrderItem>();
            SiocProductMedia = new HashSet<SiocProductMedia>();
            SiocProductModule = new HashSet<SiocProductModule>();
            SiocRelatedProductS = new HashSet<SiocRelatedProduct>();
            SiocRelatedProductSiocProduct = new HashSet<SiocRelatedProduct>();
        }

        public string Id { get; set; }
        public string Specificulture { get; set; }
        public string Content { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string Excerpt { get; set; }
        public string ExtraProperties { get; set; }
        public string Icon { get; set; }
        public string Image { get; set; }
        public DateTime? LastModified { get; set; }
        public string ModifiedBy { get; set; }
        public double Price { get; set; }
        public string PriceUnit { get; set; }
        public int Priority { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeywords { get; set; }
        public string SeoName { get; set; }
        public string SeoTitle { get; set; }
        public string Source { get; set; }
        public int Status { get; set; }
        public string Tags { get; set; }
        public string Template { get; set; }
        public string Thumbnail { get; set; }
        public string Title { get; set; }
        public int Type { get; set; }
        public int? Views { get; set; }
        public string Code { get; set; }
        public double? DealPrice { get; set; }
        public double Discount { get; set; }
        public double ImportPrice { get; set; }
        public string Material { get; set; }
        public double NormalPrice { get; set; }
        public int PackageCount { get; set; }
        public string Size { get; set; }
        public int TotalSaled { get; set; }

        public SiocCulture SpecificultureNavigation { get; set; }
        public ICollection<SiocCategoryProduct> SiocCategoryProduct { get; set; }
        public ICollection<SiocComment> SiocComment { get; set; }
        public ICollection<SiocModuleProduct> SiocModuleProduct { get; set; }
        public ICollection<SiocOrderItem> SiocOrderItem { get; set; }
        public ICollection<SiocProductMedia> SiocProductMedia { get; set; }
        public ICollection<SiocProductModule> SiocProductModule { get; set; }
        public ICollection<SiocRelatedProduct> SiocRelatedProductS { get; set; }
        public ICollection<SiocRelatedProduct> SiocRelatedProductSiocProduct { get; set; }
    }
}
