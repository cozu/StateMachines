using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachineLib
{
    [Serializable]
    public class TranzitionTableEntry
    {
        /// <summary>
        /// Input parameters
        /// </summary>
        public List<IInputParameter> InputParameters { get; set; }

        /// <summary>
        /// Input State
        /// </summary>
        public int InputState { get; set; }

        /// <summary>
        /// Output State
        /// </summary>
        public int OutputState { get; set; }


        /// <summary>
        /// Default constructor
        /// </summary>
        public TranzitionTableEntry() { }

        /// <summary>
        /// Constructor. Sets default value for public members.
        /// </summary>
        /// <param name="inputParams"></param>
        /// <param name="inputState"></param>
        /// <param name="outputState"></param>
        public TranzitionTableEntry(List<IInputParameter> inputParams, int inputState, int outputState)
        {
            InputParameters = inputParams;
            InputState = inputState;
            OutputState = outputState;
        }

        /// <summary>
        /// Returns true if the input parameters provided match it's own input parameters
        /// </summary>
        /// <param name="inputParams"></param>
        /// <returns></returns>
        public bool MatchesInput(List<IInputParameter> inputParams)
        {
            if (inputParams.Count != InputParameters.Count) return false;
            
            //Console.WriteLine("GetNextState: input: " + inputParams[0].GetValue() + ", " + inputParams[1].GetValue());
            
            for(int i=0;i<inputParams.Count;i++)
            {
                //Console.WriteLine(" i = " + i + " Matching " + inputParams.ElementAt(i).GetValue() + " -> " + InputParameters.ElementAt(i).GetValue());
                if (inputParams[i].IsTranzitionParam && !inputParams.ElementAt(i).Matches(InputParameters.ElementAt(i).GetValue()))
                    return false;
            }

            return true;
        }



        public void Print()
        {
            Console.WriteLine("InputState: " + InputState);
            Console.Write("InputParameters:");
            for (int i = 0; i < InputParameters.Count; i++)
            {
                Console.Write(" " + InputParameters[i].GetValue().ToString());
            }
            Console.WriteLine();
            Console.WriteLine("OutputState: " + OutputState);
        }
    }
}
