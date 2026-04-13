namespace Application.Interfaces
{
    public interface IRatingService
    {
        /// <summary>Пересчитать рейтинг одной книги по её отзывам.</summary>
        Task RecalculateAsync(Guid bookId);

        /// <summary>Пересчитать рейтинг всех книг.</summary>
        Task RecalculateAllAsync();
    }
}
