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
public class DocumentsController : ControllerBase
{
    private readonly PharmaDocsDbContext _context;

    public DocumentsController(PharmaDocsDbContext context)
    {
        _context = context;
    }

    /// <summary>Get all documents. Requires authentication.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DocumentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var docs = await _context.DocumentRecords
            .OrderBy(d => d.Title)
            .ToListAsync();
        return Ok(docs.Select(ToDto));
    }

    /// <summary>Get documents for a specific product. Requires authentication.</summary>
    [HttpGet("by-product/{productId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<DocumentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByProduct(Guid productId)
    {
        var docs = await _context.DocumentRecords
            .Where(d => d.ProductId == productId)
            .OrderBy(d => d.Title)
            .ToListAsync();
        return Ok(docs.Select(ToDto));
    }

    /// <summary>Get a single document by ID. Requires authentication.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DocumentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var doc = await _context.DocumentRecords.FindAsync(id);
        if (doc is null) return NotFound();
        return Ok(ToDto(doc));
    }

    /// <summary>Create a new document. Requires RegAffairsOfficer role.</summary>
    [HttpPost]
    [Authorize(Roles = "RegAffairsOfficer")]
    [ProducesResponseType(typeof(DocumentResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateDocumentDto dto)
    {
        var doc = new DocumentRecord
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Type = dto.Type,
            Version = dto.Version,
            Date = dto.Date,
            Notes = dto.Notes,
            ProductId = dto.ProductId,
            CreatedById = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            CreatedAt = DateTime.UtcNow
        };

        _context.DocumentRecords.Add(doc);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = doc.Id }, ToDto(doc));
    }

    /// <summary>Update an existing document. Requires RegAffairsOfficer role.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "RegAffairsOfficer")]
    [ProducesResponseType(typeof(DocumentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDocumentDto dto)
    {
        var doc = await _context.DocumentRecords.FindAsync(id);
        if (doc is null) return NotFound();

        doc.Title = dto.Title;
        doc.Type = dto.Type;
        doc.Status = dto.Status;
        doc.Version = dto.Version;
        doc.Date = dto.Date;
        doc.Notes = dto.Notes;

        await _context.SaveChangesAsync();
        return Ok(ToDto(doc));
    }

    /// <summary>Delete a document. Requires RegAffairsOfficer role.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "RegAffairsOfficer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var doc = await _context.DocumentRecords.FindAsync(id);
        if (doc is null) return NotFound();

        _context.DocumentRecords.Remove(doc);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static DocumentResponseDto ToDto(DocumentRecord d) => new()
    {
        Id = d.Id,
        Title = d.Title,
        Type = d.Type.ToString(),
        Status = d.Status.ToString(),
        Version = d.Version,
        Date = d.Date,
        Notes = d.Notes,
        CreatedAt = d.CreatedAt,
        ProductId = d.ProductId,
        CreatedById = d.CreatedById
    };
}