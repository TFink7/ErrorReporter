namespace ErrorReporter.Dtos;

public record CreateErrorReportDto(
    string Service,
    string Message,
    string? StackTrace,
    DateTime OccurredAt
);
