using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Talabat.API.Dots;
using Talabat.API.Errors;
using Talabat.API.Helpers;
using Talabat.BLL.Interfaces;
using Talabat.BLL.Specifications;
using Talabat.DAL.Data;
using Talabat.DAL.Entities;
namespace Talabat.API.Controllers
{

    public class DashboardProduct : BaseApiController
    {
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<Image> _ImagesRepo;
        private readonly StoreContext _context;
        private readonly IMapper _mapper;

        public DashboardProduct(IGenericRepository<Product> productsRepo, IGenericRepository<Image> imagesRepo , StoreContext context, IMapper mapper)
        {
            _productsRepo = productsRepo;
            _mapper = mapper;
            _ImagesRepo = imagesRepo;
            _context = context;
        }

        [HttpPost("addproduct")]
        public async Task<ActionResult> AddProduct([FromForm] ProductToReturnDto ProductToReturnDto )
        {
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
                productOwner = "nove"
            };
            //var product = _mapper.Map<ProductToReturnDto, Product>(ProductToReturnDto);
            await _productsRepo.Add(product);
            if(ProductToReturnDto.ImagesUrl.Count > 0)
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
                        Rating = ProductToReturnDto.Rating

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
                        //foreach (var file in ProductToReturnDto.ImagesUrl)
                        //{
                        //    var fileName = DocumentSettings.UploadFile(file);

                        //    var existingImage = await _context.Image.FirstOrDefaultAsync(ds => ds.Id == file.);

                        //    if (existingImage != null)
                        //    {
                        //        existingImage.Name = fileName;
                        //        _context.Image.Update(existingImage);
                        //    }
                        //    else
                        //    {
                        //        var newImage = new Image
                        //        {
                        //            Name = fileName,
                        //            ProductId = product.Id
                        //        };
                        //        _context.Image.Add(newImage);
                        //    }
                        //}
                        await _context.SaveChangesAsync();
                    //var image = new Image
                    //{
                    //    //Id = product.Images
                    //    Name = fileName,
                    //    ProductId = product.Id
                    //};

                    //await _ImagesRepo.Add(image);


                    return Ok(product);
                }
                catch
                {
                    return Ok(ProductToReturnDto);
                }
            }
            return Ok(ProductToReturnDto);

        }

        [HttpPost("deleteproduct/{id}")]
        public async Task<ActionResult> DeleteProduct([FromRoute] int? id, [FromForm] ProductToReturnDto ProductToReturnDto)
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







    }
}
