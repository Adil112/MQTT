using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppMQTT.Models
{
    public class HistoryDatas
    {
        public IEnumerable<History> Histories { get; set; }

        public static implicit operator HistoryDatas(List<History> v)
        {
            throw new NotImplementedException();
        }
    }
}
