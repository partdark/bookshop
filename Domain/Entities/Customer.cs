using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; set;  } = Guid.NewGuid();

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public string Mail { get; set; } = string.Empty;

        public string Phone {  get; set; }   = string.Empty;

        public DateOnly DateOfBirth { get; set; } = new DateOnly(1900,1,1);


        public ICollection<Order> Orders = null!;

        public ICollection<Review> Reviews= null!;
    }
}
