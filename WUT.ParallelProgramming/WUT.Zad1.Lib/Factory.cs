using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WUT.Zad1.Lib
{
    public class Factory
    {
        private int MinProdTime, Interval, MaxSize;
        private int CurrSize = 0;
        private int charmsCount = 0;

        public string Name { get; private set; }

        private Semaphore sizeSem = new Semaphore(1, 1);
        private Semaphore charmsSem = new Semaphore(1, 1);
        private Semaphore productionSem;
        private Semaphore isOccupiedSem = new Semaphore(1, 1);

        private bool Running = true;
        private Random random = new Random(Guid.NewGuid().GetHashCode());

        public void Stop()
        {
            Running = false;
        }
        bool wasReleasedOccupied = true;
        public void AddCharm()
        {
            charmsSem.WaitOne();
            charmsCount++;
            // Console.WriteLine($"New Charm added to {Name} Factory");
            //Console.WriteLine(this);
            charmsSem.Release();
        }


        public void RemoveCharm()
        {
            charmsSem.WaitOne();
            var canRelease = charmsCount == 1;
            charmsCount = Math.Max(0, charmsCount - 1);
            if (!wasReleasedOccupied && canRelease)
            {
                wasReleasedOccupied = true;
                isOccupiedSem.Release();
            }
            //Console.WriteLine($"One Charm removed from {Name} Factory");
            //Console.WriteLine(this);
            charmsSem.Release();
        }

        public int GetProduct()
        {
            sizeSem.WaitOne();
            var res = CurrSize == 0 ? 0 : 1;
            if (CurrSize > 0)
            {
                CurrSize = CurrSize - 1;
                productionSem.Release();
            }
            sizeSem.Release();
            return res;
        }
        public bool HasProduct
        {
            get
            {
                bool res = CurrSize != 0;
                return res;
            }
        }

        private void AddProduct()
        {
            sizeSem.WaitOne();
            CurrSize++;
            //Console.WriteLine($"Resource has been produced by Factory {Name}");
            //Console.WriteLine(this);
            Storehouse.SignalNewResource();
            sizeSem.Release();
        }

        public Factory(string name, int minProdTime, int interval, int maxSize = 2)
        {
            productionSem = new Semaphore(maxSize, maxSize);
            Name = name;
            MinProdTime = minProdTime;
            Interval = interval;
            MaxSize = maxSize;
        }
        private void WaitIfOccupied()
        {
            charmsSem.WaitOne();
            if (charmsCount == 0 && !wasReleasedOccupied)
            {
                charmsSem.Release();
                return;
            }
            charmsSem.Release();
            StateLogger.DrawState($"Factory {Name} is OCCUPIED with {charmsCount} charms");
            isOccupiedSem.WaitOne();
            StateLogger.DrawState($"Factory {Name} CONTINUE work");
            wasReleasedOccupied = false;
        }
        public void Run()
        {
            Running = true;
            //Console.WriteLine($"{Name} Started");
            StateLogger.DrawState($"{Name} Started");
            while (Running)
            {
                productionSem.WaitOne();
                WaitIfOccupied();

                //Console.WriteLine($"Factory {Name} is producing new resource");
                StateLogger.DrawState($"Factory {Name} is producing new resource");
                Thread.Sleep(MinProdTime + random.Next() % Interval);
                AddProduct();
                StateLogger.DrawState($"Factory {Name} is produced new resource");
                Thread.Sleep(0);
            }
            //Console.WriteLine($"{Name} End Work");
            StateLogger.DrawState($"{Name} End Work");
        }
        public override string ToString()
        {
            return $"{Name} : has {CurrSize} resources, is occupied by {charmsCount} charms";
        }
    }
}
