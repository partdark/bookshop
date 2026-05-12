using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public static class DbSeeder
{
    public static async Task SeedAsync(BookShopContext context)
    {
        if (await context.Books.AnyAsync()) return;

        var genres = new List<Genre>
        {
            new() { Id = Guid.NewGuid(), Name = "Классика" },
            new() { Id = Guid.NewGuid(), Name = "Фантастика" },
            new() { Id = Guid.NewGuid(), Name = "Детектив" },
            new() { Id = Guid.NewGuid(), Name = "Роман" },
            new() { Id = Guid.NewGuid(), Name = "Приключения" },
            new() { Id = Guid.NewGuid(), Name = "Психология" },
            new() { Id = Guid.NewGuid(), Name = "История" },
            new() { Id = Guid.NewGuid(), Name = "Философия" },
        };
        context.Genres.AddRange(genres);

        var authors = new List<Author>
        {
            new() { Id = Guid.NewGuid(), Name = "Фёдор Достоевский", Year = 1821 },
            new() { Id = Guid.NewGuid(), Name = "Лев Толстой", Year = 1828 },
            new() { Id = Guid.NewGuid(), Name = "Михаил Булгаков", Year = 1891 },
            new() { Id = Guid.NewGuid(), Name = "Антон Чехов", Year = 1860 },
            new() { Id = Guid.NewGuid(), Name = "Александр Пушкин", Year = 1799 },
            new() { Id = Guid.NewGuid(), Name = "Иван Тургенев", Year = 1818 },
            new() { Id = Guid.NewGuid(), Name = "Николай Гоголь", Year = 1809 },
            new() { Id = Guid.NewGuid(), Name = "Борис Пастернак", Year = 1890 },
        };
        context.Authors.AddRange(authors);

        var g = genres.ToDictionary(x => x.Name);
        var a = authors.ToDictionary(x => x.Name);

        var books = new List<Book>
        {
            new()
            {
                Title = "Преступление и наказание",
                Description = "Роман о студенте Раскольникове, совершившем убийство и терзаемом муками совести. Глубокое исследование психологии преступника и нравственного возрождения.",
                Rating = 4.8f, Price = 590, Count = 15, PublicationYear = 1866,
                UrlImage = "https://cdn.azbooka.ru/cv/w383/webp/a915208d-9e3a-4293-beb9-06d77344f3d5.webp",
                Authors = new List<Author> { a["Фёдор Достоевский"] },
                Genres = new List<Genre> { g["Классика"], g["Психология"] }
            },
            new()
            {
                Title = "Идиот",
                Description = "История князя Мышкина — человека с чистой душой, попавшего в мир лжи и страстей петербургского общества.",
                Rating = 4.7f, Price = 620, Count = 10, PublicationYear = 1869,
                UrlImage = "https://cdn.azbooka.ru/cv/w1100/e7ee6556-090e-4648-8806-63d536b75936.jpg",
                Authors = new List<Author> { a["Фёдор Достоевский"] },
                Genres = new List<Genre> { g["Классика"], g["Роман"] }
            },
            new()
            {
                Title = "Война и мир",
                Description = "Эпический роман о судьбах нескольких дворянских семей на фоне Отечественной войны 1812 года. Одно из величайших произведений мировой литературы.",
                Rating = 4.9f, Price = 890, Count = 8, PublicationYear = 1869,
                UrlImage = "https://cdn.azbooka.ru/cv/w383/webp/7ddfe6cf-3467-4194-a037-44fecd24ca51.webp",
                Authors = new List<Author> { a["Лев Толстой"] },
                Genres = new List<Genre> { g["Классика"], g["История"], g["Роман"] }
            },
            new()
            {
                Title = "Анна Каренина",
                Description = "Трагическая история замужней женщины, решившейся на любовь вне брака. Роман о свободе, долге и неизбежности судьбы.",
                Rating = 4.7f, Price = 750, Count = 12, PublicationYear = 1878,
                UrlImage = "https://cdn.azbooka.ru/cv/w1100/a7fa791d-1eaa-40e5-9418-1c84d01a69b9.jpg",
                Authors = new List<Author> { a["Лев Толстой"] },
                Genres = new List<Genre> { g["Классика"], g["Роман"] }
            },
            new()
            {
                Title = "Мастер и Маргарита",
                Description = "Мистический роман о визите дьявола в советскую Москву. Переплетение двух сюжетных линий — современной и библейской — создаёт уникальное произведение.",
                Rating = 4.9f, Price = 680, Count = 20, PublicationYear = 1967,
                UrlImage = "https://cdn.azbooka.ru/cv/w383/webp/98fa6b42-e86d-4f17-9376-25e98cc784e5.webp",
                Authors = new List<Author> { a["Михаил Булгаков"] },
                Genres = new List<Genre> { g["Классика"], g["Фантастика"] }
            },
            new()
            {
                Title = "Собачье сердце",
                Description = "Сатирическая повесть о профессоре Преображенском, пересадившем собаке человеческий гипофиз. Острая критика советского общества.",
                Rating = 4.8f, Price = 420, Count = 18, PublicationYear = 1925,
                UrlImage = "https://covers.openlibrary.org/b/id/8739168-L.jpg",
                Authors = new List<Author> { a["Михаил Булгаков"] },
                Genres = new List<Genre> { g["Классика"], g["Фантастика"] }
            },
            new()
            {
                Title = "Вишнёвый сад",
                Description = "Последняя пьеса Чехова о гибели дворянского уклада жизни. История семьи, теряющей родовое имение с вишнёвым садом.",
                Rating = 4.5f, Price = 380, Count = 14, PublicationYear = 1904,
                UrlImage = "https://cdn.azbooka.ru/cv/w383/webp/ce9168ea-2f18-443c-8c05-216a50e22c6a.webp",
                Authors = new List<Author> { a["Антон Чехов"] },
                Genres = new List<Genre> { g["Классика"] }
            },
            new()
            {
                Title = "Палата №6",
                Description = "Повесть о враче психиатрической больницы, постепенно сближающемся с пациентами. Философское размышление о свободе и безумии.",
                Rating = 4.6f, Price = 350, Count = 9, PublicationYear = 1892,
                UrlImage = "https://cdn.azbooka.ru/cv/w383/webp/6b1d7378-517e-43e3-9c5d-fc7a6fc8aa79.webp",
                Authors = new List<Author> { a["Антон Чехов"] },
                Genres = new List<Genre> { g["Классика"], g["Психология"] }
            },
            new()
            {
                Title = "Евгений Онегин",
                Description = "Роман в стихах — энциклопедия русской жизни первой трети XIX века. История несостоявшейся любви Онегина и Татьяны.",
                Rating = 4.7f, Price = 450, Count = 22, PublicationYear = 1833,
                UrlImage = "https://cdn.azbooka.ru/cv/w383/webp/8fb16b40-17d4-43fa-b3fa-20238b342ad3.webp",
                Authors = new List<Author> { a["Александр Пушкин"] },
                Genres = new List<Genre> { g["Классика"], g["Роман"] }
            },
            new()
            {
                Title = "Капитанская дочка",
                Description = "Исторический роман о временах Пугачёвского восстания. История любви и чести на фоне народного бунта.",
                Rating = 4.6f, Price = 390, Count = 16, PublicationYear = 1836,
                UrlImage = "https://cdn.azbooka.ru/cv/w383/webp/c064c378-2356-40be-a612-a1e617ebb6a9.webp",
                Authors = new List<Author> { a["Александр Пушкин"] },
                Genres = new List<Genre> { g["Классика"], g["История"], g["Приключения"] }
            },
            new()
            {
                Title = "Отцы и дети",
                Description = "Роман о конфликте поколений и нигилизме. История Базарова — человека, отрицающего все авторитеты и традиции.",
                Rating = 4.5f, Price = 480, Count = 11, PublicationYear = 1862,
                UrlImage = "https://cdn.azbooka.ru/cv/w383/webp/674e6485-e248-4f4c-a337-a94170c0d455.webp",
                Authors = new List<Author> { a["Иван Тургенев"] },
                Genres = new List<Genre> { g["Классика"], g["Роман"] }
            },
            new()
            {
                Title = "Мёртвые души",
                Description = "Поэма в прозе о помещике Чичикове, скупающем «мёртвые души» крепостных. Сатирическая панорама провинциальной России.",
                Rating = 4.6f, Price = 520, Count = 13, PublicationYear = 1842,
                UrlImage = "https://cdn.azbooka.ru/cv/w383/webp/f58da535-6bff-4f54-84c3-10d1afde210b.webp",
                Authors = new List<Author> { a["Николай Гоголь"] },
                Genres = new List<Genre> { g["Классика"], g["Приключения"] }
            },
            new()
            {
                Title = "Ревизор",
                Description = "Комедия о чиновниках провинциального города, принявших мелкого чиновника за ревизора из Петербурга. Бессмертная сатира на бюрократию.",
                Rating = 4.5f, Price = 320, Count = 25, PublicationYear = 1836,
                UrlImage = "https://cdn.azbooka.ru/cv/w383/webp/4f139b55-e221-4cdc-930e-ea19f10b9799.webp",
                Authors = new List<Author> { a["Николай Гоголь"] },
                Genres = new List<Genre> { g["Классика"] }
            },
            new()
            {
                Title = "Доктор Живаго",
                Description = "Роман о судьбе русского интеллигента на фоне революции и Гражданской войны. Лауреат Нобелевской премии по литературе 1958 года.",
                Rating = 4.6f, Price = 710, Count = 7, PublicationYear = 1957,
                UrlImage = "https://cdn.azbooka.ru/cv/w383/webp/24efe788-0c3b-4215-a482-d6dc149da9fe.webp",
                Authors = new List<Author> { a["Борис Пастернак"] },
                Genres = new List<Genre> { g["Классика"], g["История"], g["Роман"] }
            },
            new()
            {
                Title = "Братья Карамазовы",
                Description = "Последний и самый масштабный роман Достоевского. История трёх братьев, их отца и убийства, ставшего испытанием для каждого.",
                Rating = 4.9f, Price = 820, Count = 6, PublicationYear = 1880,
                UrlImage = "https://cdn.azbooka.ru/cv/w383/webp/73c4b879-1553-4c03-98b4-f7ab4a82f6e9.webp",
                Authors = new List<Author> { a["Фёдор Достоевский"] },
                Genres = new List<Genre> { g["Классика"], g["Детектив"], g["Философия"] }
            },
        };

        context.Books.AddRange(books);
        await context.SaveChangesAsync();
    }
}
