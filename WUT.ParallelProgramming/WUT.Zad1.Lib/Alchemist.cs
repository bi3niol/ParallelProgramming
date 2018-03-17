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
        private SemaphoreSlim getResourceSem;
        public Alchemist()
        {
            getResourceSem = new SemaphoreSlim(0, 1);
        }
        protected virtual void LogEndWork() { }
        public void Run()
        {
            //Console.WriteLine($"{this.ToString()} Started...");
            //Console.WriteLine($"{this.ToString()} waitting for resources {needResources}...");
            Storehouse.AddTask(new Tasks.GetResourcesTask(this, getResourceSem, needResources));
            getResourceSem.Wait();
            LogEndWork();
            //Console.WriteLine($"{this.ToString()} Back to labolatory...");
            StateLogger.DrawState($"{this.ToString()} Back to labolatory...");
        }
    }
}
