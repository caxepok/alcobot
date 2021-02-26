using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace alcobot.service.Services.Interfaces
{
    public interface IAlcoCounterService
    {
        Task ProcessMessage(long chatId, long userId, string message);
    }
}
