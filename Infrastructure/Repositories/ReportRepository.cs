using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Dto;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{

    public class ReportRepository : IReportRepository
    {
        private readonly BookShopContext _context;

        public ReportRepository(BookShopContext context)
        {
            _context = context;
        }

        public async Task<List<ReportOrderCount>> OrderCountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var q = _context.Orders.AsQueryable();



            if (startDate.HasValue)
            {

                q = q.Where(o => o.CreatedDate >= ToUtc(startDate.Value));
            }
            if (endDate.HasValue)
            {

                q = q.Where(o => o.CreatedDate <= ToUtc(endDate.Value));
            }

            var result = await q.GroupBy(o => o.Status)
                .Select(o => new ReportOrderCount
                (
                    o.Key.ToString(),
                    o.Count()
                )
                ).ToListAsync();

            return result;
        }

        public async Task<List<ReportOrderMoney>> OrdersMoneyAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var q = _context.Orders.AsQueryable();
            if (startDate.HasValue)
            {

                q = q.Where(o => o.CreatedDate >= ToUtc(startDate.Value));
            }
            if (endDate.HasValue)
            {

                q = q.Where(o => o.CreatedDate <= ToUtc(endDate.Value));
            }



            var result = await q.GroupBy(o => o.Status).Select(o => new ReportOrderMoney(o.Key.ToString(), o.Count(), o.Sum(p => p.TotalPrice))).ToListAsync();
            result.Add(new ReportOrderMoney("Total", result.Select(r => r.Count).Sum(), result.Select(r => r.TotalMoney).Sum()));

            return result;

        }


        private DateTime ToUtc(DateTime date)
        {
            return date.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(date, DateTimeKind.Utc)
                : date.ToUniversalTime();
        }
    }
}
