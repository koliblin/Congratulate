namespace Congratulate.Domain.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public string? PhotoPath { get; set; }
    }
} 