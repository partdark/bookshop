using Application.Interfaces;
using Infrastructure.Dto;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {

            _reportRepository = reportRepository;
        }

        public async Task<List<ReportOrderCount>> ReportGetCountGroupByTypeAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _reportRepository.OrderCountAsync(startDate, endDate);
        }
        public async Task<List<ReportOrderMoney>> ReportMoneyByTypesAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _reportRepository.OrdersMoneyAsync(startDate, endDate);
        }
    }
}
