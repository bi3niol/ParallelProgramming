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
        private SemaphoreSlim SemaphoreSlim;
        private ResourceTypes needResources;
        private object Owner;
        public GetResourcesTask(object owner,SemaphoreSlim _SemaphoreSlim, ResourceTypes _needResources)
        {
            SemaphoreSlim = _SemaphoreSlim;
            needResources = _needResources;
            Owner = owner;
        }

        public int Piority { get; set; }

        public ResourceTypes NeedResources => needResources;

        public DateTime StartTime { get; } = DateTime.Now;

        public bool CanExecute(ResourceTypes availableResources)
        {
            return (availableResources & needResources) == needResources;
        }

        public ResourceTypes Execute()
        {
            //Console.WriteLine($"{Owner} : resources are available. Take resources : {needResources}");
            StateLogger.DrawState($"{Owner} : resources are available. Take resources : {needResources}");
            SemaphoreSlim.Release();
            return needResources;
        }
    }
}
