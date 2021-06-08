using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using AppMQTT.Models;
using AppMQTT.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net;

namespace AppMQTT
{
    public class MQTTClient
    {
        public MQTTnet.Client.IMqttClient mqttClient;
        public MQTTnet.Client.Options.IMqttClientOptions mainOptions = null;
        private readonly SignalsRepository SignalsRepository;

        public MQTTClient(IConfiguration configuration)
        {
            SignalsRepository = new SignalsRepository(configuration);
        }
        public async void ConnectAsync(string ip, string username, string password, List<string> topics)
        {
            var factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
            .WithTcpServer(ip)
            .WithCredentials(username, password)
            .Build();
            mainOptions = options;
            await mqttClient.ConnectAsync(options, CancellationToken.None);
            foreach (var t in topics)
            {
                await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(t).Build());
            }
        }
        public void Reconnect(List<string> topics)
        {
            mqttClient.UseDisconnectedHandler(async e =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                try
                {
                    await mqttClient.ConnectAsync(mainOptions, CancellationToken.None);
                    if (mqttClient.IsConnected)
                    {
                        foreach (var t in topics)
                        {
                            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(t).Build());
                        }
                    }
                }
                catch
                {
                }
            });
        }
        /*public async IAsyncEnumerable<string> Receive()
         {
             do
             {
                 await Task.Delay(100);
                 string mes = await Task.Run(() => ReceiveMesHandler(mqttClient));
                 yield return mes;
             }
             while (true);
         }*/
        public async Task<string> ReceiveAsync()
        {
            string t = null;
            do t = await Task.Run(() => ReceiveMesHandler(mqttClient));
            while (t == null);
            return t;
        }

        static string ReceiveMesHandler(MQTTnet.Client.IMqttClient mqttClient)
        {
            string mes = null;
            do
            {
                mqttClient.UseApplicationMessageReceivedHandler(e =>
                {
                    try
                    {
                        string topic = e.ApplicationMessage.Topic;

                        if (string.IsNullOrWhiteSpace(topic) == false)
                        {
                            mes = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                        }
                    }
                    catch (Exception ex)
                    {
                        mes = ex.Message;
                    }
                });
            } while (mes == null);
            return mes;
        }

        public void SaveAsync(string mes)
        {
            
            RealTime real = new RealTime();
            try
            {
                Signals recSign = JsonSerializer.Deserialize<Signals>(mes);
                //if (recSign.Type == "int") recSign.Type = "1";
                SignalsRepository.Add(recSign);
                real.Name = recSign.Name;
                real.Data = recSign.Data;
                real.Edizm = recSign.Edizm;
                real.Quality = recSign.Quality;
                real.Time = recSign.Time;
                string mess = recSign.Name;
            }
            catch
            {

            }
        }

        public async void PublisherAsync(string theme = "testing")
        {
            DateTime dt = DateTime.Now;

            Signals signals1 = new Signals() { Name = "PLK1", Data = "11", Time = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond), Quality = 111, Edizm = "Om", Type = 1 };
            Signals signals2 = new Signals() { Name = "PLK2", Data = "22", Time = dt, Quality = 222, Edizm = "K", Type = 1 };
            string json1 = JsonSerializer.Serialize<Signals>(signals1);
            string json2 = JsonSerializer.Serialize<Signals>(signals2);
            string[] json = new string[2] { json1, json2 };
            int i = 0;
            do
            {
                //await Task.Delay(TimeSpan.FromSeconds(1));
                {
                    var message = new MqttApplicationMessageBuilder()
                      .WithTopic(theme)
                      .WithPayload(json[i].ToString())
                      .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce)
                      .WithRetainFlag(true)
                      .Build();
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    if (mqttClient.IsConnected)
                    {
                        await mqttClient.PublishAsync(message, CancellationToken.None);
                        i++;
                    }
                }
            } while (i < 2);
        }
    }
}