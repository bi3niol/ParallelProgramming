using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUT.Zad1.Lib
{
    public abstract class Wizard
    {
        protected virtual string Name { get; }
        protected Factory[] factories { get; set; }
        protected Random random = new Random(Guid.NewGuid().GetHashCode());
        protected bool Working = true;

        protected int MinCharmTime { get; }
        protected int Interval { get; }

        public Wizard(int minCharmTime, int interval)
        {
            MinCharmTime = minCharmTime;
            Interval = interval;
        }
        public abstract void Run();
    }
}
