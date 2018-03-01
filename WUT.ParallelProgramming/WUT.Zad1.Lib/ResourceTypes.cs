using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUT.Zad1.Lib
{
    [Flags]
    public enum ResourceTypes
    {
        Pb = 1,
        S = 2,
        Hg = 4,
        Pb_S = Pb | S,
        S_Pb = Pb | S,
        Pb_Hg = Pb | Hg,
        Hg_Pb = Pb | Hg,
        S_Hg = S | Hg,
        Hg_S = S | Hg,
        Pb_S_Hg = Pb_S | Hg,
        Pb_Hb_S = Pb_S | Hg,
        S_Pb_Hg = Pb_S | Hg,
        S_Hg_Pb = Pb_S | Hg,
        Hg_S_Pb = Pb_S | Hg,
        Hg_Pb_S = Pb_S | Hg 
    }
}
