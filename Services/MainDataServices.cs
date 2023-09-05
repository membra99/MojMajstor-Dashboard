using AutoMapper;
using Entities.Context;
using Entities.Migrations;
using Entities.Universal.MainData;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetTopologySuite.Index.HPRtree;
using Services.AWS;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;
using static Universal.DTO.CommonModels.CommonModels;

namespace Services
{
    public class MainDataServices : BaseServices
    {
        public static UsersServices _userServices;
        private readonly IAWSS3FileService _AWSS3FileService;

        public MainDataServices(MainContext context, IMapper mapper, UsersServices usersServices, IAWSS3FileService AWSS3FileService) : base(context, mapper)
        {
            _userServices = usersServices;
            _AWSS3FileService = AWSS3FileService;
        }

        public List<ChildODTO> children = new List<ChildODTO>();
        private int i = 0;

        #region FileUploads

        public async Task<MediaODTO> UploadProductImage(AWSFileUpload awsFile, string mediaType, int productId)
        {
            bool successUpload = false;

            if (awsFile.Attachments.Count > 0)
                successUpload = await _AWSS3FileService.UploadFile(awsFile);

            if (successUpload)
            {
                var key = await _AWSS3FileService.FilesListSearch("DOT/" + awsFile.Attachments.First().FileName);
                var media = new Media();
                media.ProductId = productId;
                media.Extension = awsFile.Attachments.First().FileName.Split('.')[1];
                media.Src = "DOT/" + key.First();
                media.MediaTypeId = _context.MediaTypes.FirstOrDefault(x => x.MediaTypeName == mediaType).MediaTypeId;
                _context.Medias.Add(media);
                await _context.SaveChangesAsync();
                return _mapper.Map<MediaODTO>(media);
            }
            else
            {
                return null;
            }
        }

        #endregion FileUploads

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
            if (!_context.Categories.Any(x => x.CategoryName == categoriesIDTO.CategoryName && x.ParentCategoryId == categoriesIDTO.ParentCategoryId))
            {
                int seo = 0;
                if (categoriesIDTO.IsAttribute == false)
                {
                    if (categoriesIDTO.SeoIDTO.GoogleKeywords != null || categoriesIDTO.SeoIDTO.GoogleKeywords != null)
                    {
                        seo = await AddSeo(categoriesIDTO.SeoIDTO.GoogleDesc, categoriesIDTO.SeoIDTO.GoogleKeywords);
                    }
                }
                var categories = _mapper.Map<Categories>(categoriesIDTO);
                categories.CategoryId = 0;
                categories.SeoId = (seo != 0) ? seo : null;
                _context.Categories.Add(categories);

                await SaveContextChangesAsync();

                return await GetCategoriesById(categories.CategoryId);
            }
            else
            {
                var categories = await _context.Categories.Where(x => x.CategoryName == categoriesIDTO.CategoryName && x.ParentCategoryId == categoriesIDTO.ParentCategoryId && x.IsActive == false).FirstOrDefaultAsync();
                categories.IsActive = true;
                _context.Entry(categories).State = EntityState.Modified;
                await SaveContextChangesAsync();
            }
            return null;
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
            var childCat = ReturnChildren(id).OrderByDescending(x => x.CategoryId);

            var catwithAtr = childCat.Where(x => x.IsAttribute == true).ToList();

            var catwithoutAtr = childCat.Where(x => x.IsAttribute == false).Select(x => x.CategoryId).ToList();
            catwithoutAtr.Add(id);

            foreach (var item in catwithAtr)
            {
                var atributes = await _context.Attributes.Where(x => x.CategoriesId == item.CategoryId).ToListAsync();
				foreach (var atribute in atributes)
				{
					var productAttributes = await _context.ProductAttributes.Where(x => x.AttributesId == atribute.AttributesId).ToListAsync();
					_context.ProductAttributes.RemoveRange(productAttributes);
				}
				_context.Attributes.RemoveRange(atributes);
                await SaveContextChangesAsync();
            }

