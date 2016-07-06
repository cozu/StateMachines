using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using StateMachineLib;
using System.Threading;

namespace ImageProcessingApp
{
    class Program
    {
        private const string BITMAP_INPUT_PATH = "test.jpg";
        private const string BITMAP_OUTPUT_PATH = "test_out.bmp";
        private const int NUMBER_OF_MACHINES = 4;
        private const int NUMBER_OF_BITMAPS = 1;


        private static DateTime StartTime;
        private static DateTime EndTime;

        static void Main(string[] args)
        {
            using (new MPI.Environment(ref args))
            {
                List<int> myMachines = null;

                if (MPI.Communicator.world.Rank == 0)
                {
                    StateMachinesManager manager = new StateMachinesManager("0", typeof(LoadInfo));
                    Bitmap originalBitmap = new Bitmap(BITMAP_INPUT_PATH); ;
                    for (int k = 0; k < NUMBER_OF_BITMAPS; k++)
                    {
                        originalBitmap = new Bitmap(BITMAP_INPUT_PATH);

                        for (int i = 0; i < NUMBER_OF_MACHINES; i++)
                        {
                            Bitmap machineBitmap;
                            if (i == NUMBER_OF_MACHINES - 1)
                            {
                                int lastMachineOffset = (originalBitmap.Height / NUMBER_OF_MACHINES) * (NUMBER_OF_MACHINES - 1);
                                int lastMachineBitmapHeight = originalBitmap.Height - lastMachineOffset;

                                machineBitmap = originalBitmap.Clone(
                                       new Rectangle(0, lastMachineOffset, originalBitmap.Width, lastMachineBitmapHeight),
                                       originalBitmap.PixelFormat
                                   );

                            }
                            else
                            {
                                machineBitmap = originalBitmap.Clone(
                                    new Rectangle(0, i * originalBitmap.Height / NUMBER_OF_MACHINES, originalBitmap.Width, originalBitmap.Height / NUMBER_OF_MACHINES),
                                    originalBitmap.PixelFormat
                                );
                            }

                            StateMachineData data = new StateMachineData();
                            data.CurrentState = 0;
                            data.Parameters = new List<IInputParameter>();
                            data.Parameters.Add(new ImageParameter(machineBitmap, false));
                            data.ID = i;
                            int bitmapsPerMachines = (machineBitmap.Width / machineBitmap.Height) + 1;
                            data.FinalState = bitmapsPerMachines;
                            //System.Console.Out.WriteLine("Final state: " + data.FinalState);
                            data.TranzitionTable = new TranzitionTable();
                            //System.Console.Out.WriteLine("bitmapspermachines " + bitmapsPerMachines);
                            for (int j = 0; j < bitmapsPerMachines; j++)
                            {
                                List<IInputParameter> parameters = new List<IInputParameter>();
                                parameters.Add(new ImageParameter(machineBitmap, false));
                                data.TranzitionTable.Add(new TranzitionTableEntry(parameters, j, j + 1));
                            }

                            ImageProcessingSM machine = new ImageProcessingSM(data);
                            manager.AddMachine(machine);

                        }
                    }
                    
                    StartTime = DateTime.Now;
                    int numberOfMachinesPerHost = NUMBER_OF_BITMAPS * NUMBER_OF_MACHINES / MPI.Communicator.world.Size;
                    for (int i = 0; i < MPI.Communicator.world.Size - 1; i++)
                    {
                        if(i == MPI.Communicator.world.Size-2)
                            StartTime = DateTime.Now;

                        for (int j = 0; j < numberOfMachinesPerHost; j++)
                        {
                            manager.MPISendMachine(i * numberOfMachinesPerHost + j, i + 1);
                        }
                    }


                    manager.RunMachines();
                    manager.JoinMachines();
                    
                    for (int i = 1; i < MPI.Communicator.world.Size; i++)
                    {
                        for (int j = 0; j < numberOfMachinesPerHost; j++)
                        {
                            manager.MPIReceiveMachine(i);
                        }
                    }

                    manager.JoinMachines();
                    EndTime = DateTime.Now;

                    System.Console.Out.WriteLine("Processing time = " + (EndTime.Subtract(StartTime).TotalSeconds) + " s");

                    Bitmap resultBmp = new Bitmap(originalBitmap);
                    int machineBitmapHeight = originalBitmap.Height / NUMBER_OF_MACHINES;
                    StateMachineData[] dataList = manager.GetMachinesData().ToArray();
                    foreach (StateMachineData data in dataList)
                    {
                        Bitmap machineBitmap = (Bitmap)((ImageParameter)data.Parameters[0]).GetValue();
                        int machineNumber = data.ID;
                        for (int i = 0; i < machineBitmap.Width; i++)
                            for (int j = 0; j < machineBitmap.Height; j++)
                            {
                                try
                                {
                                    resultBmp.SetPixel(i, machineNumber * machineBitmapHeight + j, machineBitmap.GetPixel(i, j));
                                }
                                catch { }
                            }
                    }

                    resultBmp.Save(BITMAP_OUTPUT_PATH);
                }
                else if(MPI.Communicator.world.Rank < NUMBER_OF_MACHINES)
                {

                    StateMachinesManager manager = new StateMachinesManager(((int)MPI.Communicator.world.Rank).ToString(), typeof(LoadInfo));

                    int numberOfMachinesPerHost = NUMBER_OF_BITMAPS * NUMBER_OF_MACHINES / MPI.Communicator.world.Size;
                    for (int i = 0; i < numberOfMachinesPerHost; i++)
                    {
                        manager.MPIReceiveMachine(0);
                    }
                    manager.RunMachines();
                    manager.JoinMachines();

                    foreach(StateMachineData m in manager.GetMachinesData())
                    {
                        manager.MPISendMachine(m.ID,0);
                    }
                }
            }
        }
    }
}
