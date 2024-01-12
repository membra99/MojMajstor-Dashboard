using AutoMapper;
using Entities.Context;
using Entities.Universal.MainData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using OfficeOpenXml;
using Services.AWS;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;
using static Universal.DTO.CommonModels.CommonModels;
using Seo = Entities.Universal.MainData.Seo;

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

		public async Task<MediaODTO> UploadProductImage(AWSFileUpload awsFile, string mediaType, int? productId)
		{
			string successUpload = "";

			if (awsFile.Attachments.Count > 0)
				successUpload = await _AWSS3FileService.UploadFile(awsFile);

			if (successUpload != null)
			{
				var key = await _AWSS3FileService.FilesListSearch(successUpload);
				var media = new Media();
				media.ProductId = productId;
				media.Extension = awsFile.Attachments.First().FileName.Split('.')[1];
				media.Src = key.First();
				media.MediaTypeId = _context.MediaTypes.FirstOrDefault(x => x.MediaTypeName == mediaType).MediaTypeId;
				var index = media.Src.LastIndexOf('/');
				media.MetaTitle = media.Src.Substring(index + 1);
				_context.Medias.Add(media);
				await _context.SaveChangesAsync();
				return _mapper.Map<MediaODTO>(media);
			}
			else
			{
				return null;
			}
		}

		public async Task SetProperGallery(List<string> galleryImg, int productId)
		{
			if(galleryImg == null)
			{
				var mediaIds = await _context.Medias.Where(x => x.ProductId == productId && x.MediaTypeId == 5).ToListAsync();
				_context.Medias.RemoveRange(mediaIds);
				await SaveContextChangesAsync();
			}
			else
			{
				var galleryDb = await _context.Medias.Where(x => x.ProductId == productId && x.MediaTypeId == 5).Select(x => x.Src).ToListAsync();
				List<string> itemsOnlyInGalleryDb = galleryDb.Except(galleryImg).ToList();
				foreach (var item in itemsOnlyInGalleryDb)
				{
					var mediaId = await _context.Medias.Where(x => x.ProductId == productId && x.MediaTypeId == 5 && x.Src == item).Select(x => x.MediaId).SingleOrDefaultAsync();
					var mediaForDel = await _context.Medias.FindAsync(mediaId);
					_context.Medias.Remove(mediaForDel);
					await SaveContextChangesAsync();
				}
			}
		}

		public async Task<List<ProductODTO>> ImportFromExcel(IFormFile file)
		{
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var list = new List<Product>();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowcount = worksheet.Dimension.Rows;
                    for (int i = 2; i <= rowcount; i++)
                    {
                        var catName = await _context.Categories.Where(x => x.CategoryName == worksheet.Cells[i, 3].Value.ToString().Trim()).SingleOrDefaultAsync();
						if (catName == null) throw new Exception("This category name does not exists");

                        Seo seo = new Seo();
						seo.GoogleDesc = worksheet.Cells[i, 10].Value.ToString().Trim();
						seo.GoogleKeywords = worksheet.Cells[i, 11].Value.ToString().Trim();
						seo.LanguageID = Convert.ToInt32(worksheet.Cells[i, 13].Value.ToString().Trim());
						_context.Seos.Add(seo);
						await SaveContextChangesAsync();


                        list.Add(new Product
						{
							ProductId = 0,
							ProductName = worksheet.Cells[i, 1].Value.ToString().Trim(),
							BestProduct = Convert.ToBoolean(worksheet.Cells[i, 2].Value.ToString().Trim()),
							CategoriesId = catName.CategoryId,
							CreatedAt = DateTime.Now,
							Description = worksheet.Cells[i, 4].Value.ToString().Trim(),
                            IsOnSale = Convert.ToBoolean(worksheet.Cells[i, 5].Value.ToString().Trim()),
							Price = Convert.ToInt32(worksheet.Cells[i, 6].Value.ToString().Trim()),
							ProductCode = worksheet.Cells[i, 7].Value.ToString().Trim(),
							Recommended = Convert.ToBoolean(worksheet.Cells[i, 8].Value.ToString().Trim()),
							Specification = worksheet.Cells[i, 9].Value.ToString().Trim(),
							UpdatedAt = DateTime.Now,
							SeoId = seo.SeoId,
							Declaration = null,
							IsActive = true,
							Quantity = Convert.ToInt32(worksheet.Cells[i, 12].Value.ToString().Trim()),
							LanguageID = Convert.ToInt32(worksheet.Cells[i, 13].Value.ToString().Trim())
                        }) ;
                    }
                }
            }
			_context.Products.AddRange(list);
			await SaveContextChangesAsync();
            return list.Select(x => _mapper.Map<ProductODTO>(x)).ToList();
        }

		#endregion FileUploads

		#region Categories

		private IQueryable<CategoriesODTO> GetCategories(int id, int langId)
		{
			return from x in _context.Categories
				   where (id == 0 || x.CategoryId == id)
				   && (langId == 0 || x.LanguageID == langId)
				   select _mapper.Map<CategoriesODTO>(x);
		}

		public async Task<CategoriesODTO> GetCategoriesById(int id)
		{
			return await _context.Categories.Include(x => x.Media).Include(x => x.Seo).Where(x => x.CategoryId == id).Select(x => _mapper.Map<CategoriesODTO>(x)).SingleOrDefaultAsync();
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
						seo = await AddSeo(categoriesIDTO.SeoIDTO.GoogleDesc, categoriesIDTO.SeoIDTO.GoogleKeywords, categoriesIDTO.SeoIDTO.LanguageID);
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
				categories.LanguageID = categoriesIDTO.LanguageID;
				categories.IsActive = true;
				categories.MediaId = categoriesIDTO.MediaId;
				_context.Entry(categories).State = EntityState.Modified;
				await SaveContextChangesAsync();
			}
			return null;
		}

		public async Task<CategoriesODTO> EditCategory(CategoriesIDTO categoriesIDTO)
		{
			var cat = _mapper.Map<Categories>(categoriesIDTO);
			if (categoriesIDTO.IsImageChanged == "true")
				cat.MediaId = null;
			_context.Entry(cat).State = EntityState.Modified;
			if (categoriesIDTO.CategoryImage == null && categoriesIDTO.IsImageChanged != "true")
			{
				_context.Entry(cat).Property(x => x.MediaId).IsModified = false;
			}
			await SaveContextChangesAsync();

			return await GetCategoriesById(cat.CategoryId);
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
					var media = await _context.Medias.Where(x => x.ProductId == product.ProductId).ToListAsync();
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

		private IQueryable<ProductODTO> GetProducts(int id, int langId)
		{
			return from x in _context.Products
				   .Include(x => x.Categories)
				   where (id == 0 || x.ProductId == id) &&
				   (langId == 0 || x.LanguageID == langId) &&
				   (x.IsActive == true)
				   select _mapper.Map<ProductODTO>(x);
		}

		public async Task<ProductODTO> GetProductsById(int id)
		{
			return await GetProducts(id, 0).AsNoTracking().SingleOrDefaultAsync();
		}

		public async Task<List<SaleTypeODTO>> GetAllSaleType()
		{
			return await _context.SaleTypes.Select(x => _mapper.Map<SaleTypeODTO>(x)).ToListAsync();
		}

		public async Task<ProductIDTO> GetProductsByIdForEdit(int id)
		{
			var product = _mapper.Map<ProductIDTO>(await GetProducts(id, 0).AsNoTracking().SingleOrDefaultAsync());
			product.FeatureImg = await _context.Medias.Where(x => x.ProductId == product.ProductId && x.MediaTypeId == 4).Select(x => x.Src).SingleOrDefaultAsync();
			product.GalleyImg = await _context.Medias.Where(x => x.ProductId == product.ProductId && x.MediaTypeId == 5).Select(x => x.Src).ToListAsync();
			product.SaleIDTO = _mapper.Map<SaleIDTO>(await _context.Sales.FirstOrDefaultAsync(x => x.ProductId == product.ProductId));
			product.SeoIDTO = _mapper.Map<SeoIDTO>(await _context.Seos.FirstOrDefaultAsync(x => x.SeoId == product.SeoId));
			if (product.SaleIDTO != null)
			{
				DateTime startdate = DateTime.ParseExact(product.SaleIDTO.StartDate, "dd-MMM-yy HH:mm:ss", null);
				DateTime enddate = DateTime.ParseExact(product.SaleIDTO.EndDate, "dd-MMM-yy HH:mm:ss", null);
				product.SaleIDTO.StartDate = startdate.ToString("yyyy-MM-dd");
				product.SaleIDTO.EndDate = enddate.ToString("yyyy-MM-dd");
			}
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

		public async Task<List<ProductODTO>> GetAllProducts(int langId)
		{
			return await GetProducts(0, langId).AsNoTracking().ToListAsync();
		}

		public async Task<ProductODTO> AddProduct(ProductIDTO productIDTO)
		{
			int seo = 0;
			if (productIDTO.SeoIDTO.GoogleKeywords != null || productIDTO.SeoIDTO.GoogleKeywords != null)
			{
				seo = await AddSeo(productIDTO.SeoIDTO.GoogleDesc, productIDTO.SeoIDTO.GoogleKeywords, productIDTO.SeoIDTO.LanguageID);
			}

			var product = _mapper.Map<Product>(productIDTO);
			product.ProductId = 0;
			product.CreatedAt = DateTime.Now;
			product.SeoId = (seo != 0) ? seo : null;
			_context.Products.Add(product);

			await SaveContextChangesAsync();

			if (product.IsOnSale == true)
			{
				try
				{
					SaleIDTO sale = new SaleIDTO();
					sale.Value = productIDTO.SaleIDTO.Value;
					sale.SaleTypeId = productIDTO.SaleIDTO.SaleTypeId;
					DateTime startDate = DateTime.ParseExact(productIDTO.SaleIDTO.StartDate, "MM/dd/yyyy", null);
					DateTime endDate = DateTime.ParseExact(productIDTO.SaleIDTO.EndDate, "MM/dd/yyyy", null);
					sale.StartDate = startDate.ToString("yyyy-MM-dd");
					sale.EndDate = endDate.ToString("yyyy-MM-dd");
					sale.IsActive = true;
					sale.ProductId = product.ProductId;
					var SaleForDB = _mapper.Map<Sale>(sale);
					_context.Sales.Add(SaleForDB);
					await SaveContextChangesAsync();
				} catch(Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
				
			}

			return await GetProductsById(product.ProductId);
		}

		public async Task<int> AddSeo(string googleDesc, string googleKeywords, int? langId)
		{
			Seo seo = new Seo();
			seo.SeoId = 0;
			seo.GoogleDesc = googleDesc;
			seo.GoogleKeywords = googleKeywords;
			seo.LanguageID = langId != null ? langId : null;
			try
			{
				_context.Seos.Add(seo);
				await SaveContextChangesAsync();
			} 
			catch(Exception ex)
			{
				Console.WriteLine(ex.ToString());	
			}


			return await (from x in _context.Seos
						  where x.SeoId == seo.SeoId
						  select x.SeoId).SingleOrDefaultAsync();
		}

		public async Task EditFeaturedImage(int productId, int mediaId)
		{
			var currentImage = _context.Medias.Where(x => x.ProductId == productId && x.MediaTypeId == 4).FirstOrDefault();
			if (currentImage != null)
			{
				currentImage.ProductId = null;
				_context.Entry(currentImage).State = EntityState.Modified;
				await SaveContextChangesAsync();

				var newMedia = await _context.Medias.Where(x => x.MediaId == mediaId).SingleOrDefaultAsync();
				if (newMedia.ProductId == null)
				{
					newMedia.ProductId = productId;
					newMedia.MediaTypeId = 4;
					_context.Entry(newMedia).State = EntityState.Modified;
					await SaveContextChangesAsync();
				}
				else
				{
					Media newInsertMedia = new Media();
					newInsertMedia.ProductId = productId;
					newInsertMedia.MediaTypeId = 4;
					newInsertMedia.Src = await _context.Medias.Where(x => x.MediaId == mediaId).Select(x => x.Src).SingleOrDefaultAsync();
					newInsertMedia.Extension = Path.GetExtension(newInsertMedia.Src);
					int lastIndex = newInsertMedia.Src.LastIndexOf('/');
					newInsertMedia.MetaTitle = newInsertMedia.Src.Substring(lastIndex + 1);
					_context.Medias.Add(newInsertMedia);
					await SaveContextChangesAsync();
				}
			}
			else
			{
				Media newInsertMedia = new Media();
				newInsertMedia.ProductId = productId;
				newInsertMedia.MediaTypeId = 4;
				newInsertMedia.Src = await _context.Medias.Where(x => x.MediaId == mediaId).Select(x => x.Src).SingleOrDefaultAsync();
				newInsertMedia.Extension = Path.GetExtension(newInsertMedia.Src);
				int lastIndex = newInsertMedia.Src.LastIndexOf('/');
				newInsertMedia.MetaTitle = newInsertMedia.Src.Substring(lastIndex + 1);
				_context.Medias.Add(newInsertMedia);
				await SaveContextChangesAsync();
			}
		}

		public async Task<ParentChildODTO> GetTree(int Id, int langId)
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
			if (productIDTO.SeoIDTO.GoogleDesc != null || productIDTO.SeoIDTO.GoogleKeywords != null || productIDTO.SeoId != 0)
			{
				var seo = await _context.Seos.Where(x => x.SeoId == productIDTO.SeoId).SingleOrDefaultAsync();
				if (seo == null)
				{
					SeoIDTO seoIDTO = new SeoIDTO();
					seoIDTO.GoogleDesc = productIDTO.SeoIDTO.GoogleDesc;
					seoIDTO.GoogleKeywords = productIDTO.SeoIDTO.GoogleKeywords;

					var s = _mapper.Map<Seo>(seoIDTO);
					_context.Seos.Add(s);
					await SaveContextChangesAsync();

					productIDTO.SeoId = s.SeoId;
				}
				else
				{
					seo.GoogleDesc = productIDTO.SeoIDTO.GoogleDesc;
					seo.GoogleKeywords = productIDTO.SeoIDTO.GoogleKeywords;
					_context.Entry(seo).State = EntityState.Modified;
				}
			}
			var product = _mapper.Map<Product>(productIDTO);
			_context.Entry(product).State = EntityState.Modified;

			if (product.SeoId == 0)
			{
				product.SeoId = null;
			}
			product.LanguageID = 1; //TODO Set LanguageID dinamicaly
	
			await SaveContextChangesAsync();

			return await GetProductsById(product.ProductId);
		}

		public async Task<ProductODTO> DeleteProduct(int id)
		{
			var product = await _context.Products.FindAsync(id);
			if (product == null) return null;

			var productODTO = await GetProductsById(id);

			product.IsActive = false;

			try
			{
				_context.Entry(product).State = EntityState.Modified;
			} catch(Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			
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
				   .Include(x => x.SiteContentType)
				   where (id == 0 || x.SiteContentId == id)
				   select _mapper.Map<SiteContentODTO>(x);
		}

		public async Task<SiteContentODTO> GetSiteContentById(int id)
		{
			return await GetSiteContent(id).AsNoTracking().SingleOrDefaultAsync();
		}

		public async Task<List<SiteContentODTO>> GetAllSiteContentByType(string Type, int langId)
		{
			var siteContentType = await _context.SiteContentTypes.Where(x => x.SiteContentTypeName == Type).Select(x => x.SiteContentTypeId).SingleOrDefaultAsync();
			return await _context.SiteContents.Where(x => x.SiteContentTypeId == siteContentType && x.IsActive == true && x.LanguageID == langId).Select(x => _mapper.Map<SiteContentODTO>(x)).ToListAsync();
		}

		public async Task<SiteContentIDTO> GetSiteContentByIdForEdit(int id)
		{
			var siteContent = _mapper.Map<SiteContentIDTO>(await _context.SiteContents.Include(x => x.Media).Where(x => x.SiteContentId == id).AsNoTracking().SingleOrDefaultAsync());
			siteContent.TagODTOs = await _context.Tags.Select(x => new TagODTO { TagId = x.TagId, Title = x.Title, Description = x.Description, LanguageID = x.LanguageID} ).ToListAsync();
			if(siteContent.SeoId != null)
			{
				siteContent.SeoIDTO = new SeoIDTO();
				var seo = await _context.Seos.Where(x => x.SeoId == siteContent.SeoId).SingleOrDefaultAsync();
				siteContent.SeoIDTO.GoogleDesc = seo.GoogleDesc;
				siteContent.SeoIDTO.GoogleKeywords = seo.GoogleKeywords;
				siteContent.SeoIDTO.LanguageID = seo.LanguageID;
				siteContent.SeoIDTO.SeoId = seo.SeoId;
			}
			return siteContent;
		}

		public async Task<List<TagODTO>> GetTags()
		{
			return await _context.Tags.Select(x => _mapper.Map<TagODTO>(x)).ToListAsync();
		}

		public async Task<SiteContentODTO> AddSiteContent(SiteContentIDTO siteContentIDTO)
		{
			int seo = 0;
			if (siteContentIDTO.SeoIDTO.GoogleKeywords != null || siteContentIDTO.SeoIDTO.GoogleKeywords != null)
			{
				seo = await AddSeo(siteContentIDTO.SeoIDTO.GoogleDesc, siteContentIDTO.SeoIDTO.GoogleKeywords, siteContentIDTO.SeoIDTO.LanguageID);
			}

			var siteContent = _mapper.Map<SiteContent>(siteContentIDTO);
			siteContent.SiteContentId = 0;
			siteContent.SeoId = (seo != 0) ? seo : null;
			_context.SiteContents.Add(siteContent);

			await SaveContextChangesAsync();

			return await GetSiteContentById(siteContent.SiteContentId);
		}

		public async Task<SiteContentODTO> EditSiteContent(SiteContentIDTO siteContentIDTO)
		{
			var siteContent = _mapper.Map<SiteContent>(siteContentIDTO);
			siteContent.IsActive = true;
			if (siteContentIDTO.IsImageChanged == "true" && siteContentIDTO.MediaId == null)
				siteContent.MediaId = null;
			
			if(siteContentIDTO.SeoIDTO.SeoId != null)
			{
				var seo = await _context.Seos.Where(x => x.SeoId == siteContentIDTO.SeoIDTO.SeoId).SingleOrDefaultAsync();
				seo.GoogleDesc = siteContentIDTO.SeoIDTO.GoogleDesc;
				seo.GoogleKeywords = siteContentIDTO.SeoIDTO.GoogleKeywords;
				_context.Entry(seo).State = EntityState.Modified;
				await SaveContextChangesAsync();
				siteContent.SeoId = siteContentIDTO.SeoIDTO.SeoId;
			}
			else if(siteContentIDTO.SeoIDTO.GoogleKeywords != null || siteContentIDTO.SeoIDTO.GoogleDesc != null)
			{
				var newSeo = _mapper.Map<Seo>(siteContentIDTO.SeoIDTO);
				newSeo.SeoId = 0;
				_context.Seos.Add(newSeo);
				await SaveContextChangesAsync();
				siteContent.SeoId = newSeo.SeoId;
			}
			
			_context.Entry(siteContent).State = EntityState.Modified;
			if (siteContentIDTO.Image == null && siteContentIDTO.IsImageChanged != "true")
			{
				_context.Entry(siteContent).Property(x => x.MediaId).IsModified = false;
			}

			

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

		public async Task<bool> CreateInvoice(InvoiceIDTO html)
		{
			List<string> companyData = new List<string> { "ConcordSoft", "Prozivka 18", "24000 Subotica", "TaxId: 23242424", "Bank Account:<br>160-000000111000-12" }; //hardcoded seller company data that we can change depending on customer
			List<string> buyerData = new List<string> { "Invoice No: <b>R-" + html.OrderNumber.ToString().PadLeft(4, '0') + "/" + html.Dateofpayment.Split('.')[2] +"</b>",
				"Date: "+html.Dateofpayment.Split(' ')[0], "Buyer: "+html.BuyerName, "Address: "+html.Address, "City: "+html.City }; //dynamic buyer data

			string htmlForRender = html.htmltable;
			var pdf = new ChromePdfRenderer();
			pdf.RenderingOptions.CustomCssUrl = Directory.GetCurrentDirectory() + @"\wwwroot\css\RenderHtml.css";
			string headerHtml = "<div class='headerFlex'>" +
				"<div><ul class='noSymbol'>";
			foreach (var data in companyData) {
				headerHtml += "<li>" + data + "</li>";
			}
			headerHtml += "</ul></div>";
			headerHtml += "<div><ul class='noSymbol'>";
			foreach (var data in buyerData)
			{
				headerHtml += "<li>" + data + "</li>";
			}
			headerHtml += "</ul></div></div>";

			PdfDocument doc = pdf.RenderHtmlAsPdf(headerHtml + "<table class='table1'>" + htmlForRender +
				"<tr><td class='no-border' colspan=4></td><td>Sum</td><td>" + html.TotalPrice + "</td></tr>" +
				"<tr><td class='no-border' colspan=4></td><td>Shipping</td><td>0.00</td></tr>" +
				"<tr><td class='no-border' colspan=4></td><td>Total</td><td>" + html.TotalPrice + "</td></tr>" + "</table>");

			byte[] pdfbytes = doc.Stream.ToArray();
			var stream = new MemoryStream(pdfbytes);
			IFormFile file = new FormFile(stream, 0, pdfbytes.Length, "test", "test.pdf");
			AWSFileUpload aws = new AWSFileUpload();
			aws.Attachments = new List<IFormFile>();
			aws.Attachments.Add(file);
			var mediaOdto = await UploadProductImage(aws, "Invoice", null);

            InvoiceEntitiesIDTO entitiesIDTO = new InvoiceEntitiesIDTO();
            entitiesIDTO.InvoiceId = 0;
			entitiesIDTO.MediaId = mediaOdto.MediaId;
            entitiesIDTO.PdfName = "test.pdf";
            entitiesIDTO.DateOfPayment = Convert.ToDateTime(html.Dateofpayment);
            entitiesIDTO.CreatedAt = DateTime.Now;
            entitiesIDTO.UpdatedAt = DateTime.Now;
            var invoice = _mapper.Map<Invoice>(entitiesIDTO);
            _context.Invoices.Add(invoice);
            await SaveContextChangesAsync();


			return true;
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
            fullOrder.CreatedAt = order.OrderDate.ToString();
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
				productODTO.Sum = productODTO.Quantity * (int)productODTO.Price;

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
			return await GetDeclarations(id).AsNoTracking().FirstOrDefaultAsync();
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
			await _context.SaveChangesAsync();

			return await GetDeclarationById(declaration.DeclarationId);
		}

		public async Task<DeclarationODTO> DeleteDeclaration(int id)
		{
			var product = await _context.Products.Where(x => x.DeclarationId == id).ToListAsync();
			foreach (var item in product)
			{
				item.DeclarationId = null;
				_context.Entry(item).State = EntityState.Modified;
			}

			await SaveContextChangesAsync();
			var declaration = await _context.Declarations.FindAsync(id);
			if (declaration == null) return null;

			var declarationODTO = await GetDeclarationById(id);

			_context.Declarations.Remove(declaration);
			await SaveContextChangesAsync();

			return declarationODTO;
		}

		public async Task<TagODTO> DeleteTag(int id)
		{
			var tag = await _context.Tags.FindAsync(id);
			if (tag == null) return null;

			var sitesContent = await _context.SiteContents.Where(x => x.TagId == id).ToListAsync();
			foreach (var site in sitesContent)
			{
				site.TagId = null;
				_context.Entry(site).State = EntityState.Modified;
				await _context.SaveChangesAsync();
			}

			var tagODTO = await GetTagById(id);

			_context.Tags.Remove(tag);
			await SaveContextChangesAsync();

			return tagODTO;
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

		public async Task<string> GetStringForModal(int mediaId)
		{
			var retVal = "";
			var user = await _context.Users.Where(x => x.MediaId == mediaId).FirstOrDefaultAsync();
			if (user != null) retVal += "This picture is connected with User<br>";

			var tag = await _context.Tags.Where(x => x.MediaId == mediaId).FirstOrDefaultAsync();
			if (tag != null) retVal+= "This picture is connected with Tag<br>";

			var siteContent = await _context.SiteContents.Where(x => x.MediaId == mediaId).FirstOrDefaultAsync();
			if(siteContent != null)
			{
				if(siteContent.SiteContentTypeId == 1)
				{
					retVal+= "This picture is connected with Page<br>";
				}
				else
				{
					retVal+= "This picture is connected with Blog<br>";
				}

			}

			var category = await _context.Categories.Where(x => x.MediaId == mediaId).FirstOrDefaultAsync();
			if (category != null) retVal+= "This picture is connected with Category<br>";

			return retVal;
		}

		public async Task<List<MediaODTO>> GetAllImagesRoute()
		{
            string[] cars = { "png", "jpg", "webp", "jiff" };
			return await _context.Medias.Where(x => cars.Contains(x.Extension)).Select(x => _mapper.Map<MediaODTO>(x)).ToListAsync();
		}

		public async Task EditMediaImageMetaProperties(MediaIDTO mediaIDTO)
		{
			var mediaImage = await _context.Medias.SingleOrDefaultAsync(x => x.MediaId == mediaIDTO.MediaId);
			mediaImage.MetaTitle = mediaIDTO.MetaTitle;
			mediaImage.MetaDescription = mediaIDTO.MetaDescription;
			mediaImage.AltTitle = mediaIDTO.AltTitle;

			await SaveContextChangesAsync();
		}

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
			var category = await _context.Categories.Where(x => x.IsAttribute == false && x.IsActive == true && x.ParentCategoryId != null).Select(x => new { x.CategoryId, x.ParentCategoryId }).ToListAsync();
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
			return productAttr;
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

		#region Language

		private IQueryable<LanguageODTO> GetLanguage(int id)
		{
			return from x in _context.Languages
				   where (id == 0 || x.LanguageID == id)
				   select _mapper.Map<LanguageODTO>(x);
		}

		public async Task<LanguageODTO> GetLanguageById(int id)
		{
			return await GetLanguage(id).AsNoTracking().SingleOrDefaultAsync();
		}
		public async Task<List<LanguageODTO>> GetAllLanguages()
		{
			return await GetLanguage(0).AsNoTracking().ToListAsync();
		}
		public async Task<LanguageODTO> EditLanguage(LanguageIDTO languageIDTO)
		{
			var language = _mapper.Map<Language>(languageIDTO);

			_context.Entry(language).State = EntityState.Modified;

			await SaveContextChangesAsync();

			return await GetLanguageById(language.LanguageID);
		}

		public async Task<LanguageODTO> AddLanguage(LanguageIDTO languageIDTO)
		{
			var language = _mapper.Map<Language>(languageIDTO);
			language.LanguageID = 0;
			_context.Languages.Add(language);

			await SaveContextChangesAsync();

			return await GetLanguageById(language.LanguageID);
		}

		public async Task<LanguageODTO> DeleteLanguage(int id)
		{
			var language = await _context.Languages.FindAsync(id);
			if (language == null) return null;

			var languageODTO = await GetLanguageById(id);
			_context.Languages.Remove(language);
			await SaveContextChangesAsync();
			return languageODTO;
		}

		#endregion Language

		#region Invoices

		public async Task<List<InvoiceEntitiesODTO>> GetAllInvoices()
		{
			return await _context.Invoices.Include(x => x.Media).Select(x => _mapper.Map<InvoiceEntitiesODTO>(x)).ToListAsync();
		}

		public async Task<Stream> GetStreamForInvoice(string path)
		{
			return await _AWSS3FileService.GetFile(path);
		}

		#endregion

		#region Tag

		public async Task<TagODTO> AddTag(TagIDTO tagIDTO)
		{
			tagIDTO.LanguageID = 1; //TODO - Set LanguageID dinamicaly
			var tag = _mapper.Map<Tag>(tagIDTO);
			tag.TagId = 0;
			_context.Tags.Add(tag);
			await SaveContextChangesAsync();

			return await _context.Tags.Where(x => x.TagId == tag.TagId).Select(x => _mapper.Map<TagODTO>(x)).SingleOrDefaultAsync();
		}

		public async Task<TagODTO> GetTagById(int id)
		{
			return await _context.Tags.Where(x => x.TagId == id).Select(x => _mapper.Map<TagODTO>(x)).SingleOrDefaultAsync();
		}

		public async Task<TagODTO> EditTag(TagIDTO tagIDTO)
		{
			var tag = _mapper.Map<Tag>(tagIDTO);
			if (tagIDTO.IsImageChanged == "true")
				tagIDTO.MediaId = null;
			
			if(tagIDTO.TagImage == null && tagIDTO.IsImageChanged != "true")
			{
				_context.Entry(tag).Property(x => x.MediaId).IsModified = false;
			}
			try
			{
				_context.Entry(tag).State = EntityState.Modified;
				await SaveContextChangesAsync();
			} 
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}


			return await GetTagById(tag.TagId);
		}

		private IQueryable<TagIDTO> GetTagsIDTO(int id)
		{
			return from x in _context.Tags
				   .Include(x => x.Media)
				   where (id == 0 || x.TagId == id)
				   select _mapper.Map<TagIDTO>(x);
		}

		public async Task<TagIDTO> GetTagForEditById(int id)
		{
			return await GetTagsIDTO(id).AsNoTracking().SingleOrDefaultAsync();
		}

		#endregion
	}
}