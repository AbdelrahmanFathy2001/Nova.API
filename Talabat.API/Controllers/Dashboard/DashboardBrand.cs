using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Talabat.API.Dots;
using Talabat.API.Errors;
using Talabat.API.Helpers;
using Talabat.BLL.Interfaces;
using Talabat.BLL.Specifications;
using Talabat.DAL.Data;
using Talabat.DAL.Entities;

namespace Talabat.API.Controllers.Dashboard
{

    public class DashboardBrand : BaseApiController
    {
        private readonly IGenericRepository<ProductBrand> _brandsRepo;
        private readonly IMapper _mapper;
        private readonly StoreContext _context;

        public DashboardBrand(IGenericRepository<ProductBrand> brandsRepo, IMapper mapper , StoreContext context)
        {
            _brandsRepo = brandsRepo;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetType(int id)
        {
            var type = await _context.ProductBrands.FindAsync(id);

            return Ok(_mapper.Map<ProductBrand, ProductBrandOrTypeToReturnDto>(type));
        }


        [HttpPost("addbrand")]
        public async Task<ActionResult> AddBrand([FromForm] ProductBrandOrTypeToReturnDto productBrandToReturn)
        {
            productBrandToReturn.PictureUrl = DocumentSettings.UploadFile(productBrandToReturn.Url);
            var brand = new ProductBrand()
            {
                Id = productBrandToReturn.Id,
                Name = productBrandToReturn.Name,
                PictureUrl = productBrandToReturn.PictureUrl
            };
            //var brand = _mapper.Map<ProductBrandOrTypeToReturnDto, ProductBrand>(productBrandToReturn);
            await _brandsRepo.Add(brand);
            return Ok(brand);
        }


        [HttpPost("editBrand/{id}")]
        public async Task<ActionResult> EditBrand([FromRoute] int? id, [FromForm] ProductBrandOrTypeToReturnDto productBrandToReturn)
        {
            if (id != productBrandToReturn.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    productBrandToReturn.PictureUrl = DocumentSettings.UploadFile(productBrandToReturn.Url);
                    var brand = new ProductBrand()
                    {
                        Id = productBrandToReturn.Id,
                        Name = productBrandToReturn.Name,
                        PictureUrl = productBrandToReturn.PictureUrl
                    };
                    //var brand = _mapper.Map<ProductBrandOrTypeToReturnDto, ProductBrand>(productBrandToReturn);
                    await _brandsRepo.Update(brand);
                    return Ok(brand);
                }
                catch
                {
                    return Ok(productBrandToReturn);
                }
            }
            return Ok(productBrandToReturn);

        }

        [HttpPost("deletebrand/{id}")]
        public async Task<ActionResult> DeleteBrandl([FromRoute] int? id, [FromForm] ProductBrandOrTypeToReturnDto productBrandToReturn)
        {
            if (id != productBrandToReturn.Id)
                return BadRequest();
            try
            {
                var brand = new ProductBrand()
                {
                    Id = productBrandToReturn.Id,
                    Name = productBrandToReturn.Name,
                    PictureUrl = productBrandToReturn.PictureUrl
                };
                //var brand = _mapper.Map<ProductBrandOrTypeToReturnDto, ProductBrand>(productBrandToReturn);
                DocumentSettings.DeleteFile(brand.PictureUrl);
                //DocumentSettings.DeleteFile(ProductType.PictureUrl, "products");
                await _brandsRepo.Delete(brand);
                return Ok(brand);
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }

        }
    }
}
