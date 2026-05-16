using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaDocs.Domain.Entities;
using PharmaDocs.Infrastructure.Data;
using PharmaDocs.Application.DTOs;

namespace PharmaDocs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly PharmaDocsDbContext _context;

    public ProductsController(PharmaDocsDbContext context)
    {
        _context = context;
    }

    // GET /api/products
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _context.Products
            .OrderBy(p => p.Name)
            .ToListAsync();
        return Ok(products);
    }

    // GET /api/products/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound();
        return Ok(product);
    }

    // POST /api/products
    [HttpPost]
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

    // PUT /api/products/{id}
    [HttpPut("{id:guid}")]
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

    // DELETE /api/products/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}