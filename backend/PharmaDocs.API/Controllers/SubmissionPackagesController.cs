using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaDocs.Application.DTOs;
using PharmaDocs.Domain.Entities;
using PharmaDocs.Infrastructure.Data;
using System.Security.Claims;

namespace PharmaDocs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class SubmissionPackagesController : ControllerBase
{
    private readonly PharmaDocsDbContext _context;

    public SubmissionPackagesController(PharmaDocsDbContext context)
    {
        _context = context;
    }

    /// <summary>Get all active (non-archived) submission packages. Requires authentication.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SubmissionPackageResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var packages = await _context.SubmissionPackages
            .Include(s => s.Product)
            .Include(s => s.CreatedBy)
            .Where(s => !s.IsArchived)
            .OrderBy(s => s.CreatedAt)
            .ToListAsync();

        return Ok(packages.Select(ToDto));
    }

    /// <summary>Get a single submission package by ID. Requires authentication.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SubmissionPackageResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var package = await _context.SubmissionPackages
            .Include(s => s.Product)
            .Include(s => s.CreatedBy)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsArchived);

        if (package is null) return NotFound();
        return Ok(ToDto(package));
    }

    /// <summary>Get active submission packages for a specific product. Requires authentication.</summary>
    [HttpGet("by-product/{productId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<SubmissionPackageResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByProduct(Guid productId)
    {
        var packages = await _context.SubmissionPackages
            .Include(s => s.Product)
            .Include(s => s.CreatedBy)
            .Where(s => s.ProductId == productId && !s.IsArchived)
            .OrderBy(s => s.CreatedAt)
            .ToListAsync();

        return Ok(packages.Select(ToDto));
    }

    /// <summary>Create a new submission package. Requires RegAffairsOfficer role.</summary>
    [HttpPost]
    [Authorize(Roles = "RegAffairsOfficer")]
    [ProducesResponseType(typeof(SubmissionPackageResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateSubmissionPackageDto dto)
    {
        var actorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var actorName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

        var package = new SubmissionPackage
        {
            Id = Guid.NewGuid(),
            SubmissionType = dto.SubmissionType,
            ProductId = dto.ProductId,
            RegulatoryBody = dto.RegulatoryBody,
            TargetDate = dto.TargetDate,
            CreatedById = actorId,
            CreatedAt = DateTime.UtcNow
        };

        _context.SubmissionPackages.Add(package);

        _context.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = "SubmissionPackage",
            EntityId = package.Id,
            Action = "Created",
            NewValues = $"{{\"SubmissionType\":\"{package.SubmissionType}\",\"RegulatoryBody\":\"{package.RegulatoryBody}\"}}",
            ChangedById = actorId,
            ChangedByName = actorName,
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        await _context.Entry(package).Reference(s => s.Product).LoadAsync();
        await _context.Entry(package).Reference(s => s.CreatedBy).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = package.Id }, ToDto(package));
    }

    /// <summary>Update an existing submission package. Requires RegAffairsOfficer role.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "RegAffairsOfficer")]
    [ProducesResponseType(typeof(SubmissionPackageResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubmissionPackageDto dto)
    {
        var package = await _context.SubmissionPackages
            .Include(s => s.Product)
            .Include(s => s.CreatedBy)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsArchived);

        if (package is null) return NotFound();

        var actorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var actorName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

        var oldSnapshot = $"{{\"RegulatoryBody\":\"{package.RegulatoryBody}\",\"TargetDate\":\"{package.TargetDate}\",\"SubmissionDate\":\"{package.SubmissionDate}\"}}";

        package.RegulatoryBody = dto.RegulatoryBody;
        package.TargetDate = dto.TargetDate;
        package.SubmissionDate = dto.SubmissionDate;

        _context.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = "SubmissionPackage",
            EntityId = package.Id,
            Action = "Updated",
            OldValues = oldSnapshot,
            NewValues = $"{{\"RegulatoryBody\":\"{package.RegulatoryBody}\",\"TargetDate\":\"{package.TargetDate}\",\"SubmissionDate\":\"{package.SubmissionDate}\"}}",
            ChangedById = actorId,
            ChangedByName = actorName,
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return Ok(ToDto(package));
    }

    /// <summary>Update the status of a submission package and record an audit log entry. Requires RegAffairsOfficer role.</summary>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "RegAffairsOfficer")]
    [ProducesResponseType(typeof(SubmissionPackageResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateSubmissionStatusDto dto)
    {
        var package = await _context.SubmissionPackages
            .Include(s => s.Product)
            .Include(s => s.CreatedBy)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsArchived);

        if (package is null) return NotFound();

        var actorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var actorName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

        _context.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = "SubmissionPackage",
            EntityId = package.Id,
            SubmissionPackageId = package.Id,      // kept for backwards compat
            Action = "StatusChanged",
            OldStatus = package.Status.ToString(),
            NewStatus = dto.NewStatus.ToString(),
            ChangedById = actorId,
            ChangedByName = actorName,
            Notes = dto.Notes,
            Timestamp = DateTime.UtcNow
        });

        package.Status = dto.NewStatus;

        await _context.SaveChangesAsync();
        return Ok(ToDto(package));
    }

    /// <summary>Archive a submission package (soft delete). Blocked if status is Submitted or UnderReview. Requires RegAffairsOfficer role.</summary>
    [HttpPatch("{id:guid}/archive")]
    [Authorize(Roles = "RegAffairsOfficer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Archive(Guid id)
    {
        var package = await _context.SubmissionPackages.FindAsync(id);
        if (package is null || package.IsArchived) return NotFound();

        // Domain rule: a live submission cannot be quietly archived
        if (package.Status == PharmaDocs.Domain.Enums.SubmissionStatus.Submitted ||
            package.Status == PharmaDocs.Domain.Enums.SubmissionStatus.UnderReview)
        {
            return BadRequest(new { error = "Cannot archive a submission that is Submitted or Under Review. Withdraw it first." });
        }

        var actorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var actorName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

        package.IsArchived = true;
        package.ArchivedAt = DateTime.UtcNow;
        package.ArchivedById = actorId;

        _context.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = "SubmissionPackage",
            EntityId = package.Id,
            Action = "Archived",
            OldValues = $"{{\"Status\":\"{package.Status}\"}}",
            ChangedById = actorId,
            ChangedByName = actorName,
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Unarchive a submission package (restore soft-deleted item). Requires RegAffairsOfficer role.</summary>
    [HttpPatch("{id:guid}/unarchive")]
    [Authorize(Roles = "RegAffairsOfficer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unarchive(Guid id)
    {
        var package = await _context.SubmissionPackages.FindAsync(id);
        if (package is null || !package.IsArchived) return NotFound();

        var actorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var actorName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

        package.IsArchived = false;
        package.ArchivedAt = null;
        package.ArchivedById = null;

        _context.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = "SubmissionPackage",
            EntityId = package.Id,
            Action = "Unarchived",
            NewValues = $"{{\"Status\":\"{package.Status}\"}}",
            ChangedById = actorId,
            ChangedByName = actorName,
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static SubmissionPackageResponseDto ToDto(SubmissionPackage s) => new()
    {
        Id = s.Id,
        SubmissionType = s.SubmissionType,
        RegulatoryBody = s.RegulatoryBody,
        Status = s.Status,
        TargetDate = s.TargetDate,
        SubmissionDate = s.SubmissionDate,
        CreatedAt = s.CreatedAt,
        ProductId = s.ProductId,
        ProductName = s.Product?.Name ?? string.Empty,
        CreatedById = s.CreatedById,
        CreatedByName = s.CreatedBy?.FullName ?? string.Empty
    };
}