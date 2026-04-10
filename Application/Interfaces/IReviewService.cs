using Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IReviewService
    {
        Task<List<ReviewResponseDto>> GetAll();
        Task<ReviewResponseDto?> GetById(Guid id);
        Task<Guid> Add(AddReviewDto review);
        Task<ReviewResponseDto?> Update(UpdateReviewDto review);
        Task<bool> Delete(Guid id);
    }
}
