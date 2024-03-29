using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Errors;
using API.Helpers;
using AutoMapper;
using core.Interfaces;
using core.Specificatoins;
using Core.Entities;
using Inrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    
    public class ProductsController : BaseApiController
    {
        private readonly IProductRepository _repo;
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<ProductType> _productTypeRepo;
        private readonly IGenericRepository<ProductBrand> _productBrandRepo;


        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> productRepo, IGenericRepository<ProductType> productTypeRepo, IGenericRepository<ProductBrand> productBrandRepo, IMapper mapper)
        {
            this._mapper = mapper;
            this._productBrandRepo = productBrandRepo;
            this._productTypeRepo = productTypeRepo;
            this._productRepo = productRepo;
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturn>>> GetProducts([FromQuery] ProductParamsSpec paramsSpec)
        {
            var spec = new ProductsWithBrandsAndTypesSpecification(paramsSpec);
            var countSpec=new ProductWithFiltersForCountSpecification(paramsSpec);
            var totalItems=await this._productRepo.CountAsync(countSpec); 
            var data =this._mapper.Map<IReadOnlyList<ProductToReturn>>(await this._productRepo.ListAsync(spec));
            return Ok(new Pagination<ProductToReturn>(paramsSpec.PageIndex,paramsSpec.PageSize,totalItems,data));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var spec = new ProductsWithBrandsAndTypesSpecification(id);
            var p = await this._productRepo.GetEntityWithSpec(spec);
            return Ok(this._mapper.Map<ProductToReturn>(p));
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await this._productBrandRepo.ListAllAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            return Ok(await this._productTypeRepo.ListAllAsync());
        }
    }
}