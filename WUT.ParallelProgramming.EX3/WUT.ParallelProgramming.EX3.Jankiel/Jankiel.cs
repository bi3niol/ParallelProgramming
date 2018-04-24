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
        public bool NotEnd { get; set; } = false;
        public void Run()
        {
            Console.WriteLine($"Jankiel {Name} Czeka na start ...");
            jankielManager.WaitForStart();
            while (NotEnd)
            {

            }
            Console.WriteLine($"Jankiel {Name} skonczył ...");
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
                    {
                        v = 1;
                        await jankielManager.SendMsg(new VoteMessage(Name, 1));
                    }
                    else
                        await jankielManager.SendMsg(new VoteMessage(Name, 0));

                    bool isBRecived = await jankielManager.RecivedB();
                    if (isBRecived)
                        v = 0;

                    if (v == 1)
                        status = ElectionStatus.Selected;
                    else
                    {
                        if (isBRecived)
                            status = ElectionStatus.Lose;
                    }
                    await jankielManager.SendMsg(null);

                    await jankielManager.WaitForNeighborsStatus();
                }
            }
            return status;
        }

        public enum ElectionStatus
        {
            Lose = 0,
            Selected = 1,
            None = 2
        }
    }
}
