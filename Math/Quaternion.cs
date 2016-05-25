﻿using System;
using System.Runtime.InteropServices;

namespace engenious
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Quaternion
    {
        public float X;
        public float Y;
        public float Z;
        public float W;


        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static Quaternion operator +(Quaternion val1, Quaternion val2)
        {
            val1.X += val2.X;
            val1.Y += val2.Y;
            val1.Z += val2.Z;
            val1.W += val2.W;
            return val1;
        }

        public static Quaternion operator*(Quaternion val, float scale)
        {
            return new Quaternion(val.X * scale, val.Y * scale, val.Z * scale, val.W * scale);
        }

        public static Quaternion operator*(Quaternion val1, Quaternion val2)
        {
            OpenTK.Quaternion q;

            Quaternion result = new Quaternion();
            result.W = val1.W * val2.W - val1.X * val2.X - val1.Y * val2.Y - val1.Z * val2.Z;
            result.X = val1.W * val2.X + val1.X * val2.W + val1.Y * val2.Z - val1.Z * val2.Y;
            result.Y = val1.W * val2.Y + val1.Y * val2.W + val1.X * val2.Z - val1.Z * val2.X;
            result.Z = val1.W * val2.Z + val1.Z * val2.W + val1.X * val2.Y - val1.Y * val2.X;
            return result;
        }

        public static Quaternion Lerp(Quaternion quaternion1, Quaternion quaternion2, float amount)//copied from MonoGame
        {
            float num = amount;
            float num2 = 1f - num;
            Quaternion quaternion = new Quaternion();
            float num5 = (((quaternion1.X * quaternion2.X) + (quaternion1.Y * quaternion2.Y)) + (quaternion1.Z * quaternion2.Z)) + (quaternion1.W * quaternion2.W);
            if (num5 >= 0f)
            {
                quaternion.X = (num2 * quaternion1.X) + (num * quaternion2.X);
                quaternion.Y = (num2 * quaternion1.Y) + (num * quaternion2.Y);
                quaternion.Z = (num2 * quaternion1.Z) + (num * quaternion2.Z);
                quaternion.W = (num2 * quaternion1.W) + (num * quaternion2.W);
            }
            else
            {
                quaternion.X = (num2 * quaternion1.X) - (num * quaternion2.X);
                quaternion.Y = (num2 * quaternion1.Y) - (num * quaternion2.Y);
                quaternion.Z = (num2 * quaternion1.Z) - (num * quaternion2.Z);
                quaternion.W = (num2 * quaternion1.W) - (num * quaternion2.W);
            }
            float num4 = (((quaternion.X * quaternion.X) + (quaternion.Y * quaternion.Y)) + (quaternion.Z * quaternion.Z)) + (quaternion.W * quaternion.W);
            float num3 = 1f / ((float) Math.Sqrt((double) num4));
            quaternion.X *= num3;
            quaternion.Y *= num3;
            quaternion.Z *= num3;
            quaternion.W *= num3;
            return quaternion;
        }
    }
}

