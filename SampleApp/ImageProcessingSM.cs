using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StateMachineLib;
using System.Drawing;
using System.Threading;
using AForge.Imaging;

namespace ImageProcessingApp
{
    [Serializable]
    class ImageProcessingSM : StateMachine
    {
        public ImageProcessingSM(StateMachineData data) : base(data) { }
        protected override void performComputation(IInputParameter[] parameters, int currentState)
        {
            try
            {
                int currentChunk = currentState;
                Bitmap machineBitmap = (Bitmap)((ImageParameter)parameters[0]).GetValue();
                int phaseBitmapWidth = (machineBitmap.Height * (currentState+1)> machineBitmap.Width) ? machineBitmap.Width - (machineBitmap.Height* currentState) : machineBitmap.Height;
                Bitmap phaseBitmap = machineBitmap.Clone(new Rectangle(currentChunk * machineBitmap.Height, 0, phaseBitmapWidth, machineBitmap.Height), machineBitmap.PixelFormat);

                
                AForge.Imaging.Filters.GaussianBlur gaussianBlur = new AForge.Imaging.Filters.GaussianBlur(3, 21);
                phaseBitmap = gaussianBlur.Apply(phaseBitmap);

                //AForge.Imaging.Filters.SobelEdgeDetector sed = new AForge.Imaging.Filters.SobelEdgeDetector();
                //phaseBitmap = sed.Apply(phaseBitmap);
                //AForge.Imaging.Filters.EuclideanColorFiltering ecf = new AForge.Imaging.Filters.EuclideanColorFiltering(Color.Black, 1000);
                


                for (int i = 0; i < phaseBitmap.Width; i++)
                {
                    for (int j = 0; j < phaseBitmap.Height; j++)
                    {
                        try
                        {
                            machineBitmap.SetPixel(currentChunk * machineBitmap.Height + i, j, phaseBitmap.GetPixel(i, j));
                        }
                        catch(Exception ex) 
                        {
                            System.Console.Out.WriteLine("Could not set pixel for machine: " + ID);
                        }
                    }
                }


                //System.Console.Out.WriteLine("Machine " + ID + " performed state " + currentState);

                parameters[0].SetValue(machineBitmap);
            }
            catch (Exception ex)
            {
                System.Console.Out.WriteLine("Exception" + ex.Message);
            }
        }
    }
}
