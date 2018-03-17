using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WUT.Zad1.Lib
{
    public class Storehouse
    {
        private static List<ITask> waittingTaskList = new List<ITask>();
        private static bool IsWorking = false;
        private static bool wasStoreStarted = false;
        private static Thread storeThread;
        public static Factory S_Factory { get; set; }
        public static Factory Pb_Factory { get; set; }
        public static Factory Hg_Factory { get; set; }

        private static SemaphoreSlim newResourceSem = new SemaphoreSlim(0, int.MaxValue);
        private static SemaphoreSlim taskListSem = new SemaphoreSlim(1, 1);
        private static List<ITask> SnapShotOfTaskList
        {
            get
            {
                taskListSem.Wait();
                var res = waittingTaskList.ToList();
                taskListSem.Release();
                return res;
            }
        }

        public static int WaitingCount
        {
            get
            {
                return waittingTaskList.Count;
            }
        }
        public static void SignalNewResource()
        {
            newResourceSem.Release();
        }

        public static void AddTask(ITask task)
        {
            taskListSem.Wait();
            waittingTaskList.Add(task);
            taskListSem.Release();
        }

        public static void RemoveTask(ITask task)
        {
            taskListSem.Wait();
            waittingTaskList.Remove(task);
            taskListSem.Release();
        }

        public static void StartWorkAsync()
        {
            if (wasStoreStarted)
                return;
            wasStoreStarted = true;
            IsWorking = true;
            storeThread = new Thread(DoWork);
            storeThread.Start();
        }

        public static void StopWork()
        {
            IsWorking = false;
            wasStoreStarted = false;
        }

        private static void DoWork()
        {
            while (IsWorking)
            {
                newResourceSem.Wait();
                var temListHandler = SnapShotOfTaskList;
                var resources = CheckAvailableResources();
                var groupsOfAlchemicsTasks = temListHandler.GroupBy(t => t.NeedResources).OrderBy(g=>g.FirstOrDefault().StartTime);

                foreach (var tasks in groupsOfAlchemicsTasks)
                {
                    foreach (var item in tasks)
                    {
                        if (item.CanExecute(resources))
                        {
                            GetResources(item.Execute());
                            RemoveTask(item);
                            resources = CheckAvailableResources();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
              
                Thread.Sleep(0);
            }
        }

        private static bool GetResources(ResourceTypes resource)
        {
            if ((resource & ResourceTypes.Pb) != 0)
                if(Pb_Factory.GetProduct()==0)
                    //Console.WriteLine("GetProduct returned 0 !!!!...");
                    StateLogger.DrawState("GetProduct returned 0 !!!!...");

            if ((resource & ResourceTypes.S) != 0)
                if (S_Factory.GetProduct() == 0)
                    StateLogger.DrawState("GetProduct returned 0 !!!!...");
            //Console.WriteLine("GetProduct returned 0 !!!!...");
            if ((resource & ResourceTypes.Hg) != 0)
                if (Hg_Factory.GetProduct() == 0)
                    StateLogger.DrawState("GetProduct returned 0 !!!!...");
            //Console.WriteLine("GetProduct returned 0 !!!!...");
            return true;
        }

        private static ResourceTypes CheckAvailableResources()
        {
            ResourceTypes resource = ResourceTypes.None;

            if (Pb_Factory.HasProduct)
                resource |= ResourceTypes.Pb;
            if (S_Factory.HasProduct)
                resource |= ResourceTypes.S;
            if (Hg_Factory.HasProduct)
                resource |= ResourceTypes.Hg;

            return resource;
        }
    }
}
