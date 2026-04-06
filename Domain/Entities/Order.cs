namespace Domain.Entities
{

    public enum OrderStatus
    {
        Cart,
        Placed,
        Shipped,
        Delivered,
        Cancelled

    }
    public class Order
    {
        public int Id { get; set; }

        public Guid CustomerId { get; set; }

        public Customer Customer { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; }  = OrderStatus.Cart;

        public ICollection<OrderItems> Items = null!;

    }
}
