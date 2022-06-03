using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Apple.GameController.Controller
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GCGetSymbolForInputNameResponse
    {
        public int Width;
        public int Height;
        public IntPtr DataPtr;
        public int DataLength;

        public Texture2D GetTexture()
        {
            if (Width == 0 || Height == 0 || DataLength == 0)
            {
                return null;
            }

            var data = new byte[DataLength];
            Marshal.Copy(DataPtr, data, 0, DataLength);

            var texture = new Texture2D(Width, Height);
            texture.LoadImage(data);

            return texture;
        }
    }
}