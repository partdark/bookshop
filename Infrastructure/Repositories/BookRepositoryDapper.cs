using Dapper;
using Domain.Entities;
using Npgsql;
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
          
            if (_connection.State != ConnectionState.Open)
                await _connection.OpenAsync();

            var sql = @"
                SELECT * FROM ""Books"" WHERE ""Id"" = @Id;
                SELECT a.""Name"", a.""Year"" FROM ""Authors"" a JOIN ""AuthorBook"" ab ON a.""Id"" = ab.""AuthorsId"" WHERE ab.""BooksId"" = @Id;
                SELECT g.""Name"" FROM ""Genre"" g JOIN ""BookGenre"" bg ON g.""Id"" = bg.""GenresId"" WHERE bg.""BooksId"" = @Id;
               ";

            using (var data = await _connection.QueryMultipleAsync(sql, new { Id = id }))
            {
                var book = await data.ReadFirstOrDefaultAsync<Book>();
                if (book == null) return null;

                book.Authors = (await data.ReadAsync<Author>()).ToList();
                book.Genres = (await data.ReadAsync<Genre>()).ToList();

                var sqlReviews = @"SELECT r.*,c.""Id"", c.""UserName""  FROM ""Reviews"" r 
                              LEFT JOIN ""AspNetUsers"" c ON r.""CustomerId"" = c.""Id""
                              WHERE r.""BookId"" = @Id
                                                            ";
                book.Reviews = (await _connection.QueryAsync<Review, Customer, Review>(sqlReviews, (reviews, customer) =>
                {
                    if (customer != null)
                        reviews.Customer = customer;
                    return reviews;
                }, new { Id = id }, splitOn: "Id")).ToList();



                return book;


            }
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
