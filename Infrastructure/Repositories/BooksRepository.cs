using Dapper;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Dto;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Npgsql;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
//using System.Linq.Dynamic.Core;

namespace Infrastructure.Repositories
{
    public partial class BooksRepository : IBooksRepository
    {
        private readonly NpgsqlConnection _connection;
        private readonly BookShopContext _context;
        private readonly HybridCache _cache;


        public BooksRepository(BookShopContext context, HybridCache hybridCache, IDbConnection? connectionDapper = null)
        {
            _context = context;
            _cache = hybridCache;
            _connection = connectionDapper as NpgsqlConnection ?? 
                throw new ArgumentNullException("Невозможно получить строку подключения");
        }




        public async Task<ListWithBooksBaseData> BooksBaseData(int pageCapacity = 20, int pageNumber = 1, string orderBy = "Title",
            bool notAscending = false, string? searchingWords = null, bool countMoreThenZero = true)
        {
            if (pageCapacity < 1) pageCapacity = 20;
            if (pageNumber < 1) pageNumber = 1;

            var q = _context.Books
                .AsNoTracking()
                .AsSplitQuery()
                .Include(b => b.Authors)
                .Include(b => b.Genres)
                .AsQueryable();

            if (countMoreThenZero)
            {
                q = q.Where(c => c.Count > 0);
            }

            if (!string.IsNullOrWhiteSpace(searchingWords))
                q = q.Where(x => EF.Functions.Like(x.Title, $"%{searchingWords
                    .Replace("%", "[%]")
                    .Replace("_", "[_]")
                    .Replace("[", "[[]")}%")
                );

            /*
            if (string.IsNullOrEmpty(orderBy))
                orderBy = "Title";

            var orderType = notAscending ? "desc" : "asc";
            q = q.OrderBy($"{orderBy} {orderType}");
            */

            if (SortMapper.Map.TryGetValue(orderBy, out var CurentOrdering))
            {
                q = notAscending ?
                     q.OrderByDescending(CurentOrdering) : q.OrderBy(CurentOrdering);
            }
            else
            {
                q = notAscending ?
                    q.OrderByDescending(b => b.Title) : q.OrderBy(b => b.Title);
            }


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

            var result = new ListWithBooksBaseData(totalCount, pageNumber, pageCapacity, pageNumber < lastPage, pageNumber > 1)
            {
                Books = data
            };
            return result;
        }

        public async Task<List<Guid>> AddAuthorsFromBdToBook(Guid bookId, List<Guid> authors)
        {
            // Загружаем книгу с tracking для модификации
            var book = await _context.Books
                .Include(b => b.Authors)
                .FirstOrDefaultAsync(b => b.Id == bookId);
            
            if (book == null)
            {
                throw new KeyNotFoundException($"Книга с ID {bookId} не найдена");
            }

            var authorsToAdd = await _context.Authors
                .Where(a => authors.Contains(a.Id))
                .ToListAsync();
            
            if (authorsToAdd.Count != authors.Count)
            {
                throw new ArgumentException("Не все авторы существуют в базе данных");
            }
            
            foreach (var author in authorsToAdd)
            {
                if (!book.Authors.Any(a => a.Id == author.Id))
                {
                    book.Authors.Add(author);
                }
            }
            
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync("mainpage");
            await _cache.RemoveAsync($"book:{bookId}");

            return authors;
        }

        public async Task<List<Guid>> AddGenresFromBdToBook(Guid bookId, List<Guid> genres)
        {
            // Загружаем книгу с tracking для модификации
            var book = await _context.Books
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(b => b.Id == bookId);
            
            if (book == null)
            {
                throw new KeyNotFoundException($"Книга с ID {bookId} не найдена");
            }

            var genresToAdd = await _context.Genres
                .Where(g => genres.Contains(g.Id))
                .ToListAsync();
            
            if (genresToAdd.Count != genres.Count)
            {
                throw new ArgumentException("Не все жанры существуют в базе данных");
            }
            
            foreach (var genre in genresToAdd)
            {
                if (!book.Genres.Any(g => g.Id == genre.Id))
                {
                    book.Genres.Add(genre);
                }
            }
            
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync("mainpage");
            await _cache.RemoveAsync($"book:{bookId}");

            return genres;
        }


        public async Task<Book?> GetByIdAsyncEFCORE(Guid id)
        {
            var entity = await _context.Books.AsNoTracking()
                  .Include(a => a.Authors)
                  .Include(g => g.Genres)
                  .Include(r => r.Reviews)
                      .ThenInclude(r => r.Customer)
                  .AsSplitQuery()
                  .FirstOrDefaultAsync(b => b.Id == id);

            return entity;
        }
      
       

