using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WUT.Zad1.Lib.Tasks
{
    public sealed class GetResourcesTask:ITask
    {
        private Semaphore semaphore;
        private ResourceTypes needResources;
        private object Owner;
        public GetResourcesTask(object owner,Semaphore _semaphore, ResourceTypes _needResources)
        {
            semaphore = _semaphore;
            needResources = _needResources;
            Owner = owner;
        }

        public int Piority { get; set; }

        public bool CanExecute(ResourceTypes availableResources)
        {
            return (availableResources & needResources) == needResources;
        }

        public ResourceTypes Execute()
        {
            //Console.WriteLine($"{Owner} : resources are available. Take resources : {needResources}");
            StateLogger.DrawState($"{Owner} : resources are available. Take resources : {needResources}");
            semaphore.Release();
            return needResources;
        }
    }
}
