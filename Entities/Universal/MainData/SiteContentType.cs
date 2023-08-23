using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Universal.MainData
{
    public class SiteContentType
    {
        public int SiteContentTypeId { get; set; }
        public string SiteContentTypeName { get; set; }

        public ICollection<SiteContent> SiteContents { get; set; }
    }
}
