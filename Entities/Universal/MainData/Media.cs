using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Universal.MainData
{
    public class Media
    {
        public int MediaId { get; set; }
        public int? ProductId { get; set; }
        public int MediaTypeId { get; set; }
        public string? Src { get; set; }
        public string? Extension { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? AltTitle { get; set; }
        public string? MimeType { get; set; }

        public MediaType MediaType { get; set; }
        public Product? Product { get; set; }
        public ICollection<Users> Users { get; set; }
        public ICollection<Categories> Categories { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public ICollection<SiteContent> SiteContents { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
    }
}
