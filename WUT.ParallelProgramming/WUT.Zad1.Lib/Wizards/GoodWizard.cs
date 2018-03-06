using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WUT.Zad1.Lib.Wizards
{
    public class GoodWizard : Wizard
    {
        private static int Id_static = 1;
        private int Id;
        public GoodWizard(Factory[] _factories ,int minCharmTime, int interval) : base(minCharmTime, interval)
        {
            Id = Id_static++;
            factories = _factories;
        }

        protected override string Name => $"GoodWizard_{Id}";
        public override void Run()
        {
            while (Working)
            {
                Thread.Sleep(MinCharmTime + random.Next() % Interval);
                //Console.WriteLine($"{Name} : start taking off the charms");
                foreach (var fac in factories)
                {
                    fac.RemoveCharm();
                }
                //Console.WriteLine($"{Name} : Finished");
                StateLogger.DrawState();

            }
        }
    }
}
