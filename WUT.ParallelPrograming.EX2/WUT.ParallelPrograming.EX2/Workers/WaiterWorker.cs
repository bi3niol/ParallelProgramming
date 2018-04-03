using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WUT.ParallelPrograming.EX2.Workers
{
    class WaiterWorker
    {
        private Random random = new Random(Guid.NewGuid().GetHashCode());
        private Action WorkAction = () => { };

        public static bool Working { get; set; } = true;
        private int TimeOut;
        private int TimeVariation;
        private string WaiterName;

        public WaiterWorker(Action work,string waitername,int timeout, int timevariation)
        {
            if (work != null)
                WorkAction = work;
            TimeVariation = timevariation;
            TimeOut = timeout;
            WaiterName = waitername;
        }

        public void Work()
        {
            while (Working)
            {
                Thread.Sleep(TimeOut + random.Next() % TimeVariation);
                //Console.WriteLine($"{WaiterName} work");
                WorkAction();
                //Console.WriteLine($"{WaiterName} finished");
            }
        }
    }
}
