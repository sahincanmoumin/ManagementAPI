using ApiLayer.Controller;
using BusinessLayer.Abstract;
using EntityLayer.Constants;
using EntityLayer.DTOs.Product;
using EntityLayer.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProductController : BaseController
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IProductService productService, ILogger<ProductController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpPost("{id}/sell")]
    public async Task<IActionResult> SellProduct(int id)
    {
        var product = await _productService.GetProductWithOwnershipAsync(id);

        if (!IsAdmin && CurrentUserId != product.Animal.Farm.UserId)
        {
            throw new BusinessException(ErrorKeys.UnauthorizedAction);
        }

        await _productService.SellProductAsync(CurrentUserId, id);
        _logger.LogInformation($"User {CurrentUserId} sold product {id}");
        return Ok(new { message = "Product sold successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] ProductFilterDto filter, [FromQuery] int? animalId = null)
    {
        var products = await _productService.GetAnimalProductsAsync(CurrentUserId, filter, animalId);
        return Ok(products);
    }

    [HttpGet("unsold")]
    public async Task<IActionResult> GetUnsoldProducts([FromQuery] ProductFilterDto filter)
    {
        var products = await _productService.GetUnsoldProductsAsync(CurrentUserId, filter);
        return Ok(products);
    }
}