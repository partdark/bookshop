using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Dto;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace bookshop.Controllers
{

    [ApiController]
    [Route("api")]
    public class ReportController : ControllerBase
    {

        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("report/orders")]
        [ProducesResponseType(typeof(List<ReportOrderCount>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult<List<ReportOrderCount>>> GetCount(DateTime? startDate = null, DateTime? endDate = null)
        {
            var result = await _reportService.ReportGetCountGroupByTypeAsync(startDate, endDate);
            if (result.Count == 0)
            {
                return NoContent();
            }
            return result;

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("report/money")]
        [ProducesResponseType(typeof(List<ReportOrderCount>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ReportOrderMoney>>> GetTypeCountTotalMoney(DateTime? startDate = null, DateTime? endDate = null)
        {
            var result = await _reportService.ReportMoneyByTypesAsync(startDate, endDate);
            if (result.Count == 0)
            {
                return NoContent();
            }
            return result;

        }

    }
}
