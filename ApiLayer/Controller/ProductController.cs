using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Abstract;
using System.Security.Claims;
using EntityLayer.DTOs.Product;

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
            try
            {

                var product = _productService.GetProductWithOwnership(id);

                if(product == null)
                {
                    _logger.LogWarning($"Product {id} not found");
                    return NotFound(new { message = "Product not found" });
                }

                if (!IsAdmin && CurrentUserId != product.Animal.Farm.UserId) {//**********************************

                    return Forbid();

                }


                _productService.SellProduct(CurrentUserId, id);
                _logger.LogInformation($"User {CurrentUserId} sold product {id}");
                return Ok(new { message = "Product sold successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sell product failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("animal/{animalId}")]
        public IActionResult GetAnimalProducts(int animalId)
        {
            try
            {
                
                var animal = _animalService.GetById(animalId);
                if (animal == null) return NotFound("Hayvan bulunamadı.");
                                                                             
                var farm = _farmService.GetById(animal.FarmId);
                if (farm == null) return NotFound("Çiftlik bulunamadı.");

                
                if (!IsAdmin && farm.UserId != CurrentUserId)
                {
                    return Forbid();
                }

               
                var products = _productService.GetAnimalProducts(animalId);
                return Ok(products);
                ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get animal products failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("unsold")]
        public IActionResult GetUnsoldProducts()
        {
            try
            {
                var products = _productService.GetUnsoldProducts();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get unsold products failed");
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}