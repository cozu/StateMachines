using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachineLib
{
    public class StateMachineTranzaction
    {
        public int Sender { get; set; }
        public int Receiver { get; set; }

        public StateMachineTranzaction() { }
        public StateMachineTranzaction(int sender, int receiver)
        {
            Sender = sender;
            Receiver = receiver;
        }
    }
}
