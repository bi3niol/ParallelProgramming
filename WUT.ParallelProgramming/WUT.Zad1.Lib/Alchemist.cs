using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WUT.Zad1.Lib
{
    public abstract class Alchemist
    {
        protected abstract ResourceTypes needResources {  get; }
        private Semaphore getResourceSem;
        public Alchemist()
        {
            getResourceSem = new Semaphore(0, 1);
        }

        public void Run()
        {

        }
    }
}
