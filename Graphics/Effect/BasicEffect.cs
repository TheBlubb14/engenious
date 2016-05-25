﻿using System;
using OpenTK;

namespace engenious.Graphics
{
    public class BasicEffect : Effect,IEffectMatrices
    {
        private const string vertexShader = 
            @"
#version 440

in vec3 position;
in vec4 color;
in vec2 textureCoordinate;
out vec4 psColor;
out vec2 psTexCoord;

uniform mat4 World;
uniform mat4 View;
uniform mat4 Proj;
void main(void)
{
   gl_Position = Proj*View*World*vec4(position, 1.0);
   psColor = color;
   psTexCoord = textureCoordinate;
}
";
        private const string pixelShader = 
            @"
#version 440
in vec4 psColor;
in vec2 psTexCoord;
uniform sampler2D text;
uniform int textEnabled,colorEnabled;
void main(void)
{
   gl_FragColor = vec4(1.0,1.0,1.0,1.0);
   if (textEnabled == 1)
     gl_FragColor = gl_FragColor * texture2D(text,psTexCoord);
   if (colorEnabled == 1)
     gl_FragColor = gl_FragColor * psColor;
   
}
";

        public BasicEffect(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            var technique = new EffectTechnique("Basic");
            ThreadingHelper.BlockOnUIThread(()=>{
            Shader[] shaders = new Shader[]
            {
                new Shader(ShaderType.VertexShader, vertexShader),
                new Shader(ShaderType.FragmentShader, pixelShader)
            };

            foreach (Shader shader in shaders)
                shader.Compile();
            EffectPass pass = new EffectPass("Basic");
            pass.AttachShaders(shaders);
            pass.BindAttribute(VertexElementUsage.Color, "color");
            pass.BindAttribute(VertexElementUsage.TextureCoordinate, "textureCoordinate");
            pass.BindAttribute(VertexElementUsage.Position, "position");
            pass.Link();



            technique.Passes.Add(pass);
            Techniques.Add(technique);
            });
            CurrentTechnique = technique;


            Initialize();

            World = View = Projection = Matrix.Identity;


        }

        #region IEffectMatrices implementation

        private Matrix world, view, projection;

        public Matrix Projection
        {
            get
            {
                return projection;
            }
            set
            {
                if (projection != value)
                {
                    projection = value;
                    Parameters["Proj"].SetValue(value);
                }
            }
        }

        public Matrix View
        {
            get
            {
                return view;
            }
            set
            {
                if (view != value)
                {
                    view = value;
                    Parameters["View"].SetValue(value);
                }
            }
        }

        public Matrix World
        {
            get
            {
                return world;
            }
            set
            {
                if (world != value)
                {
                    world = value;
                    Parameters["World"].SetValue(value);
                }
            }
        }

        public Texture Texture
        {

            set
            {
                GraphicsDevice.Textures[0] = value;
                Parameters["text"].SetValue(0);
            }
        }

        public bool TextureEnabled
        {

            set
            {
                Parameters["textEnabled"].SetValue(value ? 1 : 0);
            }
        }

        public bool VertexColorEnabled
        {

            set
            {
                Parameters["colorEnabled"].SetValue(value ? 1 : 0);
            }
        }

        #endregion
    }
}

