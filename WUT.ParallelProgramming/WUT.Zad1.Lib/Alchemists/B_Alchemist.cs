using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUT.Zad1.Lib.Alchemists
{
    public class B_Alchemist : Alchemist
    {
        private static int ID_static = 1;
        private int Id;
        protected override ResourceTypes needResources => ResourceTypes.S_Hg;
        public B_Alchemist() : base()
        {
            Id = ID_static++;
        }
        public override string ToString()
        {
            return "B_Alchemist " + Id;
        }
        protected override void LogEndWork()
        {
            base.LogEndWork();
            StateLogger.FinishB();
        }
    }
}
