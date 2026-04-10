using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto
{
    public record BookResponseDto(Guid Id, string Title, string Description, float Rating, decimal Price,
        string UrlImage, int Count, int PublicationYear, List<AuthorResponseDto> Authors, 
        List<GenreResponseDto> Genres, List<ReviewResponseDto> Reviews);

    public record AddBookDto(string Title, string Description, float Rating, decimal Price,
        string UrlImage, int Count, int PublicationYear);

    public record GenreResponseDto(Guid id, string Name);

    public record ReviewResponseDto(Guid Id, DateTime Date, int Rating, string ReviewText,
        CustomerResponseIdNameDto Customer);

    public record CustomerResponseDto(Guid Id, string Name,  string Mail, string Phone, DateOnly DateOfBirth);

    public record AuthorResponseDto(Guid Id, string Name, int Year);

    public record AddBookWithAuthorsAndGenresDto(AddBookDto BookDto, List<Guid> AuthorsIds, List<Guid> GenresIds);

    public record  CustomerResponseIdNameDto(Guid Id, string Name);


    public record AddAuthorDto( string Name, int Year);

    public record AddGenreDto(string Name);

    public record AuthorInfoDto(Guid Id, string Name, int Year, List<(Guid, string)> Books);

    public record AddCustomerDto(string Name, string Password, string Mail, string Phone, DateOnly DateOfBirth);

    public record UpdateCustomerDto(string Name, string Mail, string Phone, DateOnly DateOfBirth);

    public record OrderItemDto(Guid BookId, int Count, decimal PriceAtPurchase);

    public record AddOrderDto(Guid CustomerId, List<OrderItemDto> Items);

    public record OrderResponseDto(int Id, Guid CustomerId, DateTime CreatedDate, decimal TotalPrice, string Status, List<OrderItemDto> Items);

    public record AddReviewDto(int Rating, string ReviewText, Guid BookId, Guid CustomerId);

    public record UpdateReviewDto(Guid Id, int Rating, string ReviewText);
}
