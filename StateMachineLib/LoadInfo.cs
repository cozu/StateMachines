using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachineLib
{
    [Serializable]
    public class LoadInfo : ILoadInfo
    {
        public int NumberOfMachines { get; set; }

        public LoadInfo(){ }

        public LoadInfo(int numberOfMachines)
        {
            NumberOfMachines = numberOfMachines;
        }
    }
}
