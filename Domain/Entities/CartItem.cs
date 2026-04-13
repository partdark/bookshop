using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public Guid BookId { get; set; }
        public Book Book { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
