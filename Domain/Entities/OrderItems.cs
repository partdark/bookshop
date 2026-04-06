namespace Domain.Entities
{
    public class OrderItems
    {
        public int Id { get; set; }

        public Guid BookId { get; set; }
        public Book Book { get; set; }
        public int Count { get; set; }

        public Order Order { get; set; }
    }
}
