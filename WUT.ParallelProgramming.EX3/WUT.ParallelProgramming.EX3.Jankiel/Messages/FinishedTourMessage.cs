﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUT.ParallelProgramming.EX3.Jankiel.Messages
{
    [Serializable]
    public class FinishedTourMessage : Message
    {
        public bool HadConcert { get; set; }
        public FinishedTourMessage(string from,bool hadConcert) : base(from)
        {
            HadConcert = hadConcert;
        }

        public override void ProcessMessage(JankielManager jankiel)
        {
            jankiel.ProcessFinishedTourMessage(this);
        }
    }
}
