using AutoMapper;
using Entities.Context;
using Entities.Migrations;
using Entities.Universal.MainData;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;

namespace Services
{
    public class MainDataServices : BaseServices
    {
        public MainDataServices(MainContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public List<ChildODTO> children = new List<ChildODTO>();
        private int i = 0;

        #region Categories

        private IQueryable<CategoriesODTO> GetCategories(int id)
        {
            return from x in _context.Categories
                   where (id == 0 || x.CategoryId == id)
                   select _mapper.Map<CategoriesODTO>(x);
        }

        public async Task<CategoriesODTO> GetCategoriesById(int id)
        {
            return await GetCategories(id).AsNoTracking().SingleOrDefaultAsync();
        }

        public async Task<CategoriesODTO> AddCategory(CategoriesIDTO categoriesIDTO)
        {
            var categories = _mapper.Map<Categories>(categoriesIDTO);
            categories.CategoryId = 0;
            _context.Categories.Add(categories);

            await SaveContextChangesAsync();

            return await GetCategoriesById(categories.CategoryId);
        }

        public async Task<CategoriesODTO> EditCategory(CategoriesIDTO categoriesIDTO)
        {
            var categories = _mapper.Map<Categories>(categoriesIDTO);
            _context.Entry(categories).State = EntityState.Modified;
            await SaveContextChangesAsync();

            return await GetCategoriesById(categories.CategoryId);
        }

        public async Task<CategoriesODTO> DeleteCategory(int id)
        {
            var categories = await _context.Categories.FindAsync(id);
            if (categories == null) return null;

            var prodAttr = await _context.ProductAttributes.Where(x => x.CategoriesId == categories.CategoryId).ToListAsync();
            foreach (var item in prodAttr)
            {
                _context.ProductAttributes.Remove(item);
                await SaveContextChangesAsync();
            }

            var prod = await _context.Products.Where(x => x.CategoriesId == categories.CategoryId).ToListAsync();
            foreach (var item in prod)
            {
                _context.Products.Remove(item);
                await SaveContextChangesAsync();
            }
            var categoriesODTO = await GetCategoriesById(id);

            categories.IsActive = false;
            _context.Entry(categories).State = EntityState.Modified;
            await SaveContextChangesAsync();
            return categoriesODTO;
        }

        #endregion Categories

        #region Product

        private IQueryable<ProductODTO> GetProducts(int id)
        {
            return from x in _context.Products
                   where (id == 0 || x.ProductId == id)
                   select _mapper.Map<ProductODTO>(x);
        }

        public async Task<ProductODTO> GetProductsById(int id)
        {
            return await GetProducts(id).AsNoTracking().SingleOrDefaultAsync();
        }

        public async Task<List<ProductODTO>> GetAllProducts()
        {
            return await GetProducts(0).AsNoTracking().ToListAsync();
        }

        public async Task<ProductODTO> AddProduct(ProductIDTO productIDTO)
        {
            int seo = 0;
            if (productIDTO.SeoIDTO.GoogleKeywords != null || productIDTO.SeoIDTO.GoogleKeywords != null)
            {
                seo = await AddSeo(productIDTO.SeoIDTO.GoogleDesc, productIDTO.SeoIDTO.GoogleKeywords);
            }

            var product = _mapper.Map<Product>(productIDTO);
            product.ProductId = 0;
            product.SeoId = (seo != 0) ? seo : null;
            _context.Products.Add(product);

            await SaveContextChangesAsync();

            if (product.IsOnSale == true)
            {
                SaleIDTO sale = new SaleIDTO();
                sale.Value = productIDTO.SaleIDTO.Value;
                sale.SaleTypeId = productIDTO.SaleIDTO.SaleTypeId;
                sale.StartDate = productIDTO.SaleIDTO.StartDate;
                sale.EndDate = productIDTO.SaleIDTO.EndDate;
                sale.IsActive = true;
                sale.ProductId = product.ProductId;

                var SaleForDB = _mapper.Map<Sale>(sale);

                _context.Sales.Add(SaleForDB);

                await SaveContextChangesAsync();
            }

            return await GetProductsById(product.ProductId);
        }

        public async Task<int> AddSeo(string googleDesc, string googleKeywords)
        {
            Entities.Universal.MainData.Seo seo = new Entities.Universal.MainData.Seo();
            seo.SeoId = 0;
            seo.GoogleDesc = googleDesc;
            seo.GoogleKeywords = googleKeywords;
            _context.Seos.Add(seo);
            await SaveContextChangesAsync();

            return await (from x in _context.Seos
                          where x.SeoId == seo.SeoId
                          select x.SeoId).SingleOrDefaultAsync();
        }

        public async Task<ParentChildODTO> GetTree(int Id)
        {
            ParentChildODTO retval = new ParentChildODTO();
            var parentCategoryID = await _context.Categories.Where(x => x.CategoryId == Id).Select(x => x.ParentCategoryId).SingleOrDefaultAsync();
            bool notRoot = true;
            List<ParentODTO> parents = new List<ParentODTO>();
            while (notRoot)
            {
                ParentODTO parent = new ParentODTO();
                var currentCategories = await _context.Categories.Where(x => x.CategoryId == parentCategoryID).SingleOrDefaultAsync();
                parent.CategoryId = currentCategories.CategoryId;
                parent.IsRoot = (currentCategories.ParentCategoryId == null) ? true : false;
                parents.Add(parent);
                parentCategoryID = currentCategories.ParentCategoryId;
                if (currentCategories.ParentCategoryId == null)
                {
                    notRoot = false;
                }
            }

            retval.ParentCategory = parents;
            retval.ChildCategory = ReturnChildren(Id);

            return retval;
        }

        public List<ChildODTO> ReturnChildren(int Id)
        {
            var categoryList = _context.Categories.Where(x => x.ParentCategoryId == Id).ToList();
            foreach (var item in categoryList)
            {
                ChildODTO child = new ChildODTO();
                child.CategoryId = item.CategoryId;
                child.IsAttribute = item.IsAttribute;
                child.ParentCategoryId = item.ParentCategoryId;
                if (child.IsAttribute == true)
                {
                    var val = _context.ProductAttributes.Where(x => x.CategoriesId == child.CategoryId).Select(x => x.Value).ToList();
                    child.Values = val;
                    child.ParentCategoryId = Id;
                }
                children.Add(child);
            }
            if (children.Count() > i)
            {
                ReturnChildren(children[i++].CategoryId);
            }

            return children;
        }

        public async Task<ProductODTO> EditProduct(ProductIDTO productIDTO)
        {
            var product = _mapper.Map<Product>(productIDTO);
            _context.Entry(product).State = EntityState.Modified;
            await SaveContextChangesAsync();

            return await GetProductsById(product.ProductId);
        }

        public async Task<ProductODTO> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return null;

            var productODTO = await GetProductsById(id);

            var prodAttr = await _context.ProductAttributes.Where(x => x.ProductId == product.ProductId).ToListAsync();
            foreach (var item in prodAttr)
            {
                _context.ProductAttributes.Remove(item);
            }

            _context.Products.Remove(product);
            await SaveContextChangesAsync();

            return productODTO;
        }

