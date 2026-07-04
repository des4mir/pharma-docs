using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PharmaDocs.Domain.Entities;
using PharmaDocs.Infrastructure.Data;
using PharmaDocs.Application.DTOs;
using System.Security.Claims;

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

    /// <summary>Get all active (non-archived) products. Requires authentication.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var products = await _context.Products
            .Include(p => p.CreatedBy)
            .Where(p => !p.IsArchived)
            .OrderBy(p => p.Name)
            .ToListAsync();
        return Ok(products.Select(ToDto));
    }

    /// <summary>Get a single product by ID. Requires authentication.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _context.Products
            .Include(p => p.CreatedBy)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsArchived);
        if (product is null) return NotFound();
        return Ok(ToDto(product));
    }

    /// <summary>Create a new product. Requires RegAffairsOfficer role.</summary>
    [HttpPost]
    [Authorize(Roles = "RegAffairsOfficer")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var actorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var actorName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

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
            CreatedById = actorId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);

        _context.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = "Product",
            EntityId = product.Id,
            Action = "Created",
            NewValues = $"{{\"Name\":\"{product.Name}\"}}",
            ChangedById = actorId,
            ChangedByName = actorName,
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        await _context.Entry(product).Reference(p => p.CreatedBy).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, ToDto(product));
    }

    /// <summary>Update an existing product. Requires RegAffairsOfficer role.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "RegAffairsOfficer")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
    {
        var product = await _context.Products
            .Include(p => p.CreatedBy)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsArchived);
        if (product is null) return NotFound();

        var actorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var actorName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

        var oldSnapshot = $"{{\"Name\":\"{product.Name}\",\"Manufacturer\":\"{product.Manufacturer}\"}}";

        product.Name = dto.Name;
        product.DIN = dto.DIN;
        product.NPN = dto.NPN;
        product.MedicinalIngredient = dto.MedicinalIngredient;
        product.Manufacturer = dto.Manufacturer;
        product.DosageForm = dto.DosageForm;
        product.RouteOfAdministration = dto.RouteOfAdministration;
        product.TherapeuticCategory = dto.TherapeuticCategory;

        _context.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = "Product",
            EntityId = product.Id,
            Action = "Updated",
            OldValues = oldSnapshot,
            NewValues = $"{{\"Name\":\"{product.Name}\",\"Manufacturer\":\"{product.Manufacturer}\"}}",
            ChangedById = actorId,
            ChangedByName = actorName,
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return Ok(ToDto(product));
    }

    /// <summary>Archive a product (soft delete). Requires RegAffairsOfficer role.</summary>
    [HttpPatch("{id:guid}/archive")]
    [Authorize(Roles = "RegAffairsOfficer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Archive(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null || product.IsArchived) return NotFound();

        var actorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var actorName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

        product.IsArchived = true;
        product.ArchivedAt = DateTime.UtcNow;
        product.ArchivedById = actorId;

        _context.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = "Product",
            EntityId = product.Id,
            Action = "Archived",
            OldValues = $"{{\"Name\":\"{product.Name}\"}}",
            ChangedById = actorId,
            ChangedByName = actorName,
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Unarchive a product (restore soft-deleted item). Requires RegAffairsOfficer role.</summary>
    [HttpPatch("{id:guid}/unarchive")]
    [Authorize(Roles = "RegAffairsOfficer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unarchive(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null || !product.IsArchived) return NotFound();

        var actorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var actorName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

        product.IsArchived = false;
        product.ArchivedAt = null;
        product.ArchivedById = null;

        _context.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = "Product",
            EntityId = product.Id,
            Action = "Unarchived",
            NewValues = $"{{\"Name\":\"{product.Name}\"}}",
            ChangedById = actorId,
            ChangedByName = actorName,
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static ProductResponseDto ToDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        DIN = p.DIN,
        NPN = p.NPN,
        MedicinalIngredient = p.MedicinalIngredient,
        Manufacturer = p.Manufacturer,
        DosageForm = p.DosageForm,
        RouteOfAdministration = p.RouteOfAdministration,
        TherapeuticCategory = p.TherapeuticCategory,
        CreatedAt = p.CreatedAt,
        CreatedById = p.CreatedById,
        CreatedByName = p.CreatedBy?.FullName ?? string.Empty,
        IsArchived = p.IsArchived
    };
}