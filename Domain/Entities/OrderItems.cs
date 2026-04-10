namespace Domain.Entities
{
    public class OrderItems
    {

        public int OrderId { get; set; }

        public Guid BookId { get; set; }
        public required Book Book { get; set; }
        public int Count { get; set; }

        public decimal PriceAtPurchase { get; set; }

        public required Order Order { get; set; }
    }
}
