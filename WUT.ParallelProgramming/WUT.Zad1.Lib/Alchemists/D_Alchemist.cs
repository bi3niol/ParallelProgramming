using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUT.Zad1.Lib.Alchemists
{
    public class D_Alchemist : Alchemist
    {
        private static int ID_static = 1;
        private int Id;
        protected override ResourceTypes needResources => ResourceTypes.Pb_Hb_S;
        public D_Alchemist() : base()
        {
            Id = ID_static++;
        }
        public override string ToString()
        {
            return "D_Alchemist " + Id;
        }
        protected override void LogEndWork()
        {
            base.LogEndWork();
            StateLogger.FinishD();
        }
    }
}
