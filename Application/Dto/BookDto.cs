using Infrastructure.Dto;
using System.ComponentModel.DataAnnotations;

namespace Application.Dto
{
    public record BookResponseDto(Guid Id, string Title, string Description, float Rating, decimal Price,
        string UrlImage, int Count, int PublicationYear, List<AuthorResponseDto> Authors,
        List<GenreResponseDto> Genres, List<ReviewResponseDto> Reviews);

    public record AddBookDto(
        [Required, MaxLength(300)] string Title,
        [MaxLength(5000)] string Description,
        [Range(0, 5)] float Rating,
        [Range(0, 1_000_000)] decimal Price,
        [MaxLength(1000)] string UrlImage,
        [Range(0, 100_000)] int Count,
        [Range(1000, 2100)] int PublicationYear);

    public record GenreResponseDto(Guid id, [Required, MaxLength(100)] string Name);

    public record ReviewResponseDto(Guid Id, DateTime Date, int Rating, string ReviewText,
        CustomerResponseIdNameDto Customer);

    public record CustomerResponseDto(Guid Id, string Name, string Mail, string Phone, DateOnly DateOfBirth);

    public record AuthorResponseDto(Guid Id, [Required, MaxLength(200)] string Name, int Year);

    public record AddBookWithAuthorsAndGenresDto(
        [Required] AddBookDto BookDto,
        [Required] List<Guid> AuthorsIds,
        [Required] List<Guid> GenresIds);

    public record CustomerResponseIdNameDto(Guid Id, string Name);

    public record AddAuthorDto(
        [Required, MaxLength(200)] string Name,
        [Range(0, 2100)] int Year);

    public record AddGenreDto([Required, MaxLength(100)] string Name);

    public record AuthorInfoDto(Guid Id, string Name, int Year, List<(Guid, string)> Books);

    public record AddCustomerDto(
        [Required, MaxLength(100)] string Name,
        [Required, MinLength(4), MaxLength(100)] string Password,
        [Required, EmailAddress, MaxLength(200)] string Mail,
        [Phone, MaxLength(20)] string Phone,
        DateOnly DateOfBirth);

    public record UpdateCustomerDto(
        [Required, MaxLength(100)] string Name,
        [Required, EmailAddress, MaxLength(200)] string Mail,
        [Phone, MaxLength(20)] string Phone,
        DateOnly DateOfBirth);



    public record OrderResponseDto(int Id, Guid CustomerId, DateTime CreatedDate, decimal TotalPrice,
        string Status, List<OrderItemDto> Items);

    public record OrderItemDetailDto(
        Guid BookId,
        string BookTitle,
        string BookUrlImage,
        int Count,
        decimal PriceAtPurchase,
        decimal Subtotal);

    public record OrderDetailDto(
        int Id,
        Guid CustomerId,
        string CustomerName,
        string CustomerEmail,
        DateTime CreatedDate,
        decimal TotalPrice,
        string Status,
        List<OrderItemDetailDto> Items);

    public record UpdateOrderStatusDto([Required] string Status);

    public record AddReviewDto(
        [Range(1, 5)] int Rating,
        [Required, MaxLength(2000)] string ReviewText,
        [Required] Guid BookId,
        [Required] Guid CustomerId);

    public record UpdateReviewDto(
        [Required] Guid Id,
        [Range(1, 5)] int Rating,
        [Required, MaxLength(2000)] string ReviewText);

    public record CartItemDto(BookResponseDto Book, int Quantity);
}
