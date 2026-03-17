using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ErrorReporter.Entities
{
    public class ApiClient
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required]
        [JsonIgnore]
        public required string KeyHash { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
    }
}
