using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WUT.Zad1.Lib
{
    public static class StateLogger
    {
        private static List<Factory> factories = new List<Factory>();
        private static Semaphore semaphore = new Semaphore(1, 1);
        private static Semaphore facSem = new Semaphore(1, 1);
        private static Semaphore fA = new Semaphore(1, 1);
        private static Semaphore fB = new Semaphore(1, 1);
        private static Semaphore fC = new Semaphore(1, 1);
        private static Semaphore fD = new Semaphore(1, 1);
        private static string line = new string('=', 61);
        private static string prop_valFormat = "|{0,28}|{1,28}|\n";
        public static int SpawnedA = 0;
        public static int SpawnedB = 0;
        public static int SpawnedC = 0;
        public static int SpawnedD = 0;
        private static int finishedA = 0;
        private static int finishedB = 0;
        private static int finishedC = 0;
        private static int finishedD = 0;

        public static void SetFactories(Factory[] _factories)
        {
            facSem.WaitOne();
            factories = _factories.ToList();
            facSem.Release();
        }
        public static void FinishA()
        {
            fA.WaitOne();
            finishedA++;
            fA.Release();
        }
        public static void FinishB()
        {
            fB.WaitOne();
            finishedB++;
            fB.Release();
        }
        public static void FinishC()
        {
            fC.WaitOne();
            finishedC++;
            fC.Release();
        }
        public static void FinishD()
        {
            fD.WaitOne();
            finishedD++;
            fD.Release();
        }
        public static void DrawState(string msg = "")
        {
            semaphore.WaitOne();
            var sb = new StringBuilder();
            sb.AppendLine(line);
            facSem.WaitOne();
            foreach (var item in factories)
            {
                sb.AppendLine(item.ToString());
            }
            facSem.Release();
            sb.AppendLine(line);
            sb.AppendFormat(prop_valFormat, "Spawned Alchemics A", "alchemics how gets Resources");
            sb.AppendFormat(prop_valFormat, SpawnedA, finishedA);
            sb.AppendFormat(prop_valFormat, "Spawned Alchemics B", "");
            sb.AppendFormat(prop_valFormat, SpawnedB, finishedB);
            sb.AppendFormat(prop_valFormat, "Spawned Alchemics C", "");
            sb.AppendFormat(prop_valFormat, SpawnedC, finishedC);
            sb.AppendFormat(prop_valFormat, "Spawned Alchemics D", "");
            sb.AppendFormat(prop_valFormat, SpawnedD, finishedD);
            sb.AppendLine(line);
            sb.AppendFormat(prop_valFormat, "Alchemics who waiting",Storehouse.WaitingCount);
            sb.AppendLine(msg);
            sb.AppendLine(line);
            Console.Clear();
            Console.Write(sb.ToString());
            semaphore.Release();
        }
    }
}
