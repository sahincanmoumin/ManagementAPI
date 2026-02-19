using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Abstract;
using System.Security.Claims;

namespace ApiLayer.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpPost("{id}/sell")]
        public IActionResult SellProduct(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                _productService.SellProduct(userId, id);
                _logger.LogInformation($"User {userId} sold product {id}");
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
                var products = _productService.GetAnimalProducts(animalId);
                return Ok(products);
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