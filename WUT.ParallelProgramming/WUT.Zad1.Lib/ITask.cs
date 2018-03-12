using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUT.Zad1.Lib
{
    public interface ITask
    {
        DateTime StartTime { get; }
        int Piority { get; }
        ResourceTypes NeedResources { get; }
        bool CanExecute(ResourceTypes availableResources);
        ResourceTypes Execute();
    }
}
