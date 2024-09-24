 using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    ///[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IConfiguration _configuration;
        private readonly IGenericRepository<ProductBrand> _brandsRepo;
        private readonly IGenericRepository<ProductType> _typesRepo;
        private readonly IMapper _mapper;
        private readonly StoreContext _context;


        public ProductsController(StoreContext context , IGenericRepository<Product> productsRepo, IConfiguration configuration, IGenericRepository<ProductBrand> brandsRepo, IGenericRepository<ProductType> typesRepo, IMapper mapper)
        {
            _productsRepo = productsRepo;
            _configuration = configuration;
            _brandsRepo = brandsRepo;
            _typesRepo = typesRepo;
            _mapper = mapper;
            _context = context;
        }


        [CachedResponse(600)]
        [HttpGet("Products")]

        public async Task<ActionResult<Pagination<ProductsToReturnDto>>> GetProducts([FromQuery] ProductSpecParams productSpecParams)
        {

            var spec = new ProductWithTypeAndBrandSpecification(productSpecParams);

            var products = await _productsRepo.GetAllWithSpecAsync(spec);

            var Data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductsToReturnDto>>(products);

            var countSpec = new ProductWithFiltersForCountSpecification(productSpecParams);

            var Count = await _productsRepo.GetCountAsync(countSpec);

            return Ok(new Pagination<ProductsToReturnDto>(productSpecParams.PageIndex, productSpecParams.PageSize, Count ,Data));
        }


        [CachedResponse(600)]
        [HttpGet("novaProducts")]

        public async Task<ActionResult<Pagination<ProductsToReturnDto>>> GetNovaProducts([FromQuery] ProductSpecParameters productSpecParams)
        {

            var spec = new ProductWithTypeAndBrandSpecification(productSpecParams);

            var products = await _productsRepo.GetAllWithSpecAsync(spec);

            var Novaproduct = products.Where(p => p.productOwner == "nova").ToList();

            var Data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductsToReturnDto>>(Novaproduct);

            return Ok(new PaginationProduct<ProductsToReturnDto>( Data));
        }

        [CachedResponse(600)]
        [HttpGet("SellerProducts")]

        public async Task<ActionResult<Pagination<ProductsToReturnDto>>> GetSellerProducts([FromQuery] ProductSpecParameters productSpecParams)
        {

            var spec = new ProductWithTypeAndBrandSpecification(productSpecParams);

            var products = await _productsRepo.GetAllWithSpecAsync(spec);

            var Novaproduct = products.Where(p => p.productOwner != "nova").ToList();

            var Data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductsToReturnDto>>(Novaproduct);

            return Ok(new PaginationProduct<ProductsToReturnDto>(Data));
        }


        [CachedResponse(600)]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProductById(int id)
        {
            var spec = new ProductWithTypeAndBrandSpecification(id);
            var product = await _productsRepo.GetByIdWithSpecAsync(spec);
            var productDto = _mapper.Map<Product, ProductToReturnDto>(product);
            if (productDto == null) return NotFound(new ApiResponse(404));

            return Ok(productDto);
        }


        [CachedResponse(600)]
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var brands = await _brandsRepo.GetAllAsync();
            var Data = _mapper.Map<IReadOnlyList<ProductBrand>, IReadOnlyList<ProductBrandOrTypeToReturnDto>>(brands);
            return Ok(Data);
        }

        [CachedResponse(600)]
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetTypes()
        {
            var types = await _typesRepo.GetAllAsync();
            var Data = _mapper.Map<IReadOnlyList<ProductType>, IReadOnlyList<ProductBrandOrTypeToReturnDto>>(types);

            return Ok(Data);
        }

    }
}
