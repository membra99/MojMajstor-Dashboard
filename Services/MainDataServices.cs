using AutoMapper;
using Entities.Context;
using Entities.Universal.MainData;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;

namespace Services
{
    public class MainDataServices : BaseServices
    {
        public MainDataServices(MainContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Product test(ProductIDTO prod)
        {
            var x = _mapper.Map<Product>(prod);
            return x;
        }

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

            var prod = await _context.Products.Where(x => x.CategoriesId == categories.CategoryId).ToListAsync();
            foreach (var item in prod)
            {
                item.CategoriesId = categories.CategoryId;
                _context.Entry(item).State = EntityState.Modified;
            }

            var prodAttr = await _context.ProductAttributes.Where(x => x.CategoriesId == categories.CategoryId).ToListAsync();
            foreach (var item in prodAttr)
            {
                item.CategoriesId = categories.CategoryId;
                _context.Entry(item).State = EntityState.Modified;
            }

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

        public async Task<ProductODTO> AddProduct(ProductIDTO productIDTO)
        {
            var product = _mapper.Map<Product>(productIDTO);
            product.ProductId = 0;
            _context.Products.Add(product);

            await SaveContextChangesAsync();

            return await GetProductsById(product.ProductId);
        }

        public async Task<ProductODTO> EditProduct(ProductIDTO productIDTO)
        {
            var product = _mapper.Map<Product>(productIDTO);
            var prodAttr = await _context.ProductAttributes.Where(x => x.ProductId == product.ProductId).ToListAsync();

            _context.Entry(product).State = EntityState.Modified;

            foreach (var item in prodAttr)
            {
                item.ProductId = productIDTO.ProductId;
                _context.Entry(product).State = EntityState.Modified;
            }
            await SaveContextChangesAsync();

            return await GetProductsById(product.ProductId);
        }

        public async Task<ProductODTO> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return null;

            var prodAttr = await _context.ProductAttributes.Where(x => x.ProductId == product.ProductId).ToListAsync();
            foreach (var item in prodAttr)
            {
                _context.ProductAttributes.Remove(item);
            }

            _context.Entry(product).State = EntityState.Modified;
            await SaveContextChangesAsync();

            var productODTO = await GetProductsById(id);
            return productODTO;
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