        public async Task<bool> DeleteAsync(Guid id)
        {
            // Загружаем с tracking для удаления
            var entity = await _context.Books
                .FirstOrDefaultAsync(b => b.Id == id);
            
            if (entity == null)
            {
                return false;
            }
            
            _context.Books.Remove(entity);
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync("mainpage");
            await _cache.RemoveAsync($"book:{id}");
            return true;
        }

        public async Task<Dtos<Book>> TakeBookWithPagging(int pageSize = 20, int pageNumber = 1, string orderBy = "title", bool ascending = true)
        {

            if (pageSize <= 0) pageSize = 20;
            if (pageNumber <= 0) pageNumber = 1;

            var q = _context.Books.AsNoTracking()
                   .AsSplitQuery()
                   .Include(a => a.Authors)
                   .Include(g => g.Genres)
                   .Include(r => r.Reviews)
                   .AsQueryable();


            /*if (!string.IsNullOrEmpty(orderBy))
            {
                var orderString = $"{orderBy} {(ascending ? "asc" : "desc")}";
                q = q.OrderBy(orderString);
            }
            else
            {
                q = q.OrderBy(t => t.Title);
            }
            */
            if (SortMapper.Map.TryGetValue(orderBy, out var CurentOrdering))
            {
                q = !ascending ?
                     q.OrderByDescending(CurentOrdering) : q.OrderBy(CurentOrdering);
            }
            else
            {
                q = !ascending ?
                    q.OrderByDescending(b => b.Title) : q.OrderBy(b => b.Title);
            }
            var total = await q.CountAsync();
            var pages = (int)Math.Ceiling((double)total / pageSize);
            if (pages == 0)
            {
                return new Dtos<Book>(default, default, default, default, default)
                {
                    Items = [],
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

        public async Task<List<Guid>> GetIdsAsyncEFCORE()
        {
            return await _context.Books.AsNoTracking().Select(b => b.Id).ToListAsync();
        }
    


    

        public async Task<Book> UpdateAsync(Book entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            

            var existingBook = await _context.Books
                .FirstOrDefaultAsync(b => b.Id == entity.Id);
            
            if (existingBook == null)
            {
                throw new KeyNotFoundException($"Книга с ID {entity.Id} не найдена для обновления");
            }
            
            try
            {

                _context.Books.Update(entity);
                await _context.SaveChangesAsync();
                await _cache.RemoveAsync("mainpage");
                await _cache.RemoveAsync($"book:{entity.Id}");
                return entity;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Ошибка при обновлении книги '{entity.Title}'", ex);
            }
        }

        public async Task UpdateCountAsync(Guid id, int count)
        {
            await _context.Books
                   .Where(b => b.Id == id)
                   .ExecuteUpdateAsync(s => s.SetProperty(b => b.Count, count));
            if (count == 0)
            {
                await _cache.RemoveAsync("mainpage");
            }
            await _cache.RemoveAsync($"book:{id}");
        }

        public async Task PatchScalarFieldsAsync(Guid id, string title, string description, float rating, decimal price, string urlImage, int count, int publicationYear)
        {
            if (count == 0)
            {
                await _cache.RemoveAsync("mainpage");
            }
            await _cache.RemoveAsync($"book:{id}");
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
            ArgumentNullException.ThrowIfNull(entity);
            
            // Проверяем существование книги с tracking для корректной работы
            var existingBook = await _context.Books
                .FirstOrDefaultAsync(b => b.Id == entity.Id);
            
            if (existingBook != null)
            {
                throw new InvalidOperationException($"Book with ID {entity.Id} already exists");
            }
            
            try
            {
                _context.Books.Add(entity);
                await _context.SaveChangesAsync();
                await _cache.RemoveAsync("mainpage");
                return entity;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Ошибка при добавлении книги '{entity.Title}'", ex);
            }
        }
        public async Task<bool> AddAsyncWithExistsAuthorAndGenres(Book entity, List<Guid> authors, List<Guid> genres)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Books.Add(entity);
                await _context.SaveChangesAsync();

                if (authors.Count > 0)
                {
                    var authorsToAdd = await _context.Authors
                        .Where(a => authors.Contains(a.Id)).ToListAsync();
                    foreach (var author in authorsToAdd)
                        entity.Authors.Add(author);
                }

                if (genres.Count > 0)
                {
                    var genresToAdd = await _context.Genres
                        .Where(g => genres.Contains(g.Id)).ToListAsync();
                    foreach (var genre in genresToAdd)
                        entity.Genres.Add(genre);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                await _cache.RemoveAsync("mainpage");
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Ошибка транзакции: {ex.Message}", ex);


            }
        }
    }
}