        public async Task<List<ChildODTO2>> GetCategory()
        {
            var categoriesRoot = await _context.Categories.Where(x => x.ParentCategoryId == null).SingleOrDefaultAsync();

            var cat = ReturnChildren(categoriesRoot.CategoryId);

            ChildODTO child = new ChildODTO();
            child.CategoryId = categoriesRoot.CategoryId;
            child.IsAttribute = false;
            child.ParentCategoryId = categoriesRoot.ParentCategoryId;
            child.Values = null;
            cat.Insert(0, child);
            var CategoryWithoutAttr = (from y in cat
                                       where y.IsAttribute == false
                                       select y).ToList();
            List<ChildODTO2> children = new List<ChildODTO2>();
            foreach (var item in CategoryWithoutAttr)
            {
                ChildODTO2 ch = new ChildODTO2();
                ch.CategoryId = item.CategoryId;
                ch.ParentCategoryId = item.ParentCategoryId;
                ch.CategoryName = _context.Categories.Where(x => x.CategoryId == item.CategoryId).Select(x => x.CategoryName).SingleOrDefault();
                children.Add(ch);
            }

            return children;
        }

        public async Task<List<AttributeODTO>> GetAttribute(int CategoryId)
        {
            List<AttributeODTO> category = await (from x in _context.Categories
                                                  where x.ParentCategoryId == CategoryId
                                                  select new AttributeODTO
                                                  {
                                                      CategoryId = x.CategoryId,
                                                      CategoryName = x.CategoryName
                                                  }).ToListAsync();

            List<AttributeODTO> retval = new List<AttributeODTO>();
            foreach (var item in category)
            {
                AttributeODTO attributeValue = new AttributeODTO();
                attributeValue.CategoryId = item.CategoryId;
                attributeValue.CategoryName = item.CategoryName;
                attributeValue.Value = await (from x in _context.ProductAttributes
                                              where x.CategoriesId == item.CategoryId
                                              select x.Value).Distinct().ToListAsync();
                retval.Add(attributeValue);
            }

            return retval;
        }

        #endregion Product

        #region ProductAttributes

        private IQueryable<ProductAttributesODTO> GetProductAttributes(int id)
        {
            return from x in _context.ProductAttributes
                   where (id == 0 || x.ProductAttributeId == id)
                   select _mapper.Map<ProductAttributesODTO>(x);
        }

        public async Task<ProductAttributesODTO> GetProductAttributesById(int id)
        {
            return await GetProductAttributes(id).AsNoTracking().SingleOrDefaultAsync();
        }

        public async Task<ProductAttributesODTO> AddProductAttributes(ProductAttributesIDTO productAttributesIDTO)
        {
            var productAttributes = _mapper.Map<ProductAttributes>(productAttributesIDTO);
            productAttributes.ProductAttributeId = 0;
            _context.ProductAttributes.Add(productAttributes);

            await SaveContextChangesAsync();

            return await GetProductAttributesById(productAttributes.ProductAttributeId);
        }

        public async Task<ProductAttributesODTO> EditProductAttributes(ProductAttributesIDTO productAttributesIDTO)
        {
            var productAttributes = _mapper.Map<ProductAttributes>(productAttributesIDTO);
            _context.Entry(productAttributes).State = EntityState.Modified;

            await SaveContextChangesAsync();

            return await GetProductAttributesById(productAttributes.ProductAttributeId);
        }

        public async Task<ProductAttributesODTO> DeleteProductAttributes(int id)
        {
            var productAttributes = await _context.ProductAttributes.FindAsync(id);
            if (productAttributes == null) return null;

            var productAttributesODTO = await GetProductAttributesById(id);
            _context.ProductAttributes.Remove(productAttributes);
            await SaveContextChangesAsync();
            return productAttributesODTO;
        }

        #endregion ProductAttributes
    }
}