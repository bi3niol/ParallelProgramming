using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUT.ParallelProgramming.EX3.Jankiel.Messages
{
    [Serializable]
    public class StartExMessage : Message
    {
        public int D { get; private set; }
        public int M { get; private set; }
        public int n { get; private set; }
        public StartExMessage(int d, int m, int n) : this(d, m, n, "Master")
        { }
        public StartExMessage(int d, int m, int n, string from) : base(from)
        {
            D = d; M = m; this.n = n;
        }
        public override void ProcessMessage(JankielManager jankielManager)
        {
            jankielManager.ProcessStartExMessage(this);
        }
    }
}
