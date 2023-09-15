using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.WindowsService
{
    public class PowerData
    {
        public PowerData(string hour)
        {
            Hour = hour;
            Volume = 0; 
        }

        public string ? Hour { get; set; }
            public int Volume { get; set; }
          
        
    }
}
