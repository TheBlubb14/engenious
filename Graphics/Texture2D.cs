﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using engenious.Content;
using engenious.Content.Serialization;
using engenious.Helper;
using OpenTK.Graphics.OpenGL;

namespace engenious.Graphics
{
    public class Texture2D : Texture
    {
        internal int Texture;
        private readonly PixelInternalFormat _internalFormat;

        public Texture2D(GraphicsDevice graphicsDevice, int width, int height, int mipMaps = 1,
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba8)
            : base(graphicsDevice, 1, internalFormat)
        {
            _internalFormat = internalFormat;
            Width = width;
            Height = height;
            Bounds = new Rectangle(0, 0, width, height);
            using (Execute.OnUiContext)
            {
                Texture = GL.GenTexture();

                Bind();
                SetDefaultTextureParameters();
                var isDepthTarget = ((int) internalFormat >= (int) PixelInternalFormat.DepthComponent16 &&
                                      (int) internalFormat <= (int) PixelInternalFormat.DepthComponent32Sgix);
                if (isDepthTarget)
                    GL.TexImage2D(
                        Target,
                        0,
                        (OpenTK.Graphics.OpenGL.PixelInternalFormat) internalFormat,
                        width,
                        height,
                        0,
                        OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent,
                        PixelType.Float,
                        IntPtr.Zero);
                else
                    GL.TexStorage2D(
                        TextureTarget2d.Texture2D,
                        mipMaps,
                        (SizedInternalFormat) internalFormat,
                        width,
                        height);

                //GL.TexImage2D(Target, 0, (OpenTK.Graphics.OpenGL.PixelInternalFormat)internalFormat, width, height, 0, (OpenTK.Graphics.OpenGL.PixelFormat)Format, PixelType.UnsignedByte, IntPtr.Zero);
            }
        }

        private void SetDefaultTextureParameters()
        {
            GL.TexParameter(Target, TextureParameterName.TextureMinFilter, (int) All.Linear);
            GL.TexParameter(Target, TextureParameterName.TextureMagFilter, (int) All.Linear);
            //if (GL.SupportsExtension ("Version12")) {
            //GL.TexParameter(Target, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            //GL.TexParameter(Target, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);


            /*} else {
				GL.TexParameter (Target, TextureParameterName.TextureWrapS, (int)All.Clamp);
				GL.TexParameter (Target, TextureParameterName.TextureWrapT, (int)All.Clamp);
			}*/
        }

        internal override void Bind()
        {
            GL.BindTexture(Target, Texture);
        }

        public override void BindComputation(int unit = 0)
        {
            GL.BindImageTexture(unit, Texture, 0, false, 0, TextureAccess.WriteOnly,
                (SizedInternalFormat) _internalFormat);
        }

        internal override TextureTarget Target => TextureTarget.Texture2D;

        public int Width { get; }

        public int Height { get; }

        public Rectangle Bounds { get; private set; }

        public void SetData<T>(T[] data, int level = 0)
            where T : struct
        {
            SetData(data,level,OpenTK.Graphics.OpenGL.PixelFormat.Bgra);
        }

        internal void SetData<T>(T[] data, int level,OpenTK.Graphics.OpenGL.PixelFormat format)
            where T : struct //ValueType
        {
            var hwCompressed =
                format == (OpenTK.Graphics.OpenGL.PixelFormat) TextureContentFormat.DXT1 ||
                format == (OpenTK.Graphics.OpenGL.PixelFormat) TextureContentFormat.DXT3 || format ==
                (OpenTK.Graphics.OpenGL.PixelFormat) TextureContentFormat.DXT5;
            if (!hwCompressed && Marshal.SizeOf(typeof(T)) * data.Length < Width * Height)
                throw new ArgumentException("Not enough pixel data");

            int width = Width, height = Height;
            for (var i = 0; i < level; i++)
            {
                width /= 2;
                height /= 2;
                if (width == 0 || height == 0)
                    return;
            }
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            using (Execute.OnUiContext)
            {
                Bind();
                var pxType = PixelType.UnsignedByte;
                if (typeof(T) == typeof(Color))
                    pxType = PixelType.Float;
                if (hwCompressed)
                {
                    var blockSize = (format ==
                                     (OpenTK.Graphics.OpenGL.PixelFormat) TextureContentFormat
                                         .DXT1)
                        ? 8
                        : 16;
                    var mipSize = ((width + 3) / 4) * ((height + 3) / 4) * blockSize;
                    GL.CompressedTexSubImage2D(Target, level, 0, 0, width, height,
                        format, mipSize, handle.AddrOfPinnedObject());
                }
                else
                    GL.TexSubImage2D(Target, level, 0, 0, width, height, format, pxType,
                        handle.AddrOfPinnedObject());
            }
            handle.Free();
            //GL.TexSubImage2D<T> (Target, 0, 0, 0, Width, Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, getPixelType (typeof(T)), data);
        }

