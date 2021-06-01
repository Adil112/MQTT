using AppMQTT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using AppMQTT.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AppMQTT;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Data;

namespace AppMQTT.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly SignalsRepository SignalsRepository;
        private readonly ILogger<HomeController> _logger;
        private string _ip = "5.136.92.21";
        public MQTTnet.Client.IMqttClient mqttClient;
        private readonly IConfiguration _config;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            SignalsRepository = new SignalsRepository(configuration);
            _config = configuration;

        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(string theme, string Message, string username, string password)
        {
            MQTTClient mq = new MQTTClient(_config);
            List<string> topics = new List<string>();
            topics.Add("test1");
            mq.ConnectAsync(_ip, "stepa", "123", topics);
            mq.PublisherAsync("test1");
            int i = 0;
            await foreach (var m in mq.Receive())
            {
                mq.Save(m);
                i++;
                if (i == 2) break;
                mq.Reconnect(topics);
            }
            var recData = SignalsRepository.FindAll();
            string result = null;
            
            foreach (var t in recData)
            {
                result += t.Name + ' ' + t.Data + ' ' + t.Time + ' ' + t.Quality + "\n";
            }
            return Content(result);
        }

        [HttpGet]
        public IActionResult History()
        {
            List<History> history = new List<History>();
            var viewModel = new History() { Time = "1", Value = "1" };
            var viewModel2 = new History() { Time = "2", Value = "2" };
            history.Add(viewModel);
            history.Add(viewModel2);
            var res = new HistoryDatas() { Histories = history };
            return View(res);
        }
        [HttpPost]
        public IActionResult History(DateTime time1, DateTime time2, string name = "PLK1")
        {
            var data = SignalsRepository.FindByData(time1, time2);
            List<History> history = new List<History>();

            Object[,] o = new object[2, data.Count()];
            int i = 0; int j = 0;

            foreach (var a in data)
            {
                var viewModel = new History()
                {
                    Value = a.Data,
                    Time = a.Time.Year.ToString() + '-' + a.Time.Month.ToString() + '-' + a.Time.Day.ToString() + ' ' + a.Time.Hour.ToString() + ':' + a.Time.Minute.ToString() + ':' + a.Time.Second.ToString()
                };
                o[i, j] = viewModel.Time;
                o[i+1, j] = viewModel.Value;
                history.Add(viewModel);
                j++;
            }
            var res = new HistoryDatas() { Histories = history };
            ViewBag.Dd = o;
            return View(res);
        }

        


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}