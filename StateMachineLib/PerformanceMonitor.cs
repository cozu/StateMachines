using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace StateMachineLib
{
    public class PerformanceMonitor
    {
        private PerformanceCounter cpuCounter;
        private PerformanceCounter ramCounter;

        public static float MACHINE_MEMORY_USAGE_MB = 20;
        public static int CPU_MAXIMUM_LOAD = 10;
        public static int MACHINE_MEMORY_TO_STAY_AVAILABLE = 500;
        public static int MACHINES_TO_ADD_IF_CPU_AVAILABILITY = 5;
        
        private static PerformanceMonitor instance = null;
        public static PerformanceMonitor Instance
        {
            get
            {
                if (instance == null)
                    instance = new PerformanceMonitor();

                return instance;
            }
        }

        private PerformanceMonitor()
        {

            cpuCounter = new PerformanceCounter();

            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";

            ramCounter = new PerformanceCounter("Memory", "Available MBytes");

        }

        public float getCurrentCpuUsage()
        {
            return cpuCounter.NextValue();
        }

        public float getAvailableRAM()
        {
            return ramCounter.NextValue();
        }
    }
}
