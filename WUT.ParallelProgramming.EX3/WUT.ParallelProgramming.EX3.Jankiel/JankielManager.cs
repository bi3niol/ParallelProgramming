using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WUT.ParallelProgramming.EX3.Jankiel.Messages;

namespace WUT.ParallelProgramming.EX3.Jankiel
{
    public class JankielManager
    {
        private IMqttClient client;
        private Jankiel jankiel;
        public JankielManager(Jankiel j)
        {
            jankiel = j;
        }

        public int FirstMISForLength { get; set; }
        public int SecondMISForLength { get; set; }
        public Dictionary<string, NeighborInfo> NeighborsInfo = new Dictionary<string, NeighborInfo>();

        private bool started = false;
        private object startLock = new object();
        private Semaphore WaitForStartSem = new Semaphore(0, 1);
        public void StartJankiel()
        {
            lock (startLock)
                if (!started)
                {
                    started = true;
                    WaitForStartSem.Release();
                }
        }

        internal void WaitForStart()
        {
            WaitForStartSem.WaitOne();
        }

        public async Task SendMsg(Message message)
        {
            var mqMsg = new MqttApplicationMessageBuilder()
                .WithTopic(jankiel.Name)
                .WithPayload(message.GetBytes())
                .Build();

            await client.PublishAsync(mqMsg);
        }

        public void SetUpConnection(string name, string[] neighbors, string master)
        {
            client = new MqttFactory().CreateMqttClient();

            var options = new MqttClientOptionsBuilder().WithTcpServer(Settings.Default.HostName, Settings.Default.PortNum).Build();

            client.ApplicationMessageReceived += Client_ApplicationMessageReceived;
            client.Connected += async (s, e) =>
            {
                await client.SubscribeAsync(new TopicFilterBuilder().WithTopic(master).Build());
                Console.WriteLine($"{name} sub {master}");
                foreach (var item in neighbors)
                {
                    NeighborsInfo.Add(item, new NeighborInfo(item));
                    await client.SubscribeAsync(new TopicFilterBuilder().WithTopic(item).Build());
                    Console.WriteLine($"{name} sub {item}");
                }
            };
            client.ConnectAsync(options);
        }

        internal Task WaitForNeighborsStatus()
        {
            throw new NotImplementedException();
        }

        internal Task<bool> RecivedB()
        {
            throw new NotImplementedException();
        }

        private void Client_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            var msg = Message.GetMessage(e.ApplicationMessage.Payload);
            Console.WriteLine($"{jankiel.Name} : Otrzymał od  - {msg.From} - wiadomość - {msg.GetType()} -");
            msg.ProcessMessage(this);
        }
    }
}
