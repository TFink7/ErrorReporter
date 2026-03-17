using System.ComponentModel.DataAnnotations;
using ErrorReporter.Entities;

namespace ErrorReporter.Dtos;

public record CreateErrorReportDto(
    [Required, MaxLength(200)] string Service,
    [Required, MaxLength(1000)] string Message,
    [MaxLength(4000)] string? StackTrace,
    Severity Severity,
    DateTime OccurredAt
);
