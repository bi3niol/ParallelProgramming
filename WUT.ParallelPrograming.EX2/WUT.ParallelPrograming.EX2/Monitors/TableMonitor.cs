using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WUT.ParallelPrograming.EX2.Monitors
{
    class TableMonitor
    {
        private static TableMonitor _instane;
        public static TableMonitor Instance
        {
            get
            {
                if (_instane == null)
                    _instane = new TableMonitor(Settings.Default.KnightCount, Settings.Default.MaxCucumbersOnPlate, Settings.Default.WineButtleCapacity);
                return _instane;
            }
        }

        private Random random = new Random();

        //mappers
        private Dictionary<int, int> IdToCucumbers = new Dictionary<int, int>();
        private Dictionary<int, int> IdToGolbets = new Dictionary<int, int>();


        private CodeExMachina.ConditionVariable[] WaitSpeackingConditions;
        private KnightSpeakStates[] knightSpeakStates;

        private bool IsKingSpeaking = false;
        private object KingSpeakingLock = new object();
        private CodeExMachina.ConditionVariable KingSpeakingCondition = new CodeExMachina.ConditionVariable();
        private int KingId = 0;

        private int WineButtle;
        private int[] CucumberPlates;
        private bool[] GolbetInUse;
        private bool[] PlateInUse;
        private object WineButtleLock = new object();
        private CodeExMachina.ConditionVariable WineBottleCondition = new CodeExMachina.ConditionVariable();

        private KnightDrinkStates[] knightDrinkStates;
        private CodeExMachina.ConditionVariable[] WaitDrinkingConditions;

        private int KnightsCount;
        private int WineButtleCapacity;
        private int MaxCucumbersOnPlate;
        private TableMonitor(int knights, int c, int w, int kingId = 0)
        {
            KingId = kingId;
            KnightsCount = knights;
            MaxCucumbersOnPlate = c;
            WineButtle = w;
            WineButtleCapacity = w;
            knightDrinkStates = new KnightDrinkStates[knights];
            knightSpeakStates = new KnightSpeakStates[knights];
            CucumberPlates = new int[knights / 2];
            GolbetInUse = new bool[knights / 2];
            PlateInUse = new bool[knights / 2];
            for (int i = 0; i < CucumberPlates.Length; i++)
            {
                CucumberPlates[i] = c;
            }
            WaitSpeackingConditions = new CodeExMachina.ConditionVariable[knights];
            WaitDrinkingConditions = new CodeExMachina.ConditionVariable[knights];
            for (int i = 0; i < WaitSpeackingConditions.Length; i++)
            {
                WaitSpeackingConditions[i] = new CodeExMachina.ConditionVariable();
                WaitDrinkingConditions[i] = new CodeExMachina.ConditionVariable();

                IdToCucumbers.Add(i, MapIdToCucumber(i));
                IdToGolbets.Add(i, MapIdToGolbet(i));
            }
        }

        public void FillWineButtle()
        {
            lock (WineButtleLock)
            {
                WineButtle = WineButtleCapacity;
                WineBottleCondition.PulseAll();
            }
            DrawState();
        }

        public void AddCucumbers()
        {
            lock (knightDrinkStates)
            {
                if (random.Next() % 2 == 0)
                    for (int i = 0; i < WaitDrinkingConditions.Length; i++)
                    {
                        if (CucumberPlates[IdToCucumbers[i]] == 0)
                            WaitDrinkingConditions[i].Pulse();
                    }
                else
                    for (int i = WaitDrinkingConditions.Length - 1; i >= 0; i--)
                        if (CucumberPlates[IdToCucumbers[i]] == 0)
                            WaitDrinkingConditions[i].Pulse();
                for (int i = 0; i < CucumberPlates.Length; i++)
                    CucumberPlates[i] = MaxCucumbersOnPlate;
            }
            DrawState();
        }

        public void StartDrink(int id)
        {
            lock (knightDrinkStates)
            {
                while (!CanTakeItems(id))
                {
                    knightDrinkStates[id] = KnightDrinkStates.WaitDrink;
                    WaitDrinkingConditions[id].Wait(knightDrinkStates);
                }
                CucumberPlates[IdToGolbets[id]]--;
                GolbetInUse[IdToGolbets[id]] = true;
                PlateInUse[IdToCucumbers[id]] = true;
            }
            lock (WineButtleLock)
            {
                while (WineButtle == 0)
                    WineBottleCondition.Wait(WineButtleLock);
                WineButtle--;
            }
            knightDrinkStates[id] = KnightDrinkStates.Drinking;
            DrawState();
        }

        public void StopDrink(int id)
        {
            lock (knightDrinkStates)
            {
                GolbetInUse[IdToGolbets[id]] = false;
                PlateInUse[IdToCucumbers[id]] = false;

                knightDrinkStates[id] = KnightDrinkStates.Free;
                if (random.Next() % 2 == 0)
                {
                    WaitDrinkingConditions[Modulo(id - 1, KnightsCount)].Pulse();
                    WaitDrinkingConditions[Modulo(id + 1, KnightsCount)].Pulse();
                }
                else
                {
                    WaitDrinkingConditions[Modulo(id + 1, KnightsCount)].Pulse();
                    WaitDrinkingConditions[Modulo(id - 1, KnightsCount)].Pulse();
                }
            }
            DrawState();
        }

        private bool CanTakeItems(int id)
        {
            return CucumberPlates[IdToCucumbers[id]] > 0 && !GolbetInUse[IdToGolbets[id]] && !PlateInUse[IdToCucumbers[id]];
        }


        private int MapIdToCucumber(int id)
        {
            return ((id + 1) / 2) % (KnightsCount / 2);
        }
        private int MapIdToGolbet(int id)
        {
            return id / 2;
        }

        public void StartSpeak(int id)
        {
            lock (knightSpeakStates)
            {
                while (knightSpeakStates[Modulo(id - 1, KnightsCount)] == KnightSpeakStates.Speaking || knightSpeakStates[Modulo(id + 1, KnightsCount)] == KnightSpeakStates.Speaking)
                {
                    knightSpeakStates[id] = KnightSpeakStates.WaitSpeaking;
                    WaitSpeackingConditions[id].Wait(knightSpeakStates);
                }
                knightSpeakStates[id] = KnightSpeakStates.Speaking;
                if (id == KingId)
                {
                    IsKingSpeaking = false;
                }
                else
                {
                    while (IsKingSpeaking)
                        KingSpeakingCondition.Wait(knightSpeakStates);
                }
            }
            DrawState();
        }
        public void StopSpeak(int id)
        {
            lock (knightSpeakStates)
            {
                knightSpeakStates[id] = KnightSpeakStates.Free;

                if (knightSpeakStates[Modulo(id - 1, KnightsCount)] == KnightSpeakStates.WaitSpeaking && knightSpeakStates[Modulo(id - 2, KnightsCount)] != KnightSpeakStates.Speaking)
                    WaitSpeackingConditions[Modulo(id - 1, KnightsCount)].Pulse();
                if (knightSpeakStates[Modulo(id + 1, KnightsCount)] == KnightSpeakStates.WaitSpeaking && knightSpeakStates[Modulo(id + 2, KnightsCount)] != KnightSpeakStates.Speaking)
                    WaitSpeackingConditions[Modulo(id + 1, KnightsCount)].Pulse();

                if (id == KingId && IsKingSpeaking)
                {
                    IsKingSpeaking = false;
                    KingSpeakingCondition.PulseAll();
                }
            }
            DrawState();
        }

        private static int Modulo(int n, int m)
        {
            int res = n % m;
            res = res < 0 ? m + res : res;
            return res;
        }

        private const string propNameFormat = "{0,12}|";
        private const string valueFormat = "|{0,15}|";
        private void DrawState()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(valueFormat, "Knight ID");
            stringBuilder.AppendFormat(valueFormat, " Story State");
            stringBuilder.AppendFormat(valueFormat, "Drink State");
            stringBuilder.AppendFormat(valueFormat, "CUCUM.. plates");
            stringBuilder.Append($" Wine state  {WineButtle}/{WineButtleCapacity}");
            stringBuilder.AppendLine();
            for (int i = 0; i < KnightsCount; i++)
            {
                stringBuilder.AppendFormat(valueFormat, $"     {i}");
                stringBuilder.AppendFormat(valueFormat, $"{knightSpeakStates[i]}");
                stringBuilder.AppendFormat(valueFormat, $"{knightDrinkStates[i]}");
                stringBuilder.AppendFormat(valueFormat, $"{CucumberPlates[IdToCucumbers[i]]}");
                stringBuilder.AppendLine();
            }

            lock (Console.Out)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine(stringBuilder.ToString());
            }
        }

        enum KnightSpeakStates
        {
            Free = 0,
            WaitSpeaking = 1,
            Speaking = 2,
        }

        enum KnightDrinkStates
        {
            Free = 0,
            WaitDrink = 1,
            Drinking = 2
        }
    }
}
