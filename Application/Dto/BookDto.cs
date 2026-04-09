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
        CustomerResponseDto Customer);

    public record CustomerResponseDto(Guid Id, string Name);

    public record AuthorResponseDto(Guid Id, string Name, int Year);

    public record AddBookWithAuthorsAndGenresDto(AddBookDto BookDto, List<Guid> AuthorsIds, List<Guid> GenresIds);



}
