using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Customer : IdentityUser<Guid>
    {
     //   public Guid Id { get; set;  } = Guid.NewGuid();

       
      //  public string Name { get; set; } = string.Empty;

        
     //   public string PasswordHash { get; set; } = string.Empty;
        
     //   public string Mail { get; set; } = string.Empty;

      //  public string Phone {  get; set; }   = string.Empty;

        public DateOnly DateOfBirth { get; set; } = new DateOnly(1900,1,1);


        public ICollection<Order> Orders = null!;

        public ICollection<Review> Reviews= null!;

        public ICollection<CartItem> CartItems { get; set; } = null!;
    }
}
