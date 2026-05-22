using Domain.Entities;
using Infrastructure.Dto;




namespace Infrastructure.Interfaces
{
    public interface IGenresRepository : IRepository<Genre> { public Task<List<IdWithName>> GetIdsWithNamesAsync(); }


}
