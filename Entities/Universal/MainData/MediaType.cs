using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Universal.MainData
{
    public class MediaType
    {
        public int MediaTypeId { get; set; }
        public string? MediaTypeName { get; set; }

        public ICollection<Media> Medias { get; set; }
    }
}
