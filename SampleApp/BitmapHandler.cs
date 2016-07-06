using System;       
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageProcessingApp
{
    class BitmapHandler
    {
        public static byte[] BitmapToBytesArray(Bitmap img, PixelFormat pxFmt)
        {
            BitmapData bd = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, pxFmt);

            byte[] pxData = new byte[img.Width * img.Height];
            System.Runtime.InteropServices.Marshal.Copy(bd.Scan0, pxData, 0, img.Height * img.Width);
            img.UnlockBits(bd);
            return pxData;
        }
        public static Bitmap BytesArrayToBitmap(byte[] array, Bitmap originalBitmap, int newWidth, int newHeight, PixelFormat pxFmt)
        {
            BitmapData bd;
            Bitmap result = new Bitmap(newWidth, newHeight,originalBitmap.PixelFormat);
            result.Palette = originalBitmap.Palette;
            
            bd = result.LockBits(new Rectangle(0, 0, result.Width, result.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, pxFmt);
            System.Runtime.InteropServices.Marshal.Copy(array, 0, bd.Scan0, array.Length);
            result.UnlockBits(bd);
            result.SetResolution(originalBitmap.VerticalResolution, originalBitmap.HorizontalResolution);
            return result;
        }

        public static void InsertBytesToBitmap(byte[] array, int startIndex, Bitmap img, PixelFormat pxFmt)
        {
            BitmapData bd = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, pxFmt);

            IntPtr destPtr = new IntPtr(bd.Scan0.ToInt64() + (long)startIndex);
            System.Runtime.InteropServices.Marshal.Copy(array, 0, destPtr, array.Length);
            img.UnlockBits(bd);
        }

        public static IntPtr Offset(IntPtr src, int offset)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return new IntPtr((int)((uint)src + offset));
                case 8:
                    return new IntPtr((long)((long)src + (long)offset));
                default:
                    throw new NotSupportedException("Not supported");
            }
        }
    }
}
