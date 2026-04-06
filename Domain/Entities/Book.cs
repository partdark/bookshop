using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    

    public class Book
    {
        public Guid Id { get; set; } = Guid.NewGuid();    
        public  string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float Rating { get; set; } = 0;
        public decimal Price { get; set; } = 0;
        public string UrlImage { get; set; } = string.Empty;

        public int Count { get; set; } = 0;

        public int PublicationYear { get; set; } = 0;

        public ICollection<Author> Authors = null!;
        public ICollection<Genre> Genres = null!;
        public ICollection<Review> Reviews = null!;
    }
}
