using System.Security.Cryptography;
using ErrorReporter.Data;
using ErrorReporter.Entities;
using ErrorReporter.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErrorReporter.Controllers;

[ApiController]
[Route("admin/clients")]
public class ApiClientsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ApiClientsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateApiClientRequest request)
    {
        if (!IsMaster())
            return Forbid();

        // Generate a random API key
        var rawKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var keyHash = ApiKeyMiddleware.HashKey(rawKey);

        var client = new ApiClient
        {
            Name = request.Name,
            KeyHash = keyHash,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.ApiClients.Add(client);
        await _db.SaveChangesAsync();

        // Return the raw key ONCE — it can't be retrieved later
        return Ok(new
        {
            client.Id,
            client.Name,
            ApiKey = rawKey,
            Message = "Store this key securely. It cannot be retrieved again."
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!IsMaster())
            return Forbid();

        var clients = await _db.ApiClients
            .Select(c => new { c.Id, c.Name, c.IsActive, c.CreatedAt })
            .OrderBy(c => c.Name)
            .ToListAsync();

        return Ok(clients);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Revoke(int id)
    {
        if (!IsMaster())
            return Forbid();

        var client = await _db.ApiClients.FindAsync(id);
        if (client is null)
            return NotFound();

        client.IsActive = false;
        await _db.SaveChangesAsync();

        return Ok(new { Message = $"Client '{client.Name}' has been revoked." });
    }

    private bool IsMaster()
    {
        return HttpContext.Items.TryGetValue("IsMaster", out var val) && val is true;
    }
}

public record CreateApiClientRequest(string Name);
