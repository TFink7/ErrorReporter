namespace ErrorReporter.Entities
{
    public class ErrorReport
    {
        public int Id { get; set; }
        public string Service { get; set; }
        public string Message { get; set; }
        public string? StackTrace { get; set; }
        public DateTime OccurredAt { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
