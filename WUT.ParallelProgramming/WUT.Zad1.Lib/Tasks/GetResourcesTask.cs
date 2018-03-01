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
        public GetResourcesTask(Semaphore _semaphore, ResourceTypes _needResources)
        {
            semaphore = _semaphore;
            needResources = _needResources;
        }

        public int Piority { get; set; }

        public bool CanExecute(ResourceTypes availableResources)
        {
            return (availableResources & needResources) == needResources;
        }

        public void Execute()
        {
            semaphore.Release();
        }
    }
}
