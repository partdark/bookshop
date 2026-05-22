using Application.Interfaces;
using Infrastructure.Dto;
using Microsoft.AspNetCore.Mvc;


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
        //[Authorize(Roles = "Admin")]
        [HttpGet("report/orders")]
        [ProducesResponseType(typeof(List<ReportOrderCount>), StatusCodes.Status200OK)]
          [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult<List<ReportOrderCount>>> GetCount([FromQuery] string? startDate = null, [FromQuery] string? endDate = null)
        {
            DateTime? start = null;
            DateTime? end = null;
            
            if (!string.IsNullOrEmpty(startDate))
            {
                if (DateTime.TryParseExact(startDate, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var s))
                    start = s.Date;
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                if (DateTime.TryParseExact(endDate, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var e))
                    end = e.Date.AddDays(1).AddTicks(-1);
            }

            var result = await _reportService.ReportGetCountGroupByTypeAsync(start, end);
          
            return result;

        }

       // [Authorize(Roles = "Admin")]
        [HttpGet("report/money")]
        [ProducesResponseType(typeof(List<ReportOrderCount>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ReportOrderMoney>>> GetTypeCountTotalMoney([FromQuery] string? startDate = null, [FromQuery] string? endDate = null)
        {
            DateTime? start = null;
            DateTime? end = null;
            
            if (!string.IsNullOrEmpty(startDate))
            {
                if (DateTime.TryParseExact(startDate, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var s))
                    start = s.Date;
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                if (DateTime.TryParseExact(endDate, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var e))
                    end = e.Date.AddDays(1).AddTicks(-1);
            }

            var result = await _reportService.ReportMoneyByTypesAsync(start, end);
          
            return result;

        }

    }
}
