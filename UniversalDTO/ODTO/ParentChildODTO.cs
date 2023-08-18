using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.ODTO
{
    public class ParentChildODTO
    {
        public List<ParentODTO> ParentCategory { get; set; }
        public List<ChildODTO> ChildCategory { get; set; }
    }

    public class ParentODTO 
    {
        public int CategoryId { get; set; }
        public bool IsRoot { get; set; }
    }

    public class ChildODTO
    {
        public int CategoryId { get; set; }
        public bool IsAttribute { get; set; }
        public int? ParentCategoryId { get; set; }
        public List<string>? Values { get; set; }
    }

    public class ChildODTO2
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }

    }
}
