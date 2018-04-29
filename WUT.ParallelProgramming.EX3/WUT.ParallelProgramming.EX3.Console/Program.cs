using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WUT.ParallelProgramming.EX3.Jankiel;
using WUT.ParallelProgramming.EX3.Jankiel.Messages;
using WUT.ParallelProgramming.ServerMQTT;

namespace WUT.ParallelProgramming.EX3.Console
{
    class Program
    {
        private static Server server = new Server();
        static void Main(string[] args)
        {
            server.Start();

            var positions = Helpers.GetPositions(args[0]);
            int D = 0;
            for (int i = 0; i < positions.GetLength(0); i++)
            {
                var name = Helpers.GetJankielName(positions[i]);
                var neighbors = Helpers.GetNeighborsForI(positions, i, Settings.Default.HearDistance);
                if (D < neighbors.Length)
                    D = neighbors.Length;

                new Thread(new ThreadStart(new Jankiel.Jankiel(name, neighbors).Run)).Start();
            }
            var Master = new MqttFactory().CreateMqttClient();
            Master.Connected += (s, e) =>
            {
                System.Console.WriteLine("Master Connected");
            };
            Master.ConnectAsync(new MqttClientOptionsBuilder().WithTcpServer("localhost", 1884).Build());

            var msg = new StartExMessage(D*2, 5, positions.GetLength(0)).GetBytes();
            var mqMsg = new MqttApplicationMessageBuilder().WithTopic("Ex3Master").WithPayload(msg).Build();
            System.Console.WriteLine("enter by wysłac wiadomosc od mastera");
            System.Console.ReadLine();

            Master.PublishAsync(mqMsg);
            System.Console.WriteLine("enter by zakonczyc");
            System.Console.ReadLine();
            server.Stop();
        }
    }
}
