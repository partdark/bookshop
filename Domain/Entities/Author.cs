namespace Domain.Entities
{
    public class Author
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name {  get; set; } = string.Empty;

        public int Year { get; set; } = 0;

        public ICollection<Book> Books = new List<Book>()!;
    }
}