        public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct
        {
            throw new NotImplementedException("Need to implement offset");
            //TODO:GL.TexSubImage2D<T> (Target, 0, 0, 0, Width, Height, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, data);
        }

        public void SetData<T>(int level, Rectangle? rect, T[] data, int startIndex, int elementCount)
            where T : struct
        {
            throw new NotImplementedException("Need to implement offset"); //TODO:
            /*if (rect.HasValue)
				GL.TexSubImage2D<T> (Target, level, rect.Value.X, rect.Value.Y, rect.Value.Width, rect.Value.Height, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, data);
			else
				GL.TexSubImage2D<T> (Target, level, 0, 0, Width, Height, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, data);*/
        }

        public void GetData<T>(T[] data) where T : struct //ValueType
        {
            using (Execute.OnUiContext)
            {
                Bind();

                /*if (glFormat == All.CompressedTextureFormats) {
            throw new NotImplementedException ();
        } else {*/
                var level = 0;
                /*Rectangle? rect = new Rectangle?(); //new Rectangle? (new Rectangle (0, 0, Width, Height));
                if (rect.HasValue)
                {
                    //TODO:
                    var temp = new T[this.Width * this.Height];
                    GL.GetTexImage(Target, level, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                        PixelType.UnsignedByte, temp);
                    int z = 0, w = 0;

                    for (int y = rect.Value.Y; y < rect.Value.Y + rect.Value.Height; ++y)
                    {
                        for (int x = rect.Value.X; x < rect.Value.X + rect.Value.Width; ++x)
                        {
                            data[z * rect.Value.Width + w] = temp[(y * Width) + x];
                            ++w;
                        }
                        ++z;
                        w = 0;
                    }
                }
                else*/
                {
                    GL.GetTexImage(Target, level, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                        PixelType.UnsignedByte, data);
                }
            }
            //}
        }

        public void GetData<T>(int level, Rectangle? rect, T[] data, int startIndex, int elementCount)
            where T : struct
        {
            using (Execute.OnUiContext)
            {
                Bind();
                if (rect.HasValue)
                {
                    //TODO: use ? GL.CopyImageSubData(
                    var temp = new T[Width * Height];
                    GL.GetTexImage(Target, level, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                        PixelType.UnsignedByte, temp);
                    int z = 0, w = 0;

                    for (var y = rect.Value.Y; y < rect.Value.Y + rect.Value.Height; ++y)
                    {
                        for (var x = rect.Value.X; x < rect.Value.X + rect.Value.Width; ++x)
                        {
                            data[z * rect.Value.Width + w] = temp[(y * Width) + x];
                            ++w;
                        }
                        ++z;
                        w = 0;
                    }
                }
                else
                {
                    GL.GetTexImage(Target, level, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                        PixelType.UnsignedByte, data);
                }
            }
        }

        public static Texture2D FromStream(GraphicsDevice graphicsDevice, Stream stream)
        {
            using (var bmp = new Bitmap(stream))
            {
                return FromBitmap(graphicsDevice, bmp);
            }
        }

        public static Texture2D FromFile(GraphicsDevice graphicsDevice, string filename)
        {
            using (var bmp = new Bitmap(filename))
            {
                return FromBitmap(graphicsDevice, bmp);
            }
        }

        public static Texture2D FromBitmap(GraphicsDevice graphicsDevice, Bitmap bmp, int mipMaps = 1)
        {
            Texture2D text;
            text = new Texture2D(graphicsDevice, bmp.Width, bmp.Height, mipMaps);
            var bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, text.Width, text.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Execute.OnUiContext)
            {
                text.Bind();
                GL.TexSubImage2D(text.Target, 0, 0, 0, text.Width, text.Height,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
            }
            bmp.UnlockBits(bmpData);


            return text;
        }

        public static Bitmap ToBitmap(Texture2D text)
        {
            var bmp = new Bitmap(text.Width, text.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, text.Width, text.Height),
                ImageLockMode.WriteOnly, bmp.PixelFormat);
            using (Execute.OnUiContext)
            {
                text.Bind();
                GL.GetTexImage(text.Target, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                    PixelType.UnsignedByte, bmpData.Scan0);
            }
            //System.Runtime.InteropServices.Marshal.Copy (bmpData.Scan0, data, 0, data.Length);//TODO: performance
            //TODO: convert pixel formats


            bmp.UnlockBits(bmpData);

            return bmp;
        }

        public static Texture2D FromStream(GraphicsDevice graphicsDevice, Stream stream, int width, int height,
            bool zoom)
        {
            //TODO:implement correct
            using (var bmp = new Bitmap(stream))
            {
                using (var scaled = new Bitmap(bmp, width, height))
                {
                    //TODO: zoom?
                    return FromBitmap(graphicsDevice, scaled);
                }
            }
        }

        public override void Dispose()
        {
            using (Execute.OnUiContext)
                GL.DeleteTexture(Texture);

            base.Dispose();
        }
    }
}