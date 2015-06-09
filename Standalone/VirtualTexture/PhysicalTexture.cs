﻿using Engine;
using OgrePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class PhysicalTexture : IDisposable
    {
        private TexturePtr physicalTexture;
        private String textureName;

        public PhysicalTexture(String name, IntSize2 size)
        {
            this.textureName = name;
            physicalTexture = TextureManager.getInstance().createManual(textureName, VirtualTextureManager.ResourceGroup, TextureType.TEX_TYPE_2D, (uint)size.Width, (uint)size.Height, 1, 0, PixelFormat.PF_A8R8G8B8, TextureUsage.TU_DYNAMIC_WRITE_ONLY, null, false, 0);
        }

        public void Dispose()
        {
            physicalTexture.Dispose();
        }

        public unsafe void color(Color color)
        {
            using (var fiBitmap = new FreeImageAPI.FreeImageBitmap((int)physicalTexture.Value.Width, (int)physicalTexture.Value.Height, FreeImageAPI.PixelFormat.Format32bppArgb))
            {
                var fiColor = new FreeImageAPI.Color();
                fiColor.R = (byte)(color.r * 255);
                fiColor.G = (byte)(color.g * 255);
                fiColor.B = (byte)(color.b * 255);
                fiColor.A = (byte)(color.a * 255);
                fiBitmap.FillBackground(new FreeImageAPI.RGBQUAD(fiColor));

                using (var buffer = physicalTexture.Value.getBuffer())
                {
                    using (PixelBox pixelBox = new PixelBox(0, 0, fiBitmap.Width, fiBitmap.Height, OgreDrawingUtility.getOgreFormat(fiBitmap.PixelFormat), fiBitmap.GetScanlinePointer(0).ToPointer()))
                    {
                        buffer.Value.blitFromMemory(pixelBox, 0, 0, (int)physicalTexture.Value.Width, (int)physicalTexture.Value.Height);
                    }
                }
            }

            //int intColor = color.toARGB();
            //using(var buffer = physicalTexture.Value.getBuffer())
            //{
            //    buffer.Value.lockBuf(new IntRect(), HardwareBuffer.LockOptions.HBL_NORMAL); //Probably want discard here
            //    var pixelBox = buffer.Value.CurrentLock;
            //    byte* data = (byte*)pixelBox.Data;

            //    // now we loop through the whole texture pixels
            //    for (int i = 0; i < pixelBox.getWidth() / 2; i++)
            //    {
            //        for (int j = 0; j < pixelBox.getHeight() / 2; j++)
            //        {
            //            if ((i / 64 + j / 64) % 2 == 1)
            //            {
            //                *data++ = 255;
            //                *data++ = 255;
            //                *data++ = 255;
            //                *data++ = 255;

            //            }
            //            else
            //            {
            //                *data++ = 255;
            //                *data++ = 74;
            //                *data++ = 75;
            //                *data++ = 165;

            //            }
            //        }
            //    }

            //    buffer.Value.unlock();
            //}
        }

        public String TextureName
        {
            get
            {
                return textureName;
            }
        }
    }
}
