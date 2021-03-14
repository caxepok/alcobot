﻿using alcobot.service.Infrastructure;
using alcobot.service.Models;
using alcobot.service.Models.API;
using alcobot.service.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace alcobot.service.Services
{
    /// <inheritdoc cref="IAlcoMetricService"/>
    public class AlcoMetricService : IAlcoMetricService
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public AlcoMetricService(ILogger<AlcoMetricService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task<AlcoMetric> GetLastWeekMetrics(long userId)
        {
            DateTimeOffset from = GetWeekStart(DateTimeOffset.Now.Date.AddDays(-7));
            DateTimeOffset to = from.AddDays(7);
            return await GetMetric(userId, from, to);
        }

        public async Task<AlcoMetric> GetThisWeekMetrics(long userId)
        {
            DateTimeOffset from = GetWeekStart(DateTimeOffset.Now.Date);
            DateTimeOffset to = from.AddDays(7);
            return await GetMetric(userId, from, to);
        }

        private async Task<AlcoMetric> GetMetric(long userId, DateTimeOffset from, DateTimeOffset to)
        {
            var dbContext = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AlcoDBContext>();
            List<Drink> drinks = await dbContext.Drinks.Where(_ => _.UserId == userId && _.Timestamp >= from && _.Timestamp < to).ToListAsync();
            return new AlcoMetric(drinks) { From = from, To = to };
        }

        private DateTimeOffset GetWeekStart(DateTimeOffset date) =>
            date.AddDays(-(date.DayOfWeek == DayOfWeek.Sunday ? 6 : ((int)date.DayOfWeek - 1)));
    }
}
