using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUT.Zad1.Lib
{
    public interface ITask
    {
        int Piority { get; }
        bool CanExecute(ResourceTypes availableResources);
        void Execute();
    }
}
