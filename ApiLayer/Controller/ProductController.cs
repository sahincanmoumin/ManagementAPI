using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Abstract;
using System.Security.Claims;
using EntityLayer.DTOs.Product;
using EntityLayer.Exceptions;
using EntityLayer.Constants;

namespace ApiLayer.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IFarmService _farmService;
        private readonly IAnimalService _animalService;
        private readonly ILogger<ProductController> _logger;
        
        public ProductController(IProductService productService,IFarmService farmService,IAnimalService animaService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _farmService = farmService;
            _animalService = animaService;
            _logger = logger;
        }
        
        [HttpPost("{id}/sell")]
        public IActionResult SellProduct(int id)
        {

            var product = _productService.GetProductWithOwnership(id);

            if (!IsAdmin && CurrentUserId != product.Animal.Farm.UserId) {//**********************************

                throw new BusinessException(ErrorKeys.UnauthorizedAction);
            }

            _productService.SellProduct(CurrentUserId, id);
            _logger.LogInformation($"User {CurrentUserId} sold product {id}");
            return Ok(new { message = "Product sold successfully" });
            
        }

        [HttpGet]
        public IActionResult GetProducts([FromQuery] ProductFilterDto filter, [FromQuery] int? animalId = null)
        {
            var products = _productService.GetAnimalProducts(CurrentUserId, filter, animalId);
            return Ok(products);
        }

        [HttpGet("unsold")]
        public IActionResult GetUnsoldProducts([FromQuery] ProductFilterDto filter)
        {
            var products = _productService.GetUnsoldProducts(CurrentUserId, filter);
            return Ok(products);
        }
    }
}