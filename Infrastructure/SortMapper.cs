
using Domain.Entities;
using System.Linq.Expressions;

namespace Infrastructure
{
    public class SortMapper
    {
      public  static readonly Dictionary<string, Expression<Func<Book, Object>>> Map = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Title"] = b => b.Title,
            ["Description"] = b => b.Description,
            ["Rating"] = b => b.Rating,
            ["Price"] = b=> b.Price,
            ["Count"] = b => b.Count,
            ["Year"] = b=> b.PublicationYear
        };
    }
}
