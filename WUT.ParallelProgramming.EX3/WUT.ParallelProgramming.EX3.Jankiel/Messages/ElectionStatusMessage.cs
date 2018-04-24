using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUT.ParallelProgramming.EX3.Jankiel.Messages
{
    [Serializable]
    public class ElectionStatusMessage : Message
    {
        public Jankiel.ElectionStatus Status { get; }
        public ElectionStatusMessage(Jankiel.ElectionStatus status, string from) : base(from)
        {
        }

        public override void ProcessMessage(JankielManager jankiel)
        {
            throw new NotImplementedException();
        }
    }
}
