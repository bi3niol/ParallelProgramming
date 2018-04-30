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
        public int FirstMISForLength { get; set; }
        public int SecondMISForLength { get; set; }
        public Dictionary<string, NeighborInfo> NeighborsInfo = new Dictionary<string, NeighborInfo>();

        public JankielManager(Jankiel j)
        {
            jankiel = j;
        }

        internal void ProcessStartTour(StartTourMessage startTourMessage)
        {
            NeighborsInfo[startTourMessage.From].StartTourArrived();
        }

        internal void ProcessFinishedTourMessage(FinishedTourMessage finishedTourMessage)
        {
            if (finishedTourMessage.HadConcert)
            {
                ProcessFinishedMessage(finishedTourMessage.From);
            }
            NeighborsInfo[finishedTourMessage.From].FinishedTour();
        }

        internal void ProcessFinishedMessage(string From)
        {
            SemaphoreSlim unSubscribeSem = new SemaphoreSlim(0, 1);
            client.UnsubscribeAsync(From).ContinueWith(t => { unSubscribeSem.Release(); });
            unSubscribeSem.Wait();
            NeighborsInfo[From].Status = Jankiel.ElectionStatus.Finished;
            NeighborsInfo[From].HadConcert = true;
        }

        internal void ProcessVoteMessage(VoteMessage voteMessage)
        {
            NeighborsInfo[voteMessage.From].VoteMessageRecived(voteMessage.VoteValue);
        }

        internal void ProcessStartExMessage(StartExMessage startExMessage)
        {
            FirstMISForLength = (int)Math.Log(startExMessage.D);
            SecondMISForLength = startExMessage.M * (int)Math.Log(startExMessage.n);
            StartJankiel();
        }
        internal void WaitStartTour()
        {
            var neighborsToWaitFor = NeighborsInfo.Values.Where(n => !n.HadConcert).ToArray();
            foreach (var n in neighborsToWaitFor)
            {
                n.WaitStartTour();
            }
        }

        private bool started = false;
        private Semaphore WaitForStartSem = new Semaphore(0, 1);
        public void StartJankiel()
        {
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

        internal void WaitFinishTour()
        {
            var neighborsToWaitFor = NeighborsInfo.Values.Where(n => !n.HadConcert).ToArray();
            foreach (var n in neighborsToWaitFor)
            {
                n.WaitFinishedTour();
                n.ResetDataAfterTour();
            }
        }

        /// <summary>
        /// ustawianie odpowiednich połączeni pomiedzy sąsiadami
        /// </summary>
        /// <param name="name">Nazwa cymbalisty</param>
        /// <param name="neighbors">lista sąsiadów (w odległosci <=3m)</param>
        /// <param name="master">Nazwa "mastera" wykorzystywane w celu czekania na jego informacje która powiadomi
        /// wszystki o tym ze wszystkie połączenia zostały ustawione (wiadomosci typu "StartExMessage") 
        /// </param>
        public void SetUpConnection(string name, string[] neighbors, string master)
        {
            client = new MqttFactory().CreateMqttClient();

            var options = new MqttClientOptionsBuilder().WithTcpServer(Settings.Default.HostName, Settings.Default.PortNum).Build();

            client.ApplicationMessageReceived += Client_ApplicationMessageReceived;
            client.Connected += async (s, e) =>
            {
                await client.SubscribeAsync(new TopicFilterBuilder().WithTopic(master).Build());
                Console.WriteLine($"{name} subscribe {master}");
                foreach (var item in neighbors)
                {
                    NeighborsInfo.Add(item, new NeighborInfo(item));
                    await client.SubscribeAsync(new TopicFilterBuilder().WithTopic(item).Build());
                    Console.WriteLine($"{name} subscribe {item}");
                }
            };
            client.ConnectAsync(options);
        }

        internal bool RecivedB()
        {
            var neighborsToWait = NeighborsInfo.Values.Where(n => !n.HadConcert);
            bool res = false;
            foreach (var n in neighborsToWait)
            {
                if (n.SentB())
                    res = true;
            }
            return res;
        }

        private object syncMessageProcessing = new object();
        private void Client_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            var msg = Message.GetMessage(e.ApplicationMessage.Payload);
            //Console.WriteLine($"{jankiel.Name} : Otrzymał od  - {msg.From} - wiadomość - {msg} -");
            //lock - by przetwarzac jedną wiadomosc w jednym czasie
            lock (syncMessageProcessing)
            {
                msg.ProcessMessage(this);
            }
        }
    }
}
