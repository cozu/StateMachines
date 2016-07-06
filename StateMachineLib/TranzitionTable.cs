using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachineLib
{
    [Serializable]
    public class TranzitionTable 
    {
        
        /// <summary>
        /// Maps pairs <input parameter, current state> into another state
        /// </summary>
        List<TranzitionTableEntry> statesTranzitionTable;

        /// <summary>
        /// When machine has started the TranzitionMatrix should become read only. 
        /// The states tranzition matrix can not be dynamically changed.
        /// </summary>
        private bool IsReadOnly { get; set; }

        public int StatesCount
        {
            get
            {
                return statesTranzitionTable.Count;
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public TranzitionTable()
        {
            statesTranzitionTable = new List<TranzitionTableEntry>();
        }

        /// <summary>
        /// Adds a new state tranzition to table
        /// </summary>
        /// <param name="inputParams"></param>
        /// <param name="inputState"></param>
        /// <param name="outputState"></param>
        public void Add(List<IInputParameter> inputParams, int inputState, int outputState)
        {
            if (IsReadOnly) throw new StateTranzitionException("Read only tranzition table");
            statesTranzitionTable.Add(new TranzitionTableEntry(inputParams, inputState, outputState));
        }

        /// <summary>
        /// Adds a new tranzition table entry
        /// </summary>
        /// <param name="tranzition">table entry</param>
        public void Add(TranzitionTableEntry tranzition)
        {
            if (IsReadOnly) throw new StateTranzitionException("Read only tranzition table");
            statesTranzitionTable.Add(tranzition);
        }

        /// <summary>
        /// Returns the output state with specified input parameters and input state
        /// </summary>
        /// <param name="inputParams">input parameters</param>
        /// <param name="inputState">the input state</param>
        /// <returns></returns>
        public int GetNextState(List<IInputParameter> inputParams, int inputState)
        {
            foreach (TranzitionTableEntry entry in statesTranzitionTable)
            {
                if (entry.InputState == inputState && entry.MatchesInput(inputParams))
                    return entry.OutputState;
            }
            return -1;
        }

    }

    class StateTranzitionException : Exception
    {
        public StateTranzitionException(string message) : base(message) { }
    }
}
