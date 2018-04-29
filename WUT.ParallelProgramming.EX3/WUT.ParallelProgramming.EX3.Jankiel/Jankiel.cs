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
    public class Jankiel
    {
        internal JankielManager jankielManager { get; }

        public string Name { get; private set; }
        private string MasterName;
        private Random random;
        public Jankiel(string name, string[] neighbors, string master = "Ex3Master")
        {
            Name = name;
            MasterName = master;
            random = new Random(name.GetHashCode());
            jankielManager = new JankielManager(this);
            jankielManager.SetUpConnection(name, neighbors, master);
        }
        public async void Run()
        {
            Console.WriteLine($"Jankiel {Name} Czeka na start ...");
            jankielManager.WaitForStart();
            ElectionStatus status = ElectionStatus.None;
            while (status != ElectionStatus.Selected)
            {
                await jankielManager.SendMsg(new StartTourMessage(Name));
                jankielManager.WaitStartTour();
                Console.WriteLine($"Start rundy ");
                status = await MIS();
                if (status == ElectionStatus.Selected)
                {
                    Console.WriteLine($"Jankiel {Name} gra koncert !");
                    Thread.Sleep(2000);
                }

                await jankielManager.SendMsg(new FinishedTourMessage(Name, status == ElectionStatus.Selected));
                jankielManager.WaitFinishTour();
                Console.WriteLine($"koniec rundy ");
            }
            Console.WriteLine($"Jankiel {Name} skonczył {status} ...");
        }


        private async Task<ElectionStatus> MIS()
        {
            byte v;
            ElectionStatus status = ElectionStatus.None;
            for (int i = 0; i < jankielManager.FirstMISForLength; i++)
            {
                double choosePropability = 1.0 / Math.Pow(2, jankielManager.FirstMISForLength - i);
                for (int j = 0; j < jankielManager.SecondMISForLength; j++)
                {
                    v = 0;
                    if (random.NextDouble() < choosePropability)
                        v = 1;
                    if (status == ElectionStatus.Selected)
                        v = 1;
                    if (status == ElectionStatus.Lose)
                        v = 0;
                    await jankielManager.SendMsg(new VoteMessage(Name, v));
                    bool isBRecived = jankielManager.RecivedB();
                    if (isBRecived)
                        v = 0;

                    if (v == 1)
                        status = ElectionStatus.Selected;
                    await jankielManager.SendMsg(new VoteMessage(Name, v));
                    isBRecived = jankielManager.RecivedB();
                    if (v == 0 && isBRecived)
                        status = ElectionStatus.Lose;
                    await jankielManager.SendMsg(new ElectionStatusMessage(status, Name));
                    jankielManager.WaitForNeighborsStatus();
                }
            }
            return status;
        }

        public enum ElectionStatus
        {
            Lose = 0,
            Selected = 1,
            None = 2,
            Finished = 3
        }
    }
}
