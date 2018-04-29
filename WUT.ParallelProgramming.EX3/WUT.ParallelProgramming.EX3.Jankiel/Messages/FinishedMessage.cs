﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUT.ParallelProgramming.EX3.Jankiel.Messages
{
    [Serializable]
    public class FinishedMessage : Message
    {
        public FinishedMessage(string from) : base(from)
        {
        }

        public override void ProcessMessage(JankielManager jankiel)
        {
            //jankiel.ProcessFinishedMessage(this);
        }
    }
}
