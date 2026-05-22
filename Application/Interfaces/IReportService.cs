using Infrastructure.Dto;

namespace Application.Interfaces
{
    public interface IReportService
    {
        Task<List<ReportOrderCount>> ReportGetCountGroupByTypeAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<ReportOrderMoney>> ReportMoneyByTypesAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}