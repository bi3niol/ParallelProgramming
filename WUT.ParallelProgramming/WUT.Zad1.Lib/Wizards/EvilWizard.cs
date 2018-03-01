using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WUT.Zad1.Lib.Wizards
{
    public class EvilWizard : Wizard
    {
        private static int Id_static = 1;
        private int Id;
        public EvilWizard(Factory[] _factories, int minCharmTime, int interval) : base(minCharmTime, interval)
        {
            Id = Id_static++;
            factories = _factories;
        }
        protected override string Name => $"EvilWizard_{Id}";

        public override void Run()
        {
            while (Working)
            {
                Thread.Sleep(MinCharmTime + random.Next() % Interval);
                int facId = random.Next() % factories.Length;
                Console.WriteLine($"{Name} : casts a charm to {factories[facId].Name}");
                factories[facId].AddCharm();
                Console.WriteLine($"{Name} : Finished");
            }
        }
    }
}
