using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.WindowsService
{
    public class PowerServiceWrapper : IPowerTradeProvider
    {
        public Task<IEnumerable<PowerTrade>> GetPowerTradesAsync()
        {
            PowerService powerService = new();
            return powerService.GetTradesAsync(DateTime.Now);
        }

        public Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date)
        {
            PowerService powerService = new();
            return powerService.GetTradesAsync(date);
        }

        public IEnumerable<PowerTrade> GetTrades(DateTime date)
        {
            PowerService powerService = new();
            return powerService.GetTrades(date);
        }
    }
}
