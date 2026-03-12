using System.ComponentModel.DataAnnotations;

namespace ErrorReporter.Entities
{
    public class ApiClient
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required]
        public required string KeyHash { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
    }
}
