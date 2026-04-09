using Application.Dto;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Dto;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;

namespace Application.Services
{

    public class BookService : IBookService
    {
        private readonly IBooksRepository _booksRepository;

        public BookService(IBooksRepository booksRepository)
        {
            _booksRepository = booksRepository;
        }

        public async Task<Guid> CreateBookWithIndicatingExistingAuthorsAndgenres(AddBookDto bookDto,
            List<Guid> authorDto, List<Guid> genres)
        {
            var book = new Book
            {
                Title = bookDto.Title,
                Description = bookDto.Description,
                Rating = 0,
                Price = bookDto.Price,
                UrlImage = bookDto.UrlImage,
                Count = bookDto.Count,
                PublicationYear = bookDto.PublicationYear,
                Authors = new List<Author>(),
                Genres = new List<Genre>(),
                Reviews = new List<Review>()

            };
            var result = await _booksRepository.AddAsyncWithExistsAuthorAndGenres(book, authorDto, genres);

            return book.Id;




        }

        public async Task<BookResponseDto?> GetById(Guid id)
        {
            var book = await _booksRepository.GetByIdAsync(id);
            if (book != null)
            {
                return ConvertToDto(book);
            }
            return null;
        }
        public async Task<Guid> AddBook(AddBookDto bookDto)
        {
            var book = await _booksRepository.AddAsync(new Book
            {
                Title = bookDto.Title,
                Description = bookDto.Description,
                Rating = 0,
                Price = bookDto.Price,
                UrlImage = bookDto.UrlImage,
                Count = bookDto.Count,
                PublicationYear = bookDto.PublicationYear,
                Authors = new List<Author>(),
                Genres = new List<Genre>(),
                Reviews = new List<Review>()

            });
            return book.Id;

        }

        public async Task<List<Guid>> GetBookSIds()
        {
            return await _booksRepository.GetIdsAsync();
        }

        public BookResponseDto? ConvertToDto(Book book)
        {
            return new BookResponseDto(
                book.Id,
               book.Title,
                book.Description,
                book.Rating,
                book.Price,
                book.UrlImage,
               book.Count,
                book.PublicationYear,
                book.Authors.Select(a => new AuthorResponseDto(a.Id, a.Name, a.Year)).ToList(),
                book.Genres.Select(g => new GenreResponseDto(g.Id, g.Name)).ToList(),
                book.Reviews.Select(r => new ReviewResponseDto(r.Id, r.Date, r.Rating, r.ReviewText, new CustomerResponseDto(r.Customer.Id, r.Customer.Name))).ToList()
                );
        }

        public async Task<ListWithBooksBaseData> BookShowcase(int pageCapacity, int pageNumber, string orderBy,
            bool notAscending, string? searchingWords)
        {
            return await _booksRepository.BooksBaseData(pageCapacity, pageNumber, orderBy, notAscending, searchingWords);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _booksRepository.DeleteAsync(id);
        }

        public async Task<BookResponseDto?> UpdateBook(BookResponseDto bookResponse)
        {
            var bookToUpdate = _booksRepository.GetByIdAsync(bookResponse.Id);
            if (bookToUpdate == null)
            {
                return null;
            }
            var book = new Book()
            {
                Id = bookResponse.Id,
                Title = bookResponse.Title,
                Description = bookResponse.Description,
                Rating = bookResponse.Rating,
                Price = bookResponse.Price,
                UrlImage = bookResponse.UrlImage,
                Count = bookResponse.Count,
                PublicationYear = bookResponse.PublicationYear,
                Authors = (ICollection<Author>)bookResponse.Authors,
                Genres = (ICollection<Genre>)bookResponse.Genres,
                Reviews = (ICollection<Review>)bookResponse.Reviews,
            };
            await _booksRepository.UpdateAsync(book);
            return bookResponse;

        }

        public async Task<AddBookDto?> PatchBook(Guid id, JsonPatchDocument<AddBookDto> patchBook)
        {
            var book = await _booksRepository.GetByIdAsync(id);
            if (book == null)
            {
                return null;
            }
            var bookDto = new AddBookDto(
                     book.Title,
                     book.Description,
                     book.Rating,
                     book.Price,
                     book.UrlImage,
                      book.Count,
                     book.PublicationYear
                    );


            patchBook.ApplyTo(bookDto);



            book.Title = bookDto.Title;
            book.Description = bookDto.Description;
            book.Rating = bookDto.Rating;
            book.Price = bookDto.Price;
            book.UrlImage = bookDto.UrlImage;
            book.Count = bookDto.Count;
            book.PublicationYear = bookDto.PublicationYear;


            await _booksRepository.UpdateAsync(book);
            return bookDto;
        }
    }
}
