using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Universal.MainData
{
    public class Tag
    {
        public int TagId { get; set; }
        public int? MediaId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }


        public Media? Media { get; set; }
        public ICollection<SiteContent> SiteContents { get; set; }
    }
}
