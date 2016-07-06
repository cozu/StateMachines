using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StateMachineLib;
using System.Drawing;

namespace ImageProcessingApp
{
    [Serializable]
    class ImageParameter : IInputParameter
    {
        Bitmap value;

        #region IInputParameter Members

        public bool Matches(object valueToMatch)
        {
            return true;
        }

        public object GetValue()
        {
            return value;
        }

        #endregion

        public ImageParameter(Bitmap value, bool isTranzitionParam)
        {
            this.value = value;
            IsTranzitionParam = isTranzitionParam;
        }

        #region IInputParameter Members


        public void SetValue(object value)
        {
            this.value = (Bitmap)value;
        }

        #endregion

        #region IInputParameter Members


        public bool IsTranzitionParam
        {
            get; set; 
        }

        #endregion
    }
}
