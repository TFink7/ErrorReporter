namespace ErrorReporter.Dtos;

public record ErrorSummaryDto(
    string Service,
    int Count,
    DateTime? MostRecentOccurrence
);
