using ErrorReporter.Entities;

namespace ErrorReporter.Dtos;

public record CreateErrorReportDto(
    string Service,
    string Message,
    string? StackTrace,
    Severity Severity,
    DateTime OccurredAt
);
