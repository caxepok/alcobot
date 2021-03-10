using alcobot.service.Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace alcobot.service.Services.Interfaces
{
    public interface IAlcoMetricService
    {
        Task<AlcoMetric> GetLastWeekMetrics(long userId);
        Task<AlcoMetric> GetThisWeekMetrics(long userId);
    }
}
