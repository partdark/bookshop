using Dapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Infrastructure.Repositories
{
    partial class BooksRepository
    {
        public async Task<Book?> GetByIdAsync(Guid id)
        {
            Console.WriteLine(id);
            if (_connection.State != ConnectionState.Open)
                await _connection.OpenAsync();
            var sqlBook = @"SELECT * FROM ""Books"" b where b.""Id"" = @Id LIMIT 1                       
                                                                             ";
            var book = await _connection.QueryFirstOrDefaultAsync<Book?>(sqlBook, new { Id = id });

            if (book == null)
            {
                return null;
            }
            Console.WriteLine(book.Id);

            var sqlAuthors = @"SELECT a.* 
                            FROM ""Authors"" a
                                LEFT JOIN ""AuthorBook"" ab ON a.""Id"" = ab.""AuthorsId""
                                WHERE ab.""BooksId"" = @Id
                                                        ";
            var authors = (await _connection.QueryAsync<Author?>(sqlAuthors, new { Id = id })).ToList();




            var sqlGenrs = @"SELECT g.* 
                            FROM ""Genre"" g
                                LEFT JOIN ""BookGenre"" bg ON g.""Id"" = bg.""GenresId""
                                WHERE bg.""BooksId"" = @Id
                                                        ";
            var genres = (await _connection.QueryAsync<Genre?>(sqlGenrs, new { Id = id })).ToList();





            var sqlReviews = @"SELECT r.*,c.*  FROM ""Reviews"" r 
                              LEFT JOIN ""AspNetUsers"" c ON r.""CustomerId"" = c.""Id""
                              WHERE r.""BookId"" = @Id
                                                            ";
            var reviews = (await _connection.QueryAsync<Review, Customer, Review>(sqlReviews, (reviews, customer) =>
            {
                if (customer != null)
                    reviews.Customer = customer;
                return reviews;
            }, new { Id = id }, splitOn: "Id")).ToList();
            book.Authors = authors;
            book.Genres = genres;
            book.Reviews = reviews;


            return book;

        }

        public async Task<List<Guid>> GetIdsAsync()
        {
            if (_connection.State != ConnectionState.Open)
                await _connection.OpenAsync();
            var sql = """SELECT b."Id" FROM "Books" b""";
            return (await _connection.QueryAsync<Guid>(sql)).ToList();

        }
    }
}
