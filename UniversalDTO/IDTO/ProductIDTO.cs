using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.IDTO
{
    public class ProductIDTO
    {
        public int ProductId { get; set; }
        public int CategoriesId { get; set; }
        public string? ProductName { get; set; }
        public double Price { get; set; }
        public bool IsOnSale { get; set; }
        public string? Description { get; set; }
        public string? Specification { get; set; }
        public bool Recommended { get; set; }
        public bool BestProduct { get; set; }
        public string? ProductCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public SaleIDTO? SaleIDTO { get; set; }
        public SeoIDTO? SeoIDTO { get; set; }
    }
}