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


        private static Semaphore taskListSem = new Semaphore(1, 1);
        private static List<ITask> SnapShotOfTaskList
        {
            get
            {
                taskListSem.WaitOne();
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

        public static void AddTask(ITask task)
        {
            taskListSem.WaitOne();
            waittingTaskList.Add(task);
            taskListSem.Release();
        }

        public static void RemoveTask(ITask task)
        {
            taskListSem.WaitOne();
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
                var temListHandler = SnapShotOfTaskList;
                var resources = CheckAvailableResources();
                foreach (var item in temListHandler)
                {
                    if (item.CanExecute(resources))
                    {
                        GetResources(item.Execute());
                        RemoveTask(item);
                        resources = CheckAvailableResources();
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
