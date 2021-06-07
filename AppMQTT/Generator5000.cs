using AppMQTT.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppMQTT
{
    public class Generator5000
    {
        private readonly IConfiguration _config;
        private readonly SignalsRepository SignalsRepository;
        private string _ip = "5.136.92.21";
        public Generator5000(IConfiguration con)
        {
            _config = con;
            SignalsRepository = new SignalsRepository(_config);
        }
        public void GenerateMessages()
        {
            MQTTClient mq = new MQTTClient(_config);
           List<string> topics = new List<string>();
           topics.Add("testing");
           mq.ConnectAsync(_ip, "stepa", "123", topics);
           for(int i = 0; i < 2; i++)
           {
                mq.PublisherAsync("testing");
                
                //mq.Reconnect(topics);
           }
        }
    }
}
