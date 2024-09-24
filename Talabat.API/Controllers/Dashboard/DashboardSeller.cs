using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Talabat.API.Dots;
using Talabat.API.Helpers;
using Talabat.BLL.Interfaces;
using Talabat.DAL.Data;
using Talabat.DAL.Entities;

namespace Talabat.API.Controllers.Dashboard
{
    //[Authorize(Policy = "RequireSellerRole")]
    [Authorize]
    public class DashboardSeller : BaseApiController
    {
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<Image> _ImagesRepo;
        private readonly StoreContext _context;
        private readonly IMapper _mapper;

        public DashboardSeller(IGenericRepository<Product> productsRepo, IGenericRepository<Image> imagesRepo, StoreContext context, IMapper mapper)
        {
            _productsRepo = productsRepo;
            _mapper = mapper;
            _ImagesRepo = imagesRepo;
            _context = context;
        }

        [HttpPost("addproduct")]
        public async Task<ActionResult> AddProduct([FromForm] ProductToReturnDto ProductToReturnDto)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            ProductToReturnDto.PictureUrl = DocumentSettings.UploadFile(ProductToReturnDto.Url);
            var product = new Product()
            {
                Id = ProductToReturnDto.Id,
                Name = ProductToReturnDto.Name,
                Description = ProductToReturnDto.Description,
                PictureUrl = ProductToReturnDto.PictureUrl,
                ProductTypeId = ProductToReturnDto.ProductTypeId,
                ProductBrandId = ProductToReturnDto.ProductBrandId,
                Price = ProductToReturnDto.Price,
                Available = ProductToReturnDto.Available,
                Rating = ProductToReturnDto.Rating,
                productOwner = buyerEmail 
            };
            //var product = _mapper.Map<ProductToReturnDto, Product>(ProductToReturnDto);
            await _productsRepo.Add(product);
            if (ProductToReturnDto.ImagesUrl.Count > 0)
            {
                foreach (var file in ProductToReturnDto.ImagesUrl)
                {
                    var fileName = DocumentSettings.UploadFile(file);

                    var image = new Image
                    {
                        Name = fileName,
                        ProductId = product.Id
                    };
                    await _ImagesRepo.Add(image);

                }
            }


            return Ok(product);
        }


        [HttpPost("editproduct/{id}")]
        public async Task<ActionResult> EditProduct([FromRoute] int? id, [FromForm] ProductToReturnDto ProductToReturnDto)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            if(buyerEmail == ProductToReturnDto.productOwner)
            {
                if (id != ProductToReturnDto.Id)
                    return BadRequest();
                if (ModelState.IsValid)
                {
                    try
                    {
                        if (ProductToReturnDto.Url.Length > 0)
                        {
                            ProductToReturnDto.PictureUrl = DocumentSettings.UploadFile(ProductToReturnDto.Url);
                        }
                        var product = new Product()
                        {
                            Id = ProductToReturnDto.Id,
                            Name = ProductToReturnDto.Name,
                            Description = ProductToReturnDto.Description,
                            ProductTypeId = ProductToReturnDto.ProductTypeId,
                            PictureUrl = ProductToReturnDto.PictureUrl,
                            ProductBrandId = ProductToReturnDto.ProductBrandId,
                            Price = ProductToReturnDto.Price,
                            Rating = ProductToReturnDto.Rating,
                            productOwner = buyerEmail

                        };
                        //var product = _mapper.Map<ProductToReturnDto, Product>(ProductToReturnDto);
                        await _productsRepo.Update(product);

                        if (ProductToReturnDto.ImagesUrl.Count > 0)
                        {
                            var images = _context.Image.Where(c => c.ProductId == product.Id).ToArray();
                            foreach (var image in images)
                            {
                                DocumentSettings.DeleteFile(image.Name);
                                await _ImagesRepo.Delete(image);
                            }
                            foreach (var file in ProductToReturnDto.ImagesUrl)
                            {
                                var fileName = DocumentSettings.UploadFile(file);

                                var image = new Image
                                {
                                    Name = fileName,
                                    ProductId = product.Id
                                };
                                await _ImagesRepo.Add(image);

                            }

                        }
                        await _context.SaveChangesAsync();

                        return Ok(product);
                    }
                    catch
                    {
                        return Ok(ProductToReturnDto);
                    }
                }
                return Ok(ProductToReturnDto);
            }
            return BadRequest();


        }

        [HttpPost("deleteproduct/{id}")]
        public async Task<ActionResult> DeleteProduct([FromRoute] int? id, [FromForm] ProductToReturnDto ProductToReturnDto)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            if (buyerEmail == ProductToReturnDto.productOwner)
            {
                if (id != ProductToReturnDto.Id)
                    return BadRequest();
                try
                {
                    var product = new Product()
                    {
                        Id = ProductToReturnDto.Id,
                        Name = ProductToReturnDto.Name,
                        Description = ProductToReturnDto.Description,
                        ProductTypeId = ProductToReturnDto.ProductTypeId,
                        PictureUrl = ProductToReturnDto.PictureUrl,
                        ProductBrandId = ProductToReturnDto.ProductBrandId,
                        Price = ProductToReturnDto.Price
                    };
                    DocumentSettings.DeleteFile(ProductToReturnDto.PictureUrl);

                    //var product = _mapper.Map<ProductToReturnDto, Product>(ProductToReturnDto);
                    foreach (var file in ProductToReturnDto.Images)
                    {
                        DocumentSettings.DeleteFile(file.Name);
                    }
                    await _productsRepo.Delete(product);
                    return Ok(product);
                }
                catch (Exception ex)
                {
                    return Ok(ex);
                }
            }

            return BadRequest();
        }

    }
}
