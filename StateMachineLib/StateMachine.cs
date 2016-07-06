using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace StateMachineLib
{
    public abstract class StateMachine
    {
        StateMachineData data;

        Thread machineThread;
        
        public bool IsRunning { get; set; }

        public int ID { get { return data.ID; } }

        public bool IsCompleted { get { return this.data.CurrentState == this.data.FinalState; } }

        public StateMachine(StateMachineData machineData)
        {
            if (machineData.CurrentState < 0) throw new StateMachineException("Invalid data object: CurrentState<0");
            if (machineData.FinalState < 0) throw new StateMachineException("Invalid data object: FinalState<0");

            if (machineData.MachineType != this.GetType())
                machineData.MachineType = this.GetType();
            if (machineData.Parameters == null || machineData.Parameters.Count == 0)
                throw new StateMachineException("Invalid data object: No parameters specified");
            if (machineData.TranzitionTable == null || machineData.TranzitionTable.StatesCount == 0)
                throw new StateMachineException("Invalid data object: No tranzition specified");

            this.data = machineData;
        }

        public int getMachineID()
        {
            return data.ID;
        }

        public void Start()
        {
            machineThread = new Thread(new ThreadStart(startThread));
            IsRunning = true;
            machineThread.Start();
        }

        void startThread()
        {
            try
            {
                while (IsRunning)
                {
                    if (data.CurrentState == data.FinalState)
                    {
                        //Console.WriteLine("<Machine ID:" + ID + " has reached final state");
                        IsRunning = false;
                        break;
                    }

                    IInputParameter[] parameters = new IInputParameter[data.Parameters.Count];
                    data.Parameters.CopyTo(parameters);

                    //Console.WriteLine("<Machine ID:" + ID + " Input params copied: " + parameters[0].GetValue().ToString() + ", " + parameters[1].GetValue().ToString());

                    performComputation(parameters, data.CurrentState);
                    //Console.WriteLine("State computed " + data.CurrentState);
                    data.Parameters.Clear();
                    data.Parameters.AddRange(parameters);  
                    data.CurrentState = data.TranzitionTable.GetNextState(data.Parameters, data.CurrentState);

                    //Console.WriteLine("<Machine ID:" + ID + " Next state: " + data.CurrentState );
                    if (data.CurrentState == -1) throw new StateMachineException("Invalid State: -1");
                }
            }
            catch (Exception ex) { }
        }

        public void Stop()
        {
            IsRunning = false;
            try
            {
                machineThread.Abort();
            }
            catch (Exception ex) { }
        }

        public void Join()
        {
            machineThread.Join();
            IsRunning = false;
        }

        public StateMachineData pack()
        {
            if (IsRunning)
            {
                Stop();
            }
            return data;
        }

        protected abstract void performComputation(IInputParameter[] parameters, int currentState);
    }

    internal class MachineThreadInput
    {
        public List<IInputParameter> inputParams;
        public int initState;

        public MachineThreadInput(List<IInputParameter> iP, int iState)
        {
            inputParams = iP;
            initState = iState;
        }
    }

    public class StateMachineException : Exception
    {
        public StateMachineException(string message) : base(message) { }
    }
}
