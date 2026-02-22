using System.ComponentModel.DataAnnotations;

namespace ErrorReporter.Entities
{
    public class ErrorReport
    {
        public int Id { get; set; }

        [Required]
        public required string Service { get; set; }

        [Required]
        public required string Message { get; set; }

        public string? StackTrace { get; set; }

        public DateTime OccurredAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
