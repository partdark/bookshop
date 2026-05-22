using Infrastructure.Dto;

namespace Infrastructure.Interfaces
{
    public interface IReportRepository
    {
        Task<List<ReportOrderCount>> OrderCountAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<ReportOrderMoney>> OrdersMoneyAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}