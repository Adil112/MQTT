using AppMQTT.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AppMQTT
{
    public class Background
    {
        private readonly IConfiguration _config;
        private readonly SignalsRepository SignalsRepository;
        private string _ip = "5.136.92.21";

        public Background(IConfiguration con)
        {
            _config = con;
            SignalsRepository = new SignalsRepository(_config);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task CatchMessage()
        {
            Generator5000 gg = new Generator5000(_config);
            gg.GenerateMessages();
            await Task.Delay(TimeSpan.FromSeconds(2));
            MQTTClient mq = new MQTTClient(_config);
            List<string> topics = new List<string>();
            topics.Add("testing");
            mq.ConnectAsync(_ip, "stepa", "123", topics);
            var m = mq.ReceiveAsync();
            await Task.Delay(TimeSpan.FromSeconds(2));
            mq.Save(m.Result.ToString());
            mq.Reconnect(topics);
            //TimeSpan.FromSeconds(2);
        }
    }
}
