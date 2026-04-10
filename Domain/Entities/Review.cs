namespace Domain.Entities
{
    public class Review
    {
        public Guid Id { get; set;  } = Guid.NewGuid();

        public DateTime Date { get; set; } = DateTime.UtcNow;
        public int Rating { get; set; } = 0;
        public string ReviewText { get; set; } = string.Empty;

        public Guid BookId { get; set; }
        public required Book Book { get; set; }
        public Guid CustomerId { get; set; }

        public required Customer Customer { get; set; }
    }
}
