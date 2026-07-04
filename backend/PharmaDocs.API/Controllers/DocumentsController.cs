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

    /// <summary>Get all active (non-archived) documents. Requires authentication.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DocumentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var docs = await _context.DocumentRecords
            .Include(d => d.CreatedBy)
            .Where(d => !d.IsArchived)
            .OrderBy(d => d.Title)
            .ToListAsync();
        return Ok(docs.Select(ToDto));
    }

    /// <summary>Get active documents for a specific product. Requires authentication.</summary>
    [HttpGet("by-product/{productId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<DocumentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByProduct(Guid productId)
    {
        var docs = await _context.DocumentRecords
            .Include(d => d.CreatedBy)
            .Where(d => d.ProductId == productId && !d.IsArchived)
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
        var doc = await _context.DocumentRecords
            .Include(d => d.CreatedBy)
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsArchived);
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
        var actorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var actorName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

        var doc = new DocumentRecord
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Type = dto.Type,
            Status = dto.Status,
            Version = dto.Version,
            Date = dto.Date,
            Notes = dto.Notes,
            ProductId = dto.ProductId,
            CreatedById = actorId,
            CreatedAt = DateTime.UtcNow
        };

        _context.DocumentRecords.Add(doc);

        _context.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = "DocumentRecord",
            EntityId = doc.Id,
            Action = "Created",
            NewValues = $"{{\"Title\":\"{doc.Title}\",\"Type\":\"{doc.Type}\",\"Status\":\"{doc.Status}\"}}",
            ChangedById = actorId,
            ChangedByName = actorName,
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        await _context.Entry(doc).Reference(d => d.CreatedBy).LoadAsync();

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
        var doc = await _context.DocumentRecords
            .Include(d => d.CreatedBy)
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsArchived);
        if (doc is null) return NotFound();

        var actorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var actorName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

        var oldSnapshot = $"{{\"Title\":\"{doc.Title}\",\"Status\":\"{doc.Status}\",\"Version\":\"{doc.Version}\"}}";

        doc.Title = dto.Title;
        doc.Type = dto.Type;
        doc.Status = dto.Status;
        doc.Version = dto.Version;
        doc.Date = dto.Date;
        doc.Notes = dto.Notes;

        _context.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = "DocumentRecord",
            EntityId = doc.Id,
            Action = "Updated",
            OldValues = oldSnapshot,
            NewValues = $"{{\"Title\":\"{doc.Title}\",\"Status\":\"{doc.Status}\",\"Version\":\"{doc.Version}\"}}",
            ChangedById = actorId,
            ChangedByName = actorName,
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return Ok(ToDto(doc));
    }

    /// <summary>Archive a document (soft delete). Requires RegAffairsOfficer role.</summary>
    [HttpPatch("{id:guid}/archive")]
    [Authorize(Roles = "RegAffairsOfficer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Archive(Guid id)
    {
        var doc = await _context.DocumentRecords.FindAsync(id);
        if (doc is null || doc.IsArchived) return NotFound();

        var actorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var actorName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

        doc.IsArchived = true;
        doc.ArchivedAt = DateTime.UtcNow;
        doc.ArchivedById = actorId;

        _context.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = "DocumentRecord",
            EntityId = doc.Id,
            Action = "Archived",
            OldValues = $"{{\"Title\":\"{doc.Title}\",\"Status\":\"{doc.Status}\"}}",
            ChangedById = actorId,
            ChangedByName = actorName,
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Unarchive a document (restore soft-deleted item). Requires RegAffairsOfficer role.</summary>
    [HttpPatch("{id:guid}/unarchive")]
    [Authorize(Roles = "RegAffairsOfficer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unarchive(Guid id)
    {
        var doc = await _context.DocumentRecords.FindAsync(id);
        if (doc is null || !doc.IsArchived) return NotFound();

        var actorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var actorName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

        doc.IsArchived = false;
        doc.ArchivedAt = null;
        doc.ArchivedById = null;

        _context.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = "DocumentRecord",
            EntityId = doc.Id,
            Action = "Unarchived",
            NewValues = $"{{\"Title\":\"{doc.Title}\",\"Status\":\"{doc.Status}\"}}",
            ChangedById = actorId,
            ChangedByName = actorName,
            Timestamp = DateTime.UtcNow
        });

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
        CreatedById = d.CreatedById,
        CreatedByName = d.CreatedBy?.FullName ?? string.Empty,
        IsArchived = d.IsArchived
    };
}