using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PharmaDocs.Domain.Entities;
using PharmaDocs.Infrastructure.Data;
using PharmaDocs.Application.DTOs;

namespace PharmaDocs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly PharmaDocsDbContext _context;

    public ProductsController(PharmaDocsDbContext context)
    {
        _context = context;
    }
    /// <summary>Get all products. Requires authentication.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var products = await _context.Products
            .OrderBy(p => p.Name)
            .ToListAsync();
        return Ok(products);
    }

    /// <summary>Get a single product by ID. Requires authentication.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound();
        return Ok(product);
    }

    /// <summary>Create a new product. Requires RegAffairsOfficer role.</summary>
    [HttpPost]
    [Authorize(Roles = "RegAffairsOfficer")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            DIN = dto.DIN,
            NPN = dto.NPN,
            MedicinalIngredient = dto.MedicinalIngredient,
            Manufacturer = dto.Manufacturer,
            DosageForm = dto.DosageForm,
            RouteOfAdministration = dto.RouteOfAdministration,
            TherapeuticCategory = dto.TherapeuticCategory,
            CreatedById = dto.CreatedById,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>Update an existing product. Requires RegAffairsOfficer role.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "RegAffairsOfficer")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound();

        product.Name = dto.Name;
        product.DIN = dto.DIN;
        product.NPN = dto.NPN;
        product.MedicinalIngredient = dto.MedicinalIngredient;
        product.Manufacturer = dto.Manufacturer;
        product.DosageForm = dto.DosageForm;
        product.RouteOfAdministration = dto.RouteOfAdministration;
        product.TherapeuticCategory = dto.TherapeuticCategory;

        await _context.SaveChangesAsync();
        return Ok(product);
    }

    /// <summary>Delete a product. Requires RegAffairsOfficer role.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "RegAffairsOfficer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}