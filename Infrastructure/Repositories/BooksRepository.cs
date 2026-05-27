using Dapper;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Dto;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Npgsql;
using System.Data;


namespace Infrastructure.Repositories
{
    public partial class BooksRepository : IBooksRepository
    {

        private readonly BookShopContext _context;
        private readonly HybridCache _cache;
        private readonly NpgsqlConnection _connection;


        public BooksRepository(BookShopContext context, HybridCache hybridCache, IDbConnection connectionDapper)
        {
            _context = context;
            _cache = hybridCache;
            _connection = (NpgsqlConnection)connectionDapper;
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
                q = q.Where(x => EF.Functions.ILike(x.Title, $"%{searchingWords
                    .Replace("%", "[%]")
                    .Replace("_", "[_]")
                    .Replace("[", "[[]")}%")
                );


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

            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

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
                  .AsSplitQuery()
                  .Include(a => a.Authors)
                  .Include(g => g.Genres)
                  .Include(r => r.Reviews)
                      .ThenInclude(r => r.Customer)
                  .FirstOrDefaultAsync(b => b.Id == id);

            return entity;
        }



        public async Task<bool> DeleteAsync(Guid id)
        {

            var count = await _context.Books.Where(b => b.Id == id).ExecuteDeleteAsync();
            if (count == 0)
            {
                return false;
            }

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
            var updatedCount = await _context.Books
                     .Where(b => b.Id == id)
                     .ExecuteUpdateAsync(s => s.SetProperty(b => b.Count, count));
            if (updatedCount == 0)
            {
                return;
            }
            if (count == 0)
            {
                await _cache.RemoveAsync("mainpage");
            }

            var cachedBook = await _cache.GetOrCreateAsync($"book:{id}",
                async c => await GetByIdAsync(id)
                );
            if (cachedBook != null)
            {
                cachedBook.Count = count;
                await _cache.SetAsync($"book:{id}", cachedBook);

            }


        }

        public async Task PatchScalarFieldsAsync(Guid id, string title, string description, float rating, decimal price, string urlImage, int count, int publicationYear)
        {

            var result = await _context.Books
                 .Where(b => b.Id == id)
                 .ExecuteUpdateAsync(s => s
                     .SetProperty(b => b.Title, title)
                     .SetProperty(b => b.Description, description)
                     .SetProperty(b => b.Rating, rating)
                     .SetProperty(b => b.Price, price)
                     .SetProperty(b => b.UrlImage, urlImage)
                     .SetProperty(b => b.Count, count)
                     .SetProperty(b => b.PublicationYear, publicationYear));
            if (result == 0)
            {
                return;
            }

            await _cache.RemoveAsync("mainpage");
            await _cache.RemoveAsync($"book:{id}");


        }

        public async Task<Book?> AddAsync(Book entity)
        {
            ArgumentNullException.ThrowIfNull(entity);


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


