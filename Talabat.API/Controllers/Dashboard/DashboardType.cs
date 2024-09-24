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

    public class DashboardType : BaseApiController
    {
        private readonly IGenericRepository<ProductType> _typesRepo;
        private readonly IMapper _mapper;
        private readonly StoreContext _context;

        public DashboardType(IGenericRepository<ProductType> typesRepo, IMapper mapper , StoreContext context)
        {
            _typesRepo = typesRepo;
            _mapper = mapper;
            _context = context;
        }

        [HttpPost("addType")]
        public async Task<IActionResult> AddType([FromForm] ProductBrandOrTypeToReturnDto producttypeToReturn)
        {
            producttypeToReturn.PictureUrl = DocumentSettings.UploadFile(producttypeToReturn.Url);
            var type = new ProductType()
            {
                Id = producttypeToReturn.Id,
                Name = producttypeToReturn.Name,
                PictureUrl = producttypeToReturn.PictureUrl
            };
            //var type = _mapper.Map<ProductBrandOrTypeToReturnDto, ProductType>(producttypeToReturn);
            await _typesRepo.Add(type);
            return Ok(type);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetType(int id)
        {
            var type = await _context.ProductTypes.FindAsync(id);

            return Ok(_mapper.Map<ProductType, ProductBrandOrTypeToReturnDto>(type));
        }

        [HttpPost("editType/{id}")]
        public async Task<ActionResult> EditType([FromRoute] int? id, [FromForm] ProductBrandOrTypeToReturnDto typeToReturnDto)
        {
            if (id != typeToReturnDto.Id)
               return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    typeToReturnDto.PictureUrl = DocumentSettings.UploadFile(typeToReturnDto.Url);

                    var type = new ProductType()
                    {
                        Id = typeToReturnDto.Id,
                        Name = typeToReturnDto.Name,
                        PictureUrl = typeToReturnDto.PictureUrl
                    };
                    //var type = _mapper.Map<ProductBrandOrTypeToReturnDto, ProductType>(typeToReturnDto);
                    await _typesRepo.Update(type);
                    return Ok(type);
                }
                catch
                {
                    return Ok(typeToReturnDto);
                }
            }
            return Ok(typeToReturnDto);

        }

        [HttpPost("deleteType/{id}")]
        public async Task<ActionResult> DeleteProduct([FromRoute] int? id, [FromForm] ProductBrandOrTypeToReturnDto typeToReturnDto)
        {
            if (id != typeToReturnDto.Id)
                return BadRequest();
            try
            {
                var type = new ProductType()
                {
                    Id = typeToReturnDto.Id,
                    Name = typeToReturnDto.Name,
                    PictureUrl = typeToReturnDto.PictureUrl
                };
                //var type = _mapper.Map<ProductBrandOrTypeToReturnDto, ProductType>(typeToReturnDto);
                DocumentSettings.DeleteFile(type.PictureUrl);
                //DocumentSettings.DeleteFile(ProductType.PictureUrl, "products");
                await _typesRepo.Delete(type);
                return Ok(type);
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }

        }
    }
}