            foreach (var catID in catwithoutAtr)
            {
				var products = await _context.Products.Where(x => x.CategoriesId == catID).ToListAsync();
				foreach (var product in products)
				{
					var sales = await _context.Sales.Where(x => x.ProductId == product.ProductId).ToListAsync();
					var media = await _context.Medias.Where(x => x.ProductId == product.DeclarationId).ToListAsync();
					var orders = await _context.OrderDetails.Where(x => x.ProductId == product.ProductId).ToListAsync();
					_context.Sales.RemoveRange(sales);
					_context.Medias.RemoveRange(media);
					_context.OrderDetails.RemoveRange(orders);
				}
				_context.Products.RemoveRange(products);
				await SaveContextChangesAsync();
			}

            var categoryids = childCat.Select(x => x.CategoryId).ToList();
            var categories = await _context.Categories.Where(x => categoryids.Contains(x.CategoryId)).ToListAsync();
            _context.Categories.RemoveRange(categories);

            var MainCategory = await _context.Categories.Where(x => x.CategoryId == id).SingleOrDefaultAsync();
            MainCategory.IsActive = false;
            _context.Entry(MainCategory).State = EntityState.Modified;

            await SaveContextChangesAsync();

            return _mapper.Map<CategoriesODTO>(MainCategory);
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
		public async Task<ProductIDTO> GetProductsByIdForEdit(int id)
		{
			var product = _mapper.Map<ProductIDTO>(await GetProducts(id).AsNoTracking().SingleOrDefaultAsync());
            product.SaleIDTO = _mapper.Map <SaleIDTO>(await _context.Sales.FirstOrDefaultAsync(x => x.ProductId == product.ProductId));
             product.SeoIDTO = _mapper.Map <SeoIDTO>(await _context.Seos.FirstOrDefaultAsync(x => x.SeoId == product.SeoId));
            DateTime startdate = DateTime.ParseExact(product.SaleIDTO.StartDate, "dd/MM/yyyy HH:mm:ss", null);
            DateTime enddate = DateTime.ParseExact(product.SaleIDTO.EndDate, "dd/MM/yyyy HH:mm:ss", null);
            product.SaleIDTO.StartDate = startdate.ToString("yyyy-MM-dd");
            product.SaleIDTO.EndDate = enddate.ToString("yyyy-MM-dd");
            return product;
		}

