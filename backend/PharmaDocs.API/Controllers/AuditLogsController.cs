using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaDocs.Application.DTOs;
using PharmaDocs.Infrastructure.Data;

namespace PharmaDocs.API.Controllers;

[ApiController]
[Route("api/auditlogs")]
[Authorize]
[Produces("application/json")]
public class AuditLogsController : ControllerBase
{
    private readonly PharmaDocsDbContext _context;

    public AuditLogsController(PharmaDocsDbContext context)
    {
        _context = context;
    }

    /// <summary>Returns audit log entries filtered by entity type and optionally by entity ID.</summary>
    /// <param name="entityType">Required. One of: Product, DocumentRecord, SubmissionPackage</param>
    /// <param name="entityId">Optional. GUID of the specific entity.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AuditLogResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] string entityType,
        [FromQuery] Guid? entityId = null)
    {
        var validTypes = new[] { "Product", "DocumentRecord", "SubmissionPackage" };

        if (string.IsNullOrWhiteSpace(entityType) || !validTypes.Contains(entityType))
            return BadRequest(new { error = "entityType must be one of: Product, DocumentRecord, SubmissionPackage" });

        var query = _context.AuditLogs
            .Where(a => a.EntityType == entityType);

        if (entityId.HasValue)
            query = query.Where(a => a.EntityId == entityId.Value);

        var logs = await query
            .OrderByDescending(a => a.Timestamp)
            .Select(a => new AuditLogResponseDto
            {
                Id = a.Id,
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                Action = a.Action,
                OldValues = a.OldValues,
                NewValues = a.NewValues,
                OldStatus = a.OldStatus,
                NewStatus = a.NewStatus,
                ChangedByName = a.ChangedByName,
                Notes = a.Notes,
                Timestamp = a.Timestamp
            })
            .ToListAsync();

        return Ok(logs);
    }
}