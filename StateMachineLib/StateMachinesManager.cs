using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;

namespace StateMachineLib
{
    public class StateMachinesManager
    {
        private static int messageNumber = 0;

        List<StateMachine> StateMachines { get; set; }
        private String managerID;
        
        //private Thread thLoadBroadcaster;

        private bool hasRunAtLeastOneMachine = false;

        public bool IsRunning { get; set; }
        
        public Type LoadInfoType { get; set; }
    

        public StateMachinesManager(String managerID, Type loadInfoType)
        {
            this.managerID = managerID;
            StateMachines = new List<StateMachine>();
            LoadInfoType = loadInfoType;
            IsRunning = true;
        }

        public void Run()
        {
            try
            {
                while (IsRunning)
                {
                    
                    while (GetNumberOfRunningMachines() > 0)
                    {
                        //printToConsole("nb of machines = " + GetNumberOfRunningMachines());
                        Thread.Sleep(2000);
                    }
                    IsRunning = false;
                }
            }
            catch (ThreadAbortException)
            {
                printToConsole("Load negociation stopped.");
            }
        }

        public int GetNbOfMachines()
        {
            return StateMachines.Count;
        }

        public int GetNumberOfUncompletedMachines()
        {
            int rtn = 0;
            foreach (StateMachine machine in StateMachines)
            {
                if (!machine.IsCompleted) rtn++;
            }
            return rtn;
        }

        public int GetNumberOfRunningMachines()
        {
            int rtn = 0;
            foreach (StateMachine machine in StateMachines)
            {
                if (machine.IsRunning) rtn++;
            }
            return rtn;
        }

        private StateMachine GetMachineToBeSent()
        {
            foreach (StateMachine machine in StateMachines)
            {
                if (machine.IsRunning) return machine;
            }
            printToConsole("No machine is running, sending the first non-running machine");
            if (StateMachines.Count > 0)
                return StateMachines[0];

            printToConsole("No machine found on host.");
            return null;
        }

        public void AddMachine(StateMachine machine)
        {
            StateMachines.Add(machine);
            printToConsole("Machine " + machine.ID + " added.");
        }
        public void AddAndRunMachine(StateMachine machine)
        {
            AddMachine(machine);
            machine.Start();
            hasRunAtLeastOneMachine = true;
            printToConsole("Machine " + machine.ID + " started.");
        }

        public void UnpackAndRunMachine(StateMachineData machineData)
        {
            Type machineType = machineData.MachineType;
            ConstructorInfo constructInfo = machineType.GetConstructor(new Type[] { typeof(StateMachineData) });
            StateMachine machine = (StateMachine)constructInfo.Invoke(new object[] { machineData });
            StateMachines.Add(machine);
            printToConsole("Machine " + machine.ID + " unpacked.");
            machine.Start();
            hasRunAtLeastOneMachine = true;
            printToConsole("Machine " + machine.ID + " started.");
        }

        public void RemoveMachine(int machineID)
        {
            foreach (StateMachine machine in StateMachines)
            {
                if (machine.getMachineID() == machineID)
                {
                    machine.Stop();
                    printToConsole("Machine " + machine.ID + " stopped.");
                    StateMachines.Remove(machine);
                    printToConsole("Machine " + machine.ID + " removed.");
                    break;
                }
            }
        }

        public void RunMachines()
        {
            foreach (StateMachine machine in StateMachines)
            {
                if (!machine.IsRunning)
                {
                    machine.Start();
                    hasRunAtLeastOneMachine = true;
                    printToConsole("Machine " + machine.ID + " started.");
                }
                else
                {
                    printToConsole("Machine " + machine.ID + " is already running.");
                }
            }
        }

        public void JoinMachines()
        {
            foreach (StateMachine machine in StateMachines)
            {
                machine.Join();
                printToConsole("Machine " + machine.ID + " joined.");
            }
        }

        public void StopMachines()
        {
            foreach (StateMachine machine in StateMachines)
            {
                machine.Stop();
                printToConsole("Machine " + machine.ID + " stopped.");
            }
        }

        public bool MPISendMachine(int machineID, int destination)
        {
            for (int i = 0; i < StateMachines.Count; i++)
            {
                if (StateMachines.ElementAt(i).ID == machineID)
                {
                    if (StateMachines.ElementAt(i).IsRunning)
                    {
                        StateMachines.ElementAt(i).Stop();
                    }
                    printToConsole("Sending machine " + machineID + " to process <" + destination + ">");
                    MPI.Communicator.world.Send<StateMachineData>(StateMachines.ElementAt(i).pack(), destination, 100);
                    printToConsole("Sent machine [" + machineID + "] to process <" + destination + ">");
                    StateMachines.RemoveAt(i);
                    printToConsole("Machine " + machineID + " removed.");
                    return true;
                }
            }
            return false;
        }

        public bool MPIReceiveMachine(int source)
        {
            printToConsole("Receiving machine from process <" + source + ">");
            StateMachineData data = MPI.Communicator.world.Receive<StateMachineData>(source, 100);
            printToConsole("Received machine [" + data.ID + "] from process <" + source + ">");
            this.UnpackAndRunMachine(data);
            return true;
        }

        public List<StateMachineData> GetMachinesData()
        {
            List<StateMachineData> dataList = new List<StateMachineData>();
            foreach (StateMachine machine in StateMachines)
            {
                if (machine.IsRunning) throw new StateMachineException("Invalid operation: There are machines that are still running");

                dataList.Add(machine.pack());
            }

            return dataList;
        }


        private void printToConsole(String message)
        {
            //System.Console.WriteLine("Message " + (messageNumber++) + ": <" + managerID + ">: " + message);
        }

        private void printToConsoleByRoot(String message)
        {
            if (MPI.Communicator.world.Rank == 0)
            {
                //System.Console.WriteLine("Message " + (messageNumber++) + ": <" + managerID + ">: " + message);
                //Thread.Sleep(500);
            }
        }

    }
}