        public async Task<SaleODTO> AddSale(SaleIDTO saleIDTO, int productId)
        {
            var sale = _mapper.Map<Sale>(saleIDTO);
            sale.SaleId = 0;
            sale.IsActive = true;
            sale.ProductId = productId;
            _context.Sales.Add(sale);
            await SaveContextChangesAsync();

            return await _context.Sales.Where(x => x.SaleId == sale.SaleId).Select(x => _mapper.Map<SaleODTO>(x)).SingleOrDefaultAsync();
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
                child.IsActive = (bool)item.IsActive;
                child.ParentCategoryId = item.ParentCategoryId;
                //TODO proveriti da li ovo treba da ostane
                //if (child.IsAttribute == true)
                //{
                //    var val = _context.ProductAttributes.Where(x => x.CategoriesId == child.CategoryId).Select(x => x.Value).ToList();
                //    child.Values = val;
                //    child.ParentCategoryId = Id;
                //}
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

            var seo = await _context.Seos.Where(x => x.SeoId == productIDTO.SeoId).SingleOrDefaultAsync();
			seo.GoogleDesc = productIDTO.SeoIDTO.GoogleDesc;
            seo.GoogleKeywords = productIDTO.SeoIDTO.GoogleKeywords;
            _context.Entry(seo).State = EntityState.Modified;

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

		public async Task<List<ChildODTO2>> GetCategories()
		{
			var categoriesRoot = await _context.Categories.Where(x => x.ParentCategoryId == null && x.IsActive == true).SingleOrDefaultAsync();

			if (categoriesRoot == null)
				return null;

            var cat = ReturnChildren(categoriesRoot.CategoryId);

            ChildODTO child = new ChildODTO();
            child.CategoryId = categoriesRoot.CategoryId;
            child.IsAttribute = false;
            child.IsActive = true;
            child.ParentCategoryId = categoriesRoot.ParentCategoryId;
            child.Values = null;
            cat.Insert(0, child);
            var CategoryWithoutAttr = (from y in cat
                                       where (y.IsAttribute == false)
                                       && (y.IsActive == true)
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
                                                      CategoryName = x.CategoryName,
                                                      Value = _context.Attributes.Where(y => y.CategoriesId == x.CategoryId).Select(y => y.Value).Distinct().ToList()
                                                  }).ToListAsync();

            //List<AttributeODTO> retval = new List<AttributeODTO>();
            //TODO proveriti da li ovo treba
            //foreach (var item in category)
            //{
            //	AttributeODTO attributeValue = new AttributeODTO();
            //	attributeValue.CategoryId = item.CategoryId;
            //	attributeValue.CategoryName = item.CategoryName;
            //	attributeValue.Value = await (from x in _context.ProductAttributes
            //								  where x.CategoriesId == item.CategoryId
            //								  select x.Value).Distinct().ToListAsync();
            //	retval.Add(attributeValue);
            //}

            return category;
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

        public async Task DeleteAllProductAttributes(int id)
        {
            var productAttr = _context.ProductAttributes.Where(x => x.ProductId == id).ToList();

            _context.RemoveRange(productAttr);

            await SaveContextChangesAsync();
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

        #region SiteContent

        private IQueryable<SiteContentODTO> GetSiteContent(int id)
        {
            return from x in _context.SiteContents
                   where (id == 0 || x.SiteContentId == id)
                   select _mapper.Map<SiteContentODTO>(x);
        }

        public async Task<SiteContentODTO> GetSiteContentById(int id)
        {
            return await GetSiteContent(id).AsNoTracking().SingleOrDefaultAsync();
        }

        public async Task<List<SiteContentODTO>> GetAllSiteContentByType(string Type)
        {
            var siteContentType = await _context.SiteContentTypes.Where(x => x.SiteContentTypeName == Type).Select(x => x.SiteContentTypeId).SingleOrDefaultAsync();
            return await _context.SiteContents.Where(x => x.SiteContentTypeId == siteContentType && x.IsActive == true).Select(x => _mapper.Map<SiteContentODTO>(x)).ToListAsync();
        }

        public async Task<SiteContentODTO> AddSiteContent(SiteContentIDTO siteContentIDTO)
        {
            int seo = 0;
            if (siteContentIDTO.SeoIDTO.GoogleKeywords != null || siteContentIDTO.SeoIDTO.GoogleKeywords != null)
            {
                seo = await AddSeo(siteContentIDTO.SeoIDTO.GoogleDesc, siteContentIDTO.SeoIDTO.GoogleKeywords);
            }

            var siteContent = _mapper.Map<SiteContent>(siteContentIDTO);
            siteContent.SiteContentTypeId = 0;
            siteContent.SeoId = (seo != 0) ? seo : null;
            _context.SiteContents.Add(siteContent);

            await SaveContextChangesAsync();

            return await GetSiteContentById(siteContent.SiteContentId);
        }

        public async Task<SiteContentODTO> EditSiteContent(SiteContentIDTO siteContentIDTO)
        {
            var siteContent = _mapper.Map<SiteContent>(siteContentIDTO);
            _context.Entry(siteContent).State = EntityState.Modified;

            var seo = await _context.Seos.Where(x => x.SeoId == siteContentIDTO.SeoId).SingleOrDefaultAsync();
            seo.GoogleDesc = siteContent.Seo.GoogleDesc;
            seo.GoogleKeywords = siteContent.Seo.GoogleKeywords;

            _context.Entry(seo).State = EntityState.Modified;

            await SaveContextChangesAsync();

            return await GetSiteContentById(siteContent.SiteContentId);
        }

        public async Task<SiteContentODTO> DeleteSiteContent(int id)
        {
            var siteContent = await _context.SiteContents.FindAsync(id);
            if (siteContent == null) return null;

            var siteContentODTO = await GetSiteContentById(id);

            _context.SiteContents.Remove(siteContent);
            await SaveContextChangesAsync();

            return siteContentODTO;
        }

        #endregion SiteContent

        #region Order

        private IQueryable<OrderODTO> GetOrders(int id)
        {
            return from x in _context.Orders
                   where (id == 0 || x.OrderId == id)
                   select _mapper.Map<OrderODTO>(x);
        }

        public async Task<OrderODTO> GetOrderById(int id)
        {
            return await GetOrders(id).AsNoTracking().SingleOrDefaultAsync();
        }

        public async Task<int> AnonimusOrRegistredUser(UsersIDTO usersIDTO)
        {
            var isUserExist = await _context.Users.Where(x => x.Email == usersIDTO.Email).Select(x => x.UsersId).SingleOrDefaultAsync();
            int UserID = isUserExist;
            if (isUserExist == 0)
            {
                var user = await _userServices.AddUser(usersIDTO);
                UserID = user.UsersId;
            }

            return UserID;
        }

		public async Task<List<OrderODTO>> GetAllOrder()
        {
            var orders = await _context.Orders.Include(x => x.Users).Select(x => _mapper.Map<OrderODTO>(x)).ToListAsync();
            return orders;
        }

		public async Task<FullOrderODTO> EditStatus(int orderId, string status)
		{
			var order = await _context.Orders.Where(x => x.OrderId == orderId).SingleOrDefaultAsync();
			order.OrderStatus = status;
			order.UpdatedAt = DateTime.Now;
			_context.Entry(order).State = EntityState.Modified;
			await SaveContextChangesAsync();

			return await GetFullOrderById(orderId);
		}

		public async Task<FullOrderODTO> GetFullOrderById(int id)
        {
            var order = await _context.Orders.Where(x => x.OrderId == id).SingleOrDefaultAsync();
            FullOrderODTO fullOrder = new FullOrderODTO();
            fullOrder.UsersODTO = new UsersODTO();
            fullOrder.OrderId = id;
            var user = await _context.Users.Where(x => x.UsersId == order.UsersId).SingleOrDefaultAsync();
            fullOrder.UsersODTO.Address = user.Address;
            fullOrder.UsersODTO.Email = user.Email;
            fullOrder.UsersODTO.FirstName = user.FirstName;
            fullOrder.UsersODTO.LastName = user.LastName;
            fullOrder.UsersODTO.Country = user.Country;
            fullOrder.UsersODTO.Role = user.Role;
            fullOrder.UsersODTO.City = user.City;
            fullOrder.UsersODTO.Zip = user.Zip;
            fullOrder.UsersODTO.Phone = user.Phone;
            fullOrder.Name = user.FirstName + " " + user.LastName;
            fullOrder.Status = order.OrderStatus;

            var products = await _context.OrderDetails.Where(x => x.OrderId == id).Select(x => x.ProductId).ToListAsync();
            List<ProductDetailsForOrderODTO> productList = new List<ProductDetailsForOrderODTO>();
            foreach (var item in products)
            {
                var product = await _context.Products.Where(x => x.ProductId == item).SingleOrDefaultAsync();
                ProductDetailsForOrderODTO productODTO = new ProductDetailsForOrderODTO();
                productODTO.ProductId = item;
                productODTO.ProductName = product.ProductName;
                productODTO.ProductCode = product.ProductCode;
                productODTO.CategoriesId = product.CategoriesId;
                productODTO.CategoryName = await _context.Categories.Where(x => x.CategoryId == product.CategoriesId).Select(x => x.CategoryName).SingleOrDefaultAsync();
                productODTO.Price = product.Price;
                productODTO.Quantity = await _context.OrderDetails.Where(x => x.ProductId == item && x.OrderId == id).Select(x => x.Quantity).SingleOrDefaultAsync();

                productList.Add(productODTO);
            }

            fullOrder.TotalPrice = (from x in productList
                                    select x.Quantity * Convert.ToInt32(x.Price)).Sum();

            fullOrder.ProductODTO = productList;

            return fullOrder;
        }

        public async Task<OrderODTO> EditOrder(int id, string status)
        {
            var order = await _context.Orders.Where(x => x.OrderId == id).SingleOrDefaultAsync();
            order.OrderStatus = status;
            _context.Entry(order).State = EntityState.Modified;

            await SaveContextChangesAsync();

            return await GetOrderById(order.OrderId);
        }

        public async Task PostOrder(OrderDetailsIDTO orderIDTO)
        {
            var userId = await AnonimusOrRegistredUser(orderIDTO.UsersIDTO);
            OrderIDTO order = new OrderIDTO();
            order.OrderId = 0;
            order.UsersId = userId;
            order.OrderDate = DateTime.Now;
            order.OrderStatus = "On hold";

            var orderForDB = _mapper.Map<Order>(order);

            _context.Orders.Add(orderForDB);
            await SaveContextChangesAsync();

            var NewOrder = await GetOrderById(orderForDB.OrderId);

            foreach (var item in orderIDTO.ProductList)
            {
                OrderDetails orderDetails = new OrderDetails();
                orderDetails.OrderId = NewOrder.OrderId;
                orderDetails.ProductId = item;
                _context.OrderDetails.Add(orderDetails);
                await SaveContextChangesAsync();
            }
        }

        public async Task<OrderODTO> DeleteOrder(int id)
        {
            var orders = await _context.Orders.FindAsync(id);
            if (orders == null) return null;

            var ordersODTO = await GetOrderById(id);

            _context.Orders.Remove(orders);
            await SaveContextChangesAsync();

            return ordersODTO;
        }

        #endregion Order

        #region Declaration

        private IQueryable<DeclarationODTO> GetDeclarations(int id)
        {
            return from x in _context.Declarations
                   where (id == 0 || x.DeclarationId == id)
                   select _mapper.Map<DeclarationODTO>(x);
        }

		private IQueryable<DeclarationIDTO> GetDeclarationsIDTO(int id)
		{
			return from x in _context.Declarations
				   where (id == 0 || x.DeclarationId == id)
				   select _mapper.Map<DeclarationIDTO>(x);
		}

		public async Task<DeclarationODTO> GetDeclarationById(int id)
		{
			return await GetDeclarations(id).AsNoTracking().SingleOrDefaultAsync();
		}

		public async Task<DeclarationIDTO> GetDeclarationForEditById(int id)
		{
			return await GetDeclarationsIDTO(id).AsNoTracking().SingleOrDefaultAsync();
		}

		public async Task<List<DeclarationODTO>> GetAllDeclarations()
		{
			return await _context.Declarations.Select(x => _mapper.Map<DeclarationODTO>(x)).ToListAsync();
		}

        public async Task<DeclarationODTO> AddDeclaration(DeclarationIDTO declarationIDTO)
        {
            var declaration = _mapper.Map<Declaration>(declarationIDTO);
            declaration.DeclarationId = 0;
            _context.Declarations.Add(declaration);

            await SaveContextChangesAsync();

            return await GetDeclarationById(declaration.DeclarationId);
        }

        public async Task<DeclarationODTO> EditDeclaration(DeclarationIDTO declarationIDTO)
        {
            var declaration = _mapper.Map<Declaration>(declarationIDTO);
            _context.Entry(declaration).State = EntityState.Modified;
            await SaveContextChangesAsync();

            return await GetDeclarationById(declaration.DeclarationId);
        }

        public async Task<DeclarationODTO> DeleteDeclaration(int id)
        {
            var declaration = await _context.Declarations.FindAsync(id);
            if (declaration == null) return null;

            var declarationODTO = await GetDeclarationById(id);

            _context.Declarations.Remove(declaration);
            await SaveContextChangesAsync();

            return declarationODTO;
        }

        #endregion Declaration

        #region Media

        public async Task<List<MediaODTO>> GetAllMedia()
        {
            var media = await _context.Medias.Select(x => _mapper.Map<MediaODTO>(x)).ToListAsync();

            foreach (var item in media)
            {
                var index = item.Src.LastIndexOf('/');
                item.Src = item.Src.Substring(index + 1);
            }

            return media;
        }

        //public async Task<List<MediaODTO>> DeleteMultipleMedia()

        #endregion Media

        #region Attributes

        private IQueryable<AttributesODTO> GetAttributes(int id)
        {
            return from x in _context.Attributes.Include(x => x.Categories)
                   where (id == 0 || x.AttributesId == id)
                   select _mapper.Map<AttributesODTO>(x);
        }

        public async Task<AttributesODTO> GetAttributesById(int id)
        {
            return await GetAttributes(id).AsNoTracking().SingleOrDefaultAsync();
        }

        public async Task<List<CategoriesODTO>> GetAllAttributesByCategoryName(int categoryId)
        {
            var attributes = await _context.Categories.Where(x => x.ParentCategoryId == categoryId && x.IsAttribute == true && x.IsActive == true).Select(x => _mapper.Map<CategoriesODTO>(x)).ToListAsync();
            return attributes;
        }

        public async Task<List<CategoriesODTO>> GetAllCategoriesWithAttributes()
        {
            var category = await _context.Categories.Where(x => x.IsAttribute == false && x.IsActive == true && x.ParentCategoryId != null).Select(x => new {x.CategoryId,x.ParentCategoryId}).ToListAsync();
            var CategoryIds = new List<int>();
            var ParrentIds = new List<int>();
            foreach (var item in category)
            {
                ParrentIds.Add((int)item.ParentCategoryId);
                CategoryIds.Add(item.CategoryId);
			}

            var ExceptIds = CategoryIds.Except(ParrentIds).ToList();
            List<CategoriesODTO> retval = new List<CategoriesODTO>();
            foreach (var item in ExceptIds)
            {
                var cat = await _context.Categories.Where(x => x.CategoryId == item).Select(x => _mapper.Map<CategoriesODTO>(x)).SingleOrDefaultAsync();
                retval.Add(cat);
			}
			return retval;
		}

        public async Task<List<AttributesODTO>> GetAllAttributesValueByAttributeName(int categoryId)
        {
            var attributes = await _context.Attributes.Include(x => x.Categories).Where(x => x.CategoriesId == categoryId).Select(x => _mapper.Map<AttributesODTO>(x)).ToListAsync();
            return attributes;
        }

        public async Task<List<int?>> GetAllProductAttributes(int productId)
        {
            var productAttr = await _context.ProductAttributes.Where(x => x.ProductId == productId).Select(x => x.AttributesId).ToListAsync();
            return  productAttr;

		}

        public async Task<AttributesODTO> AddAttributes(AttributesIDTO attributesIDTO)
        {
            var attributes = _mapper.Map<Attributes>(attributesIDTO);
            attributes.AttributesId = 0;

            var isAttribute = await _context.Categories.Where(x => x.CategoryId == attributes.CategoriesId).Select(x => x.IsAttribute).FirstOrDefaultAsync();
            if (isAttribute == true && !_context.Attributes.Any(x => x.Value == attributes.Value && x.CategoriesId == attributes.CategoriesId))
            {
                _context.Attributes.Add(attributes);

                await SaveContextChangesAsync();

                return await GetAttributesById(attributes.AttributesId);
            }
            else
                return null;
        }

        public async Task<AttributesODTO> EditAttributes(AttributesIDTO attributesIDTO)
        {
            var attributes = _mapper.Map<Attributes>(attributesIDTO);
            _context.Entry(attributes).State = EntityState.Modified;
            await SaveContextChangesAsync();

            return await GetAttributesById(attributes.AttributesId);
        }

        public async Task<AttributesODTO> DeleteAttributes(int id)
        {
            var attributes = await _context.Attributes.FindAsync(id);
            if (attributes == null) return null;

            var attributesODTO = await GetAttributesById(id);

            var prodAttr = await _context.ProductAttributes.Where(x => x.AttributesId == attributes.AttributesId).ToListAsync();

            foreach (var item in prodAttr)
            {
                _context.ProductAttributes.Remove(item);
                await SaveContextChangesAsync();
            }

            _context.Attributes.Remove(attributes);
            await SaveContextChangesAsync();

            return attributesODTO;
        }

        #endregion Attributes
    }
}