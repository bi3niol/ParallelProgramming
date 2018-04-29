using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WUT.ParallelProgramming.EX3.Jankiel
{
    public class NeighborInfo
    {
        public string NeighboarName { get; set; }
        public bool HadConcert { get; set; }
        public Jankiel.ElectionStatus Status { get; set; }
        public byte LastVote { get; set; }
        private SemaphoreSlim waitStatusSem = new SemaphoreSlim(0, int.MaxValue);
        private SemaphoreSlim waitVoteSem = new SemaphoreSlim(0, int.MaxValue);
        private SemaphoreSlim waitFinishedTourSem = new SemaphoreSlim(0, int.MaxValue);

        public void StatusRecived()
        {
            waitStatusSem.Release();
        }

        internal void FinishedTour()
        {
            waitFinishedTourSem.Release();
        }

        public NeighborInfo(string name)
        {
            NeighboarName = name;
            Status = Jankiel.ElectionStatus.None;
        }

        internal void WaitStatus()
        {
            waitStatusSem.Wait();
        }

        internal void VoteMessageRecived(byte Vote)
        {
            LastVote = Vote;
            waitVoteSem.Release();
        }
        internal bool SentB()
        {
            waitVoteSem.Wait();
            return LastVote == 1;
        }

        internal void WaitFinishedTour()
        {
            waitFinishedTourSem.Wait();
        }

        internal void ResetDataAfterTour()
        {
            LastVote = 0;
            Status = Jankiel.ElectionStatus.None;
        }
    }
}
