using ErrorReporter.Data;
using ErrorReporter.Dtos;
using ErrorReporter.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace ErrorReporter.Controllers;

[ApiController]
[Route("errors")]
[EnableRateLimiting("PerApiKey")]
public class ErrorsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ErrorsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var error = await _db.ErrorReports.FindAsync(id);

        if (error is null)
        {
            return NotFound();
        }

        return Ok(error);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateErrorReportDto dto)
    {
        var clientId = HttpContext.Items.TryGetValue("ApiClientId", out var id) ? (int?)id : null;

        var error = new ErrorReport
        {
            Service = dto.Service,
            Message = dto.Message,
            StackTrace = dto.StackTrace,
            Severity = dto.Severity,
            OccurredAt = dto.OccurredAt,
            CreatedAt = DateTime.UtcNow,
            ApiClientId = clientId
        };

        _db.ErrorReports.Add(error);
        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = error.Id },
            error
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var error = await _db.ErrorReports.FindAsync(id);

        if (error is null)
        {
            return NotFound();
        }

        _db.ErrorReports.Remove(error);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? service,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 1;
        if (pageSize > 100) pageSize = 100;

        var query = _db.ErrorReports.AsQueryable();

        if (!string.IsNullOrWhiteSpace(service))
            query = query.Where(e => e.Service == service);

        if (from.HasValue)
            query = query.Where(e => e.OccurredAt >= from.Value);

        if (to.HasValue)
            query = query.Where(e => e.OccurredAt <= to.Value);

        var totalCount = await query.CountAsync();

        var errors = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            data = errors,
            page,
            pageSize,
            totalCount,
            totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        });
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var grouped = await _db.ErrorReports
            .GroupBy(e => e.Service)
            .Select(g => new
            {
                Service = g.Key,
                Count = g.Count(),
                MostRecentOccurrence = g.Max(e => (DateTime?)e.OccurredAt)
            })
            .OrderByDescending(s => s.Count)
            .ToListAsync();

        var summary = grouped
            .Select(g => new ErrorSummaryDto(g.Service, g.Count, g.MostRecentOccurrence))
            .ToList();

        return Ok(summary);
    }
}
