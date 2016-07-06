using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachineLib
{
    public interface IInputParameter
    {
        /// <summary>
        /// Returns true if the current input parameter's value matches the one provided
        /// </summary>
        /// <param name="value"></param>
        bool Matches(object value);

        /// <summary>
        /// Return the value of this parameter
        /// </summary>
        /// <returns></returns>
        object GetValue();


        void SetValue(object value);

        bool IsTranzitionParam  { get; set; }
    }
}
