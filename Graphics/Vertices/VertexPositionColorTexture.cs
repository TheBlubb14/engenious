﻿using System;
using System.Runtime.InteropServices;
using OpenTK;

namespace engenious.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionColorTexture:IVertexType
    {
        public static readonly VertexDeclaration VertexDeclaration;

        static VertexPositionColorTexture()
        {
            VertexElement[] elements = new VertexElement[]
            {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
                new VertexElement(28, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            };
            VertexDeclaration declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get
            {
                return VertexDeclaration;
            }
        }

        public VertexPositionColorTexture(Vector3 position, Color color, Vector2 textureCoord)
        {
            this.TextureCoordinate = textureCoord;
            this.Position = position;
            this.Color = color;


        }

        public Vector3 Position;
        public Color Color;
        public Vector2 TextureCoordinate;
        //public Vector3 Position{ get; private set; }

        //public Color Color{ get; private set; }

        //public Vector2 TextureCoordinate{ get; private set; }
    }
}

