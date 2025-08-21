namespace CarDealer.Models
{
    public class Inquiry
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public Car Car { get; set; }
    }
}
