using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Dto;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using System.Linq.Dynamic.Core;
using System.Runtime.InteropServices.Marshalling;


namespace Infrastructure.Repositories
{
    public partial class BooksRepository : IBooksRepository
    {
        private readonly BookShopContext _context;
        private readonly HybridCache _cache;


        public BooksRepository(BookShopContext context, HybridCache hybridCache)
        {
            _context = context;
            _cache = hybridCache;
        }




        public async Task<ListWithBooksBaseData> BooksBaseData(int pageCapacity = 20, int pageNumber = 1, string orderBy = "Title",
            bool notAscending = false, string? searchingWords = null, bool countMoreThenZero = true)
        {
            if (pageCapacity < 1) pageCapacity = 20;
            if (pageNumber < 1) pageNumber = 1;

            var q = _context.Books
                .AsNoTracking()
                .Include(b => b.Authors)
                .Include(b => b.Genres)
                .AsQueryable();

            if (countMoreThenZero)
            {
                q = q.Where(c => c.Count > 0);
            }

            if (!string.IsNullOrWhiteSpace(searchingWords))
                q = q.Where(x => EF.Functions.Like(x.Title, $"%{searchingWords}%"));

            if (string.IsNullOrEmpty(orderBy))
                orderBy = "Title";

            var orderType = notAscending ? "desc" : "asc";
            q = q.OrderBy($"{orderBy} {orderType}");

            var totalCount = await q.CountAsync();

            if (totalCount == 0)
                return new ListWithBooksBaseData(0, 1, pageCapacity, false, false);

            var lastPage = (int)Math.Ceiling((double)totalCount / pageCapacity);
            pageNumber = Math.Min(pageNumber, lastPage);

            var data = await q
                .Skip(pageCapacity * (pageNumber - 1))
                .Take(pageCapacity)
                .Select(b => new BookBaseData(
                    b.Id, b.Title, b.Description, b.Rating,
                    b.Price, b.UrlImage, b.Count, b.PublicationYear,
                    b.Authors.Select(a => new BookAuthorData(a.Id, a.Name, a.Year)).ToList(),
                    b.Genres.Select(g => new BookGenreData(g.Id, g.Name)).ToList()))
                .ToListAsync();

            return new ListWithBooksBaseData(totalCount, pageNumber, pageCapacity, pageNumber < lastPage, pageNumber > 1)
            {
                Books = data
            };
        }

        public async Task<List<Guid>> AddAuthorsFromBdToBook(Guid bookId, List<Guid> authors)
        {
            var book = await GetByIdAsync(bookId);

            var authorsToAdd = await _context.Authors.Where(a => authors.Contains(a.Id)).ToListAsync();
            if (authorsToAdd.Count() != authors.Count())
            {
                throw new ArgumentException("Не все авторы зарегистрированы в базе данных");
            }
            foreach (var author in authorsToAdd)
            {
                if (!book.Authors.Any(a => a.Id == author.Id))
                {
                    book.Authors.Add(author);
                }
            }
            await _context.SaveChangesAsync();

            return authors;

        }

        public async Task<List<Guid>> AddGenresFromBdToBook(Guid bookId, List<Guid> genres)
        {
            var book = await GetByIdAsync(bookId);

            var genresToAdd = await _context.Genres.Where(a => genres.Contains(a.Id)).ToListAsync();
            if (genresToAdd.Count() != genres.Count())
            {
                throw new ArgumentException("Не все жанры зарегистрированы в базе данных");
            }
            foreach (var genre in genresToAdd)
            {
                if (!book.Genres.Any(a => a.Id == genre.Id))
                {
                    book.Genres.Add(genre);
                }
            }
            await _context.SaveChangesAsync();

            return genres;

        }


        public async Task<Book?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Books.AsNoTracking()
                  .Include(a => a.Authors)
                  .Include(g => g.Genres)
                  .Include(r => r.Reviews)
                      .ThenInclude(r => r.Customer)
                  .AsSingleQuery()
                  .FirstOrDefaultAsync(b => b.Id == id);

            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {

            var entity = await GetByIdAsync(id);
            if (entity == null)
            {
                return false;
            }
            _context.Books.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Dtos<Book>> TakeBookWithPagging(int pageSize = 20, int pageNumber = 1, string orderBy = "title", bool ascending = true)
        {

            if (pageSize <= 0) pageSize = 20;
            if (pageNumber <= 0) pageNumber = 1;

            var q = _context.Books.AsNoTracking()
                   .Include(a => a.Authors)
                   .Include(g => g.Genres)
                   .Include(r => r.Reviews)
                   .AsQueryable();
            if (!string.IsNullOrEmpty(orderBy))
            {
                var orderString = $"{orderBy} {(ascending ? "asc" : "desc")}";
                q = q.OrderBy(orderString);
            }
            else
            {
                q = q.OrderBy(t => t.Title);
            }
            var total = await q.CountAsync();
            var pages = (int)Math.Ceiling((double)total / pageSize);
            if (pages == 0)
            {
                return new Dtos<Book>(default, default, default, default, default)
                {
                    Items = new List<Book>(),
                    TotalCount = 0,
                    PageCount = 0,
                    CurrentPage = 0,
                    HasNext = false,
                    HasPrevious = false,
                };
            }
            pageNumber = Math.Min(pageNumber, pages);


            q = q.Skip(pageSize * (pageNumber - 1)).Take(pageSize);

            var items = await q.ToListAsync();

            return new Dtos<Book>(default, default, default, default, default)
            {
                Items = items,
                TotalCount = total,
                PageCount = pages,
                CurrentPage = pageNumber,
                HasNext = pageNumber < pages,
                HasPrevious = pageNumber > 1,

            };


        }

        public async Task<List<Guid>> GetIdsAsync()
        {
            return await _context.Books.AsNoTracking().Select(b => b.Id).ToListAsync();
        }

        public async Task<Book> UpdateAsync(Book entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateCountAsync(Guid id, int count)
        {
            var result = await _context.Books
                   .Where(b => b.Id == id)
                   .ExecuteUpdateAsync(s => s.SetProperty(b => b.Count, count));
            if (result == 1)
            {
                var key = $"book:{id}";
                await _cache.RemoveAsync(key);
            }  
        }

        public async Task PatchScalarFieldsAsync(Guid id, string title, string description, float rating, decimal price, string urlImage, int count, int publicationYear)
        {
            await _context.Books
                .Where(b => b.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(b => b.Title, title)
                    .SetProperty(b => b.Description, description)
                    .SetProperty(b => b.Rating, rating)
                    .SetProperty(b => b.Price, price)
                    .SetProperty(b => b.UrlImage, urlImage)
                    .SetProperty(b => b.Count, count)
                    .SetProperty(b => b.PublicationYear, publicationYear));
        }

        public async Task<Book?> AddAsync(Book entity)
        {
            if (entity == null)
            {

                throw new ArgumentNullException($"{nameof(entity)}");
            }
            if (await GetByIdAsync(entity.Id) != null)
            {
                throw new InvalidOperationException($"Book with ID{entity.Id} exists");
            }
            _context.Books.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> AddAsyncWithExistsAuthorAndGenres(Book entity, List<Guid> authors, List<Guid> genres)
        {

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var book = await AddAsync(entity);
                    if (authors.Count > 0)
                    {
                        await AddAuthorsFromBdToBook(book.Id, authors);
                    }
                    if (genres.Count > 0)
                    {
                        await AddGenresFromBdToBook(book.Id, genres);
                    }
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }


        }

    }
}
