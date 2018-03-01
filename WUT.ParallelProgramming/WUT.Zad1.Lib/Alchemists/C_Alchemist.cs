using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUT.Zad1.Lib.Alchemists
{
    public class C_Alchemist : Alchemist
    {
        private static int ID_static = 1;
        private int Id;
        protected override ResourceTypes needResources => ResourceTypes.Pb_S;
        public C_Alchemist() : base()
        {
            Id = ID_static++;
        }
        public override string ToString()
        {
            return "C_Alchemist " + Id;
        }
    }
}
