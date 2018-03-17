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
        private static int MaxMessagesCount = LibSettings.Default.LoggedMsgCount;
        private static List<Factory> factories = new List<Factory>();
        private static List<String> messages = new List<string>();
        private static SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);
        private static SemaphoreSlim msgSem = new SemaphoreSlim(1, 1);
        private static SemaphoreSlim facSem = new SemaphoreSlim(1, 1);
        private static SemaphoreSlim fA = new SemaphoreSlim(1, 1);
        private static SemaphoreSlim fB = new SemaphoreSlim(1, 1);
        private static SemaphoreSlim fC = new SemaphoreSlim(1, 1);
        private static SemaphoreSlim fD = new SemaphoreSlim(1, 1);
        private static string line = new string('=', 61);
        private static string prop_valFormat = "|{0,28}|{1,28}|\n";
        private static string clearLine = new string(' ', 70) + "\r";
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
            facSem.Wait();
            factories = _factories.ToList();
            facSem.Release();
        }
        public static void FinishA()
        {
            fA.Wait();
            finishedA++;
            fA.Release();
        }
        public static void FinishB()
        {
            fB.Wait();
            finishedB++;
            fB.Release();
        }
        public static void FinishC()
        {
            fC.Wait();
            finishedC++;
            fC.Release();
        }
        public static void FinishD()
        {
            fD.Wait();
            finishedD++;
            fD.Release();
        }
        private static void AddMessage(string msg)
        {
            if (string.IsNullOrEmpty(msg))
                return;
            msgSem.Wait();
            if (messages.Count == MaxMessagesCount)
                messages.RemoveAt(0);
            messages.Add(msg);
            msgSem.Release();
        }
        public static void DrawState(string msg = "")
        {
            var sb = new StringBuilder();
            AddMessage(msg);
            sb.AppendLine(line);
            //facSem.Wait();
            foreach (var item in factories)
            {
                sb.AppendLine(item.ToString());
            }
            //facSem.Release();
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
            sb.AppendFormat(prop_valFormat, "Alchemics who are waitting", Storehouse.WaitingCount);
            sb.AppendLine(line);
            msgSem.Wait();
            foreach (var item in messages)
            {
                sb.Append(clearLine);
                sb.Append(item + "\n");
            }
            msgSem.Release();
            sb.AppendLine(line);
            SemaphoreSlim.Wait();
            Console.SetCursorPosition(0, 1);
            Console.Write(sb.ToString());
            Console.SetCursorPosition(0, 1);
            SemaphoreSlim.Release();
        }
    }
}
