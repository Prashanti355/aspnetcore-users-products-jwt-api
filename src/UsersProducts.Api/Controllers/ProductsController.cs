using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersProducts.Api.Common.Exceptions;
using UsersProducts.Api.Common.Security;
using UsersProducts.Api.Contracts.Products;
using UsersProducts.Api.Services.Products;

namespace UsersProducts.Api.Controllers;

[ApiController]
[Route("api/products")]
[Produces("application/json")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetActiveProducts(
        CancellationToken cancellationToken)
    {
        var products = await _productService.GetActiveAsync(cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> GetActiveProductById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _productService.GetActiveByIdAsync(id, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException("Producto no encontrado.");
        }

        return Ok(product);
    }

    [HttpGet("admin")]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    [ProducesResponseType(typeof(IReadOnlyList<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetAllProducts(
        CancellationToken cancellationToken)
    {
        var products = await _productService.GetAllAsync(cancellationToken);

        return Ok(products);
    }

    [HttpGet("admin/{id:guid}")]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> GetProductById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException("Producto no encontrado.");
        }

        return Ok(product);
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProductResponse>> CreateProduct(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var createdProduct = await _productService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetProductById),
            new { id = createdProduct.Id },
            createdProduct
        );
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> UpdateProduct(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var updatedProduct = await _productService.UpdateAsync(id, request, cancellationToken);

        if (updatedProduct is null)
        {
            throw new NotFoundException("Producto no encontrado.");
        }

        return Ok(updatedProduct);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateProduct(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deactivated = await _productService.DeactivateAsync(id, cancellationToken);

        if (!deactivated)
        {
            throw new NotFoundException("Producto no encontrado.");
        }

        return NoContent();
    }
}