using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Dto;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

using System.Linq.Dynamic.Core;
using System.Runtime.InteropServices.Marshalling;


namespace Infrastructure.Repositories
{
    public partial class BooksRepository : IBooksRepository
    {
        private readonly BookShopContext _context;


        public BooksRepository(BookShopContext context)
        {
            _context = context;
        }




        public async Task<ListWithBooksBaseData> BooksBaseData(int pageCapacity = 20, int pageNumber = 1, string orderBy = "Title",
            bool notAscending = false, string? searchingWords = null)
        {

            var q = _context.Books.AsQueryable();

            if (searchingWords != null)
            {
                q = q.Where(x => EF.Functions.Like(x.Title, $"%{searchingWords}%"));
            }


            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = "Title";
            }
            var orderType = notAscending ? "desc" : "asc";
            q = q.OrderBy($"{orderBy}  {orderType}");

            var booksCount = await _context.Books.CountAsync();

            if (pageCapacity < 1) pageCapacity = 20;
            if (pageNumber < 1) pageNumber = 1;
            var lastPage = (int)Math.Ceiling((double)booksCount / pageCapacity);
            pageNumber = Math.Min(pageNumber, lastPage);



            q = q.Skip(pageCapacity * (pageNumber - 1)).Take(pageCapacity);

            var data = await q.Select(q => new BookBaseData(
           q.Id,
           q.Title,
           q.Description,
           q.Rating,
           q.Price,
           q.UrlImage,
           q.Count,
           q.PublicationYear
           )).ToListAsync();


            var result = new ListWithBooksBaseData(lastPage, pageNumber < lastPage, pageNumber > 1) { Books = data };
            return result;
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

        public async Task<PaginatedResult<Book>> TakeBookWithPagging(int pageSize = 20, int pageNumber = 1, string orderBy = "title", bool ascending = true)
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
                return new PaginatedResult<Book>(default, default, default, default, default)
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

            return new PaginatedResult<Book>(default, default, default, default, default)
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
