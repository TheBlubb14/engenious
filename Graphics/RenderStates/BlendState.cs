﻿using System;

namespace engenious.Graphics
{
    public class BlendState
    {
        public static readonly BlendState AlphaBlend;
        public static readonly BlendState Additive;
        public static readonly BlendState NonPremultiplied;
        public static readonly BlendState Opaque;

        static BlendState()
        {
            AlphaBlend = new BlendState();
            /*AlphaBlend.ColorSourceBlend = BlendingFactorSrc.One;
            AlphaBlend.AlphaSourceBlend = BlendingFactorSrc.One;
            AlphaBlend.ColorDestinationBlend = BlendingFactorDest.SrcColor;
            AlphaBlend.AlphaDestinationBlend = BlendingFactorDest.OneMinusSrcAlpha;*/

            Additive = new BlendState();
            Additive.ColorSourceBlend = BlendingFactorSrc.One;
            Additive.AlphaSourceBlend = BlendingFactorSrc.One;
            Additive.ColorDestinationBlend = BlendingFactorDest.OneMinusSrcColor;//TODO: verify?	
            Additive.AlphaDestinationBlend = BlendingFactorDest.OneMinusSrcColor;

            NonPremultiplied = new BlendState();
            NonPremultiplied.ColorSourceBlend = BlendingFactorSrc.SrcAlpha;
            NonPremultiplied.AlphaSourceBlend = BlendingFactorSrc.SrcAlpha;
            NonPremultiplied.ColorDestinationBlend = BlendingFactorDest.OneMinusSrcAlpha;
            NonPremultiplied.AlphaDestinationBlend = BlendingFactorDest.OneMinusSrcAlpha;

            Opaque = new BlendState();
            Opaque.ColorSourceBlend = BlendingFactorSrc.One;
            Opaque.AlphaSourceBlend = BlendingFactorSrc.One;
            Opaque.ColorDestinationBlend = BlendingFactorDest.Zero;
            Opaque.AlphaDestinationBlend = BlendingFactorDest.Zero;

            //GL.BlendEquationSeparate(
        }

        public BlendState()
        {
            //GL.BlendFunc(BlendingFactorSrc.One,#
            this.ColorSourceBlend = BlendingFactorSrc.SrcAlpha;
            this.AlphaSourceBlend = BlendingFactorSrc.SrcAlpha;
            this.ColorDestinationBlend = BlendingFactorDest.OneMinusSrcAlpha;
            this.AlphaDestinationBlend = BlendingFactorDest.OneMinusSrcAlpha;

            this.ColorBlendFunction = BlendEquationMode.FuncAdd;
            this.AlphaBlendFunction = BlendEquationMode.FuncAdd;
        }

        public BlendingFactorSrc ColorSourceBlend{ get; set; }

        public BlendingFactorSrc AlphaSourceBlend{ get; set; }

        public BlendingFactorDest ColorDestinationBlend{ get; set; }

        public BlendingFactorDest AlphaDestinationBlend{ get; set; }

        public BlendEquationMode AlphaBlendFunction{ get; set; }

        public BlendEquationMode ColorBlendFunction{ get; set; }

        public Color BlendFactor{ get; set; }
    }
}

