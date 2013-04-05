using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring
{
    static class MatrixHelper
    {
        public static Matrix Identity
        {
            get { return Matrix.Identity; }
        }

        public static Matrix New(float m11, float m12, float m13, float m14,
                                 float m21, float m22, float m23, float m24,
                                 float m31, float m32, float m33, float m34,
                                 float m41, float m42, float m43, float m44)
        {
            return new Matrix(m11, m12, m13, m14,
                              m21, m22, m23, m24,
                              m31, m32, m33, m34,
                              m41, m42, m43, m44);
        }

        public static Matrix Translate(float x, float y)
        {
            return Translate(x, y, 0.0f);
        }

        public static Matrix Translate(float x, float y, float z)
        {
            return Matrix.CreateTranslation(x, y, z);
        }

        public static Matrix Translate(Vector3 vec)
        {
            return Matrix.CreateTranslation(vec);
        }

        public static Matrix Scale(float x, float y)
        {
            return Scale(x, y, 1.0f);
        }

        public static Matrix Scale(float x, float y, float z)
        {
            return Matrix.CreateScale(x, y, z);
        }

        public static Matrix RotateZ(float theta)
        {
            return Matrix.CreateRotationZ(theta);
        }

        public static Matrix Rotate(Vector3 axis, float theta)
        {
            return Matrix.CreateFromAxisAngle(axis, theta);
        }

        public static Matrix Invert(this Matrix matrix)
        {
            return Matrix.Invert(matrix);
        }

        public static string Serialize(this Matrix matrix)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(matrix.M11.ToString());
            sb.Append(",");
            sb.Append(matrix.M12.ToString());
            sb.Append(",");
            sb.Append(matrix.M13.ToString());
            sb.Append(",");
            sb.Append(matrix.M14.ToString());
            sb.Append(",");
            sb.Append(matrix.M21.ToString());
            sb.Append(",");
            sb.Append(matrix.M22.ToString());
            sb.Append(",");
            sb.Append(matrix.M23.ToString());
            sb.Append(",");
            sb.Append(matrix.M24.ToString());
            sb.Append(",");
            sb.Append(matrix.M31.ToString());
            sb.Append(",");
            sb.Append(matrix.M32.ToString());
            sb.Append(",");
            sb.Append(matrix.M33.ToString());
            sb.Append(",");
            sb.Append(matrix.M34.ToString());
            sb.Append(",");
            sb.Append(matrix.M41.ToString());
            sb.Append(",");
            sb.Append(matrix.M42.ToString());
            sb.Append(",");
            sb.Append(matrix.M43.ToString());
            sb.Append(",");
            sb.Append(matrix.M44.ToString());
            return sb.ToString();
        }

        public static Matrix Deserialize(string str)
        {
            string[] tokens = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 16)
            {
                throw new FormatException("Can't convert string to matrix.");
            }
            float[] elems = new float[16];
            elems.Length.Repeat(i => elems[i] = float.Parse(tokens[i]));
            return new Matrix(elems[0], elems[1], elems[2], elems[3],
                              elems[4], elems[5], elems[6], elems[7],
                              elems[8], elems[9], elems[10], elems[11],
                              elems[12], elems[13], elems[14], elems[15]);
        }

        public static Vector3 TransformCoord(this Matrix matrix, Vector3 vec)
        {
            Vector4 hvec = Vector4.Transform(new Vector4(vec, 1), matrix);
            return new Vector3(hvec.X, hvec.Y, hvec.Z) / hvec.W;
        }

        public static Vector3 TransformNormal(this Matrix matrix, Vector3 vec)
        {
            Vector4 hvec = Vector4.Transform(new Vector4(vec, 0), matrix);
            return new Vector3(hvec.X, hvec.Y, hvec.Z);
        }

        public static Ray Transform(this Matrix matrix, Ray ray)
        {
            var pt1 = matrix.TransformCoord(ray.Origin);
            var pt2 = matrix.TransformCoord(ray.Origin + ray.Direction);

            return new Ray
            {
                Origin = pt1,
                Direction = pt2 - pt1
            };
        }

        public static string ToHex(this Color color, bool rgba)
        {
            uint value = ((uint)color.R << 16) + ((uint)color.G << 8) + (uint)color.B;
            if (rgba)
            {
                value = (value << 8) + color.A;
            }
            return String.Format("{0:X6}", value);
        }
    }
}
