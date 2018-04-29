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

        internal void ProcessStartTour(StartTourMessage startTourMessage)
        {
            NeighborsInfo[startTourMessage.From].StartArrivedTour();
        }

        internal void ProcessFinishedTourMessage(FinishedTourMessage finishedTourMessage)
        {
            if (finishedTourMessage.HadConcert)
            {
                ProcessFinishedMessage(finishedTourMessage.From);
            }
            NeighborsInfo[finishedTourMessage.From].FinishedTour();
        }

        private SemaphoreSlim unSubscribeSem = new SemaphoreSlim(0, 1);
        internal void ProcessFinishedMessage(string From)
        {
            client.UnsubscribeAsync(From).ContinueWith(t=> { unSubscribeSem.Release(); });
            unSubscribeSem.Wait();
            NeighborsInfo[From].Status = Jankiel.ElectionStatus.Finished;
            NeighborsInfo[From].HadConcert = true;
            ////zabezpieczenie, na wypadek gdyby w "WaitForNeighborsStatus" rozpoczeło się czekanie na semaforze
            ////gdyby nie zostało zwolnienie semafora nastąpiłby deadlock
            //NeighborsInfo[finishedMessage.From].StatusRecived();
        }

        internal void WaitStartTour()
        {
            var neighborsToWaitFor = NeighborsInfo.Values.Where(n => !n.HadConcert).ToArray();
            foreach (var n in neighborsToWaitFor)
            {
                n.WaitStartTour();
            }
        }

        internal void ProcessVoteMessage(VoteMessage voteMessage)
        {
            NeighborsInfo[voteMessage.From].VoteMessageRecived(voteMessage.VoteValue);
        }

        public int FirstMISForLength { get; set; }
        public int SecondMISForLength { get; set; }

        internal void ProcessElectionStatusMessage(ElectionStatusMessage electionStatusMessage)
        {
            NeighborsInfo[electionStatusMessage.From].Status = electionStatusMessage.Status;
            NeighborsInfo[electionStatusMessage.From].StatusRecived();
        }

        internal void ProcessStartExMessage(StartExMessage startExMessage)
        {
            FirstMISForLength = (int)Math.Log(startExMessage.D);
            SecondMISForLength = startExMessage.M * (int)Math.Log(startExMessage.n);
            StartJankiel();
        }

        public Dictionary<string, NeighborInfo> NeighborsInfo = new Dictionary<string, NeighborInfo>();

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

        internal void WaitForNeighborsStatus()
        {
            var neighborsToWait = NeighborsInfo.Values.Where(n=>!n.HadConcert);
            foreach (var n in neighborsToWait)
                n.WaitStatus();
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
            //Console.WriteLine($"{jankiel.Name} : Otrzymał od  - {msg.From} - wiadomość - {msg.GetType()} -");
            lock (syncMessageProcessing)
            {
                msg.ProcessMessage(this);
            }
        }
    }
}
