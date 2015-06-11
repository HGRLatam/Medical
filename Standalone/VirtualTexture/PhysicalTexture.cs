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
        private VirtualTextureManager virtualTextureManager;
        private String name;
        private int texelsPerPage;
        private IntSize2 size;

        private FreeImageAPI.FreeImageBitmap blitBitmap;
        private PixelBox blitBitmapBox;

        public PhysicalTexture(String name, IntSize2 size, VirtualTextureManager virtualTextureManager, int texelsPerPage)
        {
            this.name = name;
            this.texelsPerPage = texelsPerPage;
            this.size = size;
            this.virtualTextureManager = virtualTextureManager;
            this.textureName = "PhysicalTexture" + name;
            physicalTexture = TextureManager.getInstance().createManual(textureName, VirtualTextureManager.ResourceGroup, TextureType.TEX_TYPE_2D, (uint)size.Width, (uint)size.Height, 1, 0, PixelFormat.PF_A8R8G8B8, TextureUsage.TU_RENDERTARGET, null, false, 0);
            blitBitmap = new FreeImageAPI.FreeImageBitmap(texelsPerPage, texelsPerPage, FreeImageAPI.PixelFormat.Format32bppArgb);
            unsafe
            {
                blitBitmapBox = new PixelBox(0, 0, texelsPerPage, texelsPerPage, OgreDrawingUtility.getOgreFormat(blitBitmap.PixelFormat), blitBitmap.GetScanlinePointer(0).ToPointer());
            }
        }

        public void Dispose()
        {
            blitBitmapBox.Dispose();
            blitBitmap.Dispose();
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
        }

        /// <summary>
        /// Determine which pages are active and load them.
        /// Will probably have to separate this out 
        /// </summary>
        public unsafe void loadPages()
        {
            using (var buffer = physicalTexture.Value.getBuffer())
            {
                int x = 0;
                int y = 0;
                int pageCount = 0;
                foreach (var indirectionTex in virtualTextureManager.IndirectionTextures)
                {
                    //Just load the first pages we come across until we run out of space, will implement caching later
                    String originalTextureName;
                    if (indirectionTex.OriginalTextures.TryGetValue(name, out originalTextureName))
                    {
                        using (var originalTexture = TextureManager.getInstance().getByName(originalTextureName))
                        {
                            using(var originalBuffer = originalTexture.Value.getBuffer())
                            {
                                foreach (var page in indirectionTex.ActivePages)
                                {
                                    //This is shit and relies on the textures already being loaded in ogre.
                                    //This crashes the program
                                    //buffer.Value.blit(originalBuffer, new IntRect(page.x * texelsPerPage, page.y * texelsPerPage, texelsPerPage, texelsPerPage), new IntRect(x, y, texelsPerPage, texelsPerPage));

                                    //Even crappier way copying from the textures in memory to main memory and then back
                                    //originalBuffer.Value.blitToMemory(new IntRect(page.x * texelsPerPage, page.y * texelsPerPage, texelsPerPage, texelsPerPage), blitBitmapBox);
                                    //buffer.Value.blitFromMemory(blitBitmapBox, x, y, x + texelsPerPage, x + texelsPerPage);
                                    //buffer.Value.blitFromMemory(blitBitmapBox);

                                    //Increment
                                    x += texelsPerPage;
                                    if (x == size.Width)
                                    {
                                        y += texelsPerPage;
                                        x = 0;
                                        if (y == size.Height)
                                        {
                                            break; //ran out of space
                                        }
                                    }
                                }
                            }
                        }
                    }
                    pageCount += indirectionTex.ActivePages.Count;
                    //Logging.Log.Debug("{0}, {1}", x, y);
                }
                Logging.Log.Debug("{0} Page count {1}", textureName, pageCount);
            }
        }

        public String TextureName
        {
            get
            {
                return textureName;
            }
        }

        private void saveBlitBitmap()
        {
            using(var stream = System.IO.File.Open(name + "blit.bmp", System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
            {
                blitBitmap.Save(stream, FreeImageAPI.FREE_IMAGE_FORMAT.FIF_BMP);
            }
        }
    }
}
