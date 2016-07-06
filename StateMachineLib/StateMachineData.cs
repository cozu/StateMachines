using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachineLib
{
    [Serializable]
    public class StateMachineData
    {
        public List<IInputParameter> Parameters { get; set; }

        public TranzitionTable TranzitionTable { get; set; }

        public int CurrentState { get; set; }

        public int FinalState { get; set; }
        
        public int ID { get; set; }

        public Type MachineType { get; set; }

        public List<int> PreviousStates { get; set; }
    }
}
