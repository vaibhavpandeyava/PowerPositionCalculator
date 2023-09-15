using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.WindowsService
{
    public interface IPowerTradeProvider: IPowerService
    {
      
        Task<IEnumerable<PowerTrade>> IPowerService.GetTradesAsync(DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}
