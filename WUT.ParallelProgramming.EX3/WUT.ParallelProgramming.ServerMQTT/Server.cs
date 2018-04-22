using MQTTnet;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUT.ParallelProgramming.ServerMQTT
{
    public class Server
    {
        private IMqttServer mqttServer;
        private MqttServerOptionsBuilder optionsBuilder;
        public Server()
        {
            mqttServer = new MqttFactory().CreateMqttServer();
            optionsBuilder = new MqttServerOptionsBuilder();
            optionsBuilder = optionsBuilder
                .WithConnectionBacklog(100)
                .WithDefaultEndpointPort(1884);
        }
        public async void Start()
        {
            await mqttServer.StartAsync(optionsBuilder.Build());
        }

        public async void Stop()
        {
            await mqttServer.StopAsync();
        }
    }
}
