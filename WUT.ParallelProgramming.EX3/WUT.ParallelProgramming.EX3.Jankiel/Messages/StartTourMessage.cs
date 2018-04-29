using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUT.ParallelProgramming.EX3.Jankiel.Messages
{
    [Serializable]
    public class StartTourMessage : Message
    {
        public StartTourMessage(string from) : base(from)
        {
        }

        public override void ProcessMessage(JankielManager jankiel)
        {
            jankiel.ProcessStartTour(this);
        }
    }
}
