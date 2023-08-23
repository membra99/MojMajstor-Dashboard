using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Universal.MainData
{
    public class SiteContent
    {
        public int SiteContentId { get; set; }
        public string? Title { get; set; }
        public int? SiteContentTypeId { get; set; }
        public int? MediaId { get; set; }
        public int? SeoId { get; set; }
        public int? TagId { get; set; }
        public string? Content { get; set; }
        public bool IsActive { get; set; }

        public Seo? Seo { get; set; }
        public Tag? Tag { get; set; }
        public Media? Media { get; set; }
        public SiteContentType? SiteContentType { get; set; }
    }
}
