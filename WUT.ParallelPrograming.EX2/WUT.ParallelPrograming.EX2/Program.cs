using CodeExMachina;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WUT.ParallelPrograming.EX2.Monitors;
using WUT.ParallelPrograming.EX2.Workers;

namespace WUT.ParallelPrograming.EX2
{
    class Program
    {
        static List<int> list = new List<int>();
        static object _lock = new object();
        static ConditionVariable[] conditions;
        static Random r = new Random();
        static Program()
        {
            conditions = new ConditionVariable[10];
            for (int i = 0; i < conditions.Length; i++)
            {
                conditions[i] = new ConditionVariable();
            }
        }
        private static void Add()
        {
            for (int i = 0; i < 10; i++)
            {
               // Console.WriteLine($"czeka na lock to pulls {i}");
                lock (_lock)
                {
                    //Console.WriteLine($"pulls {i}");
                    conditions[i].Pulse();
                    //Console.WriteLine($"po pulls {i}");
                    Thread.Sleep(100);
                }
                //Console.WriteLine($"koniec pentli dla {i}");

            }
        }
        private static void Read(object i)
        {
            int id = (int)i;
            Console.WriteLine($"{id} czeka na lock");
            lock (_lock)
            {
                Console.WriteLine($"{id} czeka na warunkowej");
                conditions[id].Wait(_lock);
                Console.WriteLine($"{id} koniec czekania XD");
            }
        }
        static void Main(string[] args)
        {
            //for (int i = 0; i < 1; i++)
            //{
            //    new Thread(new ThreadStart(Add)).Start();
            //}
            //Thread.Sleep(1100);
            //for (int i = 0; i < 10; i++)
            //{
            //    new Thread(new ParameterizedThreadStart(Read)).Start(i);
            //}

            Console.WriteLine((-10)%10);

            Start();

            Console.ReadLine();
            WaiterWorker.Working = false;
            Console.ReadLine();
        }

        private static void Start()
        {
            for (int i = 0; i < Settings.Default.KnightCount; i++)
            {
                new Thread(new ThreadStart(new KnightWorker(Settings.Default.SleepTime,Settings.Default.StoryTime,
                    Settings.Default.TimeVariation,i,i==Settings.Default.KingId).Work)).Start();
            }
            new Thread(new ThreadStart(new WaiterWorker(TableMonitor.Instance.FillWineButtle,"WINE waiter :",
                Settings.Default.WaiterTime,Settings.Default.TimeVariation).Work)).Start();

            new Thread(new ThreadStart(new WaiterWorker(TableMonitor.Instance.AddCucumbers, "CUCUMBER waiter :",
               Settings.Default.WaiterTime, Settings.Default.TimeVariation).Work)).Start();
        }
    }
}
