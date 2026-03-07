using ErrorReporter.Controllers;
using ErrorReporter.Data;
using ErrorReporter.Dtos;
using ErrorReporter.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErrorReporterTests;

public class ErrorsControllerTests
{
    private AppDbContext CreateDb(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new AppDbContext(options);
    }

    // GetById
    [Fact]
    public async Task GetById_ReturnsNotFound_WhenErrorDoesNotExist()
    {
        using var db = CreateDb(nameof(GetById_ReturnsNotFound_WhenErrorDoesNotExist));
        var controller = new ErrorsController(db);

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WithCorrectError()
    {
        using var db = CreateDb(nameof(GetById_ReturnsOk_WithCorrectError));
        var entity = new ErrorReport
        {
            Service = "AuthService",
            Message = "Token expired",
            Severity = Severity.Error,
            OccurredAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        db.ErrorReports.Add(entity);
        await db.SaveChangesAsync();

        var controller = new ErrorsController(db);
        var result = await controller.GetById(entity.Id);

        var ok = Assert.IsType<OkObjectResult>(result);
        var error = Assert.IsType<ErrorReport>(ok.Value);
        Assert.Equal("AuthService", error.Service);
    }

    // Create
    [Fact]
    public async Task Create_CreatesAndReturnsErrorReport()
    {
        using var db = CreateDb(nameof(Create_CreatesAndReturnsErrorReport));
        var controller = new ErrorsController(db);
        var dto = new CreateErrorReportDto("PaymentService", "Null reference exception", null, Severity.Error, DateTime.UtcNow);

        var result = await controller.Create(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(1, await db.ErrorReports.CountAsync());
        var error = Assert.IsType<ErrorReport>(created.Value);
        Assert.Equal("PaymentService", error.Service);
    }

    // Delete
    [Fact]
    public async Task Delete_ReturnsNotFound_WhenErrorDoesNotExist()
    {
        using var db = CreateDb(nameof(Delete_ReturnsNotFound_WhenErrorDoesNotExist));
        var controller = new ErrorsController(db);

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_RemovesError()
    {
        using var db = CreateDb(nameof(Delete_RemovesError));
        var entity = new ErrorReport
        {
            Service = "AuthService",
            Message = "Token expired",
            Severity = Severity.Error,
            OccurredAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        db.ErrorReports.Add(entity);
        await db.SaveChangesAsync();

        var controller = new ErrorsController(db);
        var result = await controller.Delete(entity.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(0, await db.ErrorReports.CountAsync());
    }

    // GetAll
    [Fact]
    public async Task GetAll_ReturnsAllErrors_WhenNoFiltersApplied()
    {
        using var db = CreateDb(nameof(GetAll_ReturnsAllErrors_WhenNoFiltersApplied));
        db.ErrorReports.AddRange(
            new ErrorReport { Service = "A", Message = "err", Severity = Severity.Info, OccurredAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new ErrorReport { Service = "B", Message = "err", Severity = Severity.Info, OccurredAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();

        var controller = new ErrorsController(db);
        var result = await controller.GetAll(null, null, null);

        var ok = Assert.IsType<OkObjectResult>(result);
        var errors = Assert.IsType<List<ErrorReport>>(ok.Value);
        Assert.Equal(2, errors.Count);
    }

    [Fact]
    public async Task GetAll_FiltersByService()
    {
        using var db = CreateDb(nameof(GetAll_FiltersByService));
        db.ErrorReports.AddRange(
            new ErrorReport { Service = "AuthService", Message = "err", Severity = Severity.Error, OccurredAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new ErrorReport { Service = "PaymentService", Message = "err", Severity = Severity.Warning, OccurredAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();

        var controller = new ErrorsController(db);
        var result = await controller.GetAll("AuthService", null, null);

        var ok = Assert.IsType<OkObjectResult>(result);
        var errors = Assert.IsType<List<ErrorReport>>(ok.Value);
        Assert.All(errors, e => Assert.Equal("AuthService", e.Service));
    }

    [Fact]
    public async Task GetAll_FiltersByDateRange()
    {
        using var db = CreateDb(nameof(GetAll_FiltersByDateRange));
        var inRange = DateTime.UtcNow;
        db.ErrorReports.AddRange(
            new ErrorReport { Service = "A", Message = "err", Severity = Severity.Info, OccurredAt = inRange, CreatedAt = DateTime.UtcNow },
            new ErrorReport { Service = "B", Message = "err", Severity = Severity.Info, OccurredAt = inRange.AddDays(-10), CreatedAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();

        var controller = new ErrorsController(db);
        var result = await controller.GetAll(null, inRange.AddDays(-1), inRange.AddDays(1));

        var ok = Assert.IsType<OkObjectResult>(result);
        var errors = Assert.IsType<List<ErrorReport>>(ok.Value);
        Assert.Single(errors);
        Assert.Equal("A", errors[0].Service);
    }

    [Fact]
    public async Task GetAll_FiltersByServiceAndDateRange()
    {
        using var db = CreateDb(nameof(GetAll_FiltersByServiceAndDateRange));
        var inRange = DateTime.UtcNow;
        db.ErrorReports.AddRange(
            new ErrorReport { Service = "AuthService", Message = "err", Severity = Severity.Error, OccurredAt = inRange, CreatedAt = DateTime.UtcNow },
            new ErrorReport { Service = "AuthService", Message = "err", Severity = Severity.Error, OccurredAt = inRange.AddDays(-10), CreatedAt = DateTime.UtcNow },
            new ErrorReport { Service = "PaymentService", Message = "err", Severity = Severity.Warning, OccurredAt = inRange, CreatedAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();

        var controller = new ErrorsController(db);
        var result = await controller.GetAll("AuthService", inRange.AddDays(-1), inRange.AddDays(1));

        var ok = Assert.IsType<OkObjectResult>(result);
        var errors = Assert.IsType<List<ErrorReport>>(ok.Value);
        Assert.Single(errors);
        Assert.Equal("AuthService", errors[0].Service);
    }

    [Fact]
    public async Task GetAll_ReturnsAll_WhenServiceIsEmpty()
    {
        using var db = CreateDb(nameof(GetAll_ReturnsAll_WhenServiceIsEmpty));
        db.ErrorReports.AddRange(
            new ErrorReport { Service = "A", Message = "err", Severity = Severity.Info, OccurredAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new ErrorReport { Service = "B", Message = "err", Severity = Severity.Info, OccurredAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();

        var controller = new ErrorsController(db);
        var result = await controller.GetAll("   ", null, null);

        var ok = Assert.IsType<OkObjectResult>(result);
        var errors = Assert.IsType<List<ErrorReport>>(ok.Value);
        Assert.Equal(2, errors.Count);
    }

    // GetSummary
    [Fact]
    public async Task GetSummary_GroupsAndCountsByService()
    {
        using var db = CreateDb(nameof(GetSummary_GroupsAndCountsByService));
        db.ErrorReports.AddRange(
            new ErrorReport { Service = "AuthService", Message = "err", Severity = Severity.Critical, OccurredAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new ErrorReport { Service = "AuthService", Message = "err", Severity = Severity.Error, OccurredAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new ErrorReport { Service = "PaymentService", Message = "err", Severity = Severity.Warning, OccurredAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();

        var controller = new ErrorsController(db);
        var result = await controller.GetSummary();

        var ok = Assert.IsType<OkObjectResult>(result);
        var summary = Assert.IsType<List<ErrorSummaryDto>>(ok.Value);
        Assert.Equal(2, summary.Count);
        Assert.Equal("AuthService", summary[0].Service);
        Assert.Equal(2, summary[0].Count);
        Assert.Equal("PaymentService", summary[1].Service);
        Assert.Equal(1, summary[1].Count);
    }
}
