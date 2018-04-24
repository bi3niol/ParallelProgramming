using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUT.ParallelProgramming.EX3.Jankiel.Messages
{
    [Serializable]
    public class VoteMessage : Message
    {
        public byte VoteValue { get; protected set; }
        public VoteMessage(string from, byte voteValue) : base(from)
        {
            VoteValue = voteValue;
        }

        public override void ProcessMessage(JankielManager jankiel)
        {
            throw new NotImplementedException();
        }
    }
}
