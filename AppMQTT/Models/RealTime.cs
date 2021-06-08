using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppMQTT.Models
{
    public class RealTime
    {
        public string Name { get; set; }
        public string Data { get; set; }
        public DateTime Time { get; set; }
        public int Quality { get; set; }
        public string Edizm { get; set; }

    }
}
