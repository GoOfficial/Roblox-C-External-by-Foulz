using System;
using System.Runtime.InteropServices;

// quick maf

namespace FoulzExternal.SDK.structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2int16
    {
        public short x;
        public short y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public float[] data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3x3
    {
        public float r00, r01, r02;
        public float r10, r11, r12;
        public float r20, r21, r22;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2
    {
        public float x;
        public float y;

        public float Magnitude() => MathF.Sqrt(x * x + y * y);
        public float Distance(Vector2 b) => MathF.Sqrt(MathF.Pow(b.x - x, 2) + MathF.Pow(b.y - y, 2));
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        public float x, y, z;

        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3 { x = a.x + b.x, y = a.y + b.y, z = a.z + b.z };
        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3 { x = a.x - b.x, y = a.y - b.y, z = a.z - b.z };
        public static Vector3 operator *(Vector3 a, float s) => new Vector3 { x = a.x * s, y = a.y * s, z = a.z * s };
        public static Vector3 operator *(float s, Vector3 a) => a * s;
        public static Vector3 operator /(Vector3 a, float s) => new Vector3 { x = a.x / s, y = a.y / s, z = a.z / s };

        public float Magnitude() => MathF.Sqrt(x * x + y * y + z * z);

        public Vector3 Normalize()
        {
            float m = Magnitude();
            return m == 0 ? new Vector3() : this / m;
        }

        public Vector3 Cross(Vector3 b)
        {
            return new Vector3
            {
                x = y * b.z - z * b.y,
                y = z * b.x - x * b.z,
                z = x * b.y - y * b.x
            };
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4
    {
        public float w, x, y, z;

        public static float Dot(Vector4 a, Vector4 b) => a.w * b.w + a.x * b.x + a.y * b.y + a.z * b.z;

        public void Normalize()
        {
            float m = MathF.Sqrt(w * w + x * x + y * y + z * z);
            if (m > 0) { w /= m; x /= m; y /= m; z /= m; }
        }

        public static Vector4 FromMatrix(Matrix3x3 m)
        {
            Vector4 q = new Vector4();
            float tr = m.r00 + m.r11 + m.r22;
            if (tr > 0)
            {
                float s = MathF.Sqrt(tr + 1.0f) * 2.0f;
                q.w = 0.25f * s;
                q.x = (m.r21 - m.r12) / s;
                q.y = (m.r02 - m.r20) / s;
                q.z = (m.r10 - m.r01) / s;
            }
            else if ((m.r00 > m.r11) && (m.r00 > m.r22))
            {
                float s = MathF.Sqrt(1.0f + m.r00 - m.r11 - m.r22) * 2.0f;
                q.w = (m.r21 - m.r12) / s;
                q.x = 0.25f * s;
                q.y = (m.r01 + m.r10) / s;
                q.z = (m.r02 + m.r20) / s;
            }
            else if (m.r11 > m.r22)
            {
                float s = MathF.Sqrt(1.0f + m.r11 - m.r00 - m.r22) * 2.0f;
                q.w = (m.r02 - m.r20) / s;
                q.x = (m.r01 + m.r10) / s;
                q.y = 0.25f * s;
                q.z = (m.r12 + m.r21) / s;
            }
            else
            {
                float s = MathF.Sqrt(1.0f + m.r22 - m.r00 - m.r11) * 2.0f;
                q.w = (m.r10 - m.r01) / s;
                q.x = (m.r02 + m.r20) / s;
                q.y = (m.r12 + m.r21) / s;
                q.z = 0.25f * s;
            }
            q.Normalize();
            return q;
        }

        public static Vector4 Slerp(Vector4 a, Vector4 b, float t)
        {
            t = MathF.Clamp(t, 0.0f, 1.0f);
            float cosTheta = Dot(a, b);
            Vector4 end = b;
            if (cosTheta < 0.0f) { end.w = -b.w; end.x = -b.x; end.y = -b.y; end.z = -b.z; cosTheta = -cosTheta; }

            if (cosTheta > 0.9995f)
            {
                Vector4 r = new Vector4 { w = a.w + t * (end.w - a.w), x = a.x + t * (end.x - a.x), y = a.y + t * (end.y - a.y), z = a.z + t * (end.z - a.z) };
                r.Normalize();
                return r;
            }
            float angle = MathF.Acos(cosTheta);
            float invSin = 1.0f / MathF.Sin(angle);
            float fA = MathF.Sin((1 - t) * angle) * invSin;
            float fB = MathF.Sin(t * angle) * invSin;
            return new Vector4 { w = fA * a.w + fB * end.w, x = fA * a.x + fB * end.x, y = fA * a.y + fB * end.y, z = fA * a.z + fB * end.z };
        }

        public Matrix3x3 ToMatrix()
        {
            float x2 = x + x, y2 = y + y, z2 = z + z;
            float xx = x * x2, xy = x * y2, xz = x * z2;
            float yy = y * y2, yz = y * z2, zz = z * z2;
            float wx = w * x2, wy = w * y2, wz = w * z2;

            return new Matrix3x3
            {
                r00 = 1.0f - (yy + zz), r01 = xy - wz, r02 = xz + wy,
                r10 = xy + wz, r11 = 1.0f - (xx + zz), r12 = yz - wx,
                r20 = xz - wy, r21 = yz + wx, r22 = 1.0f - (xx + yy)
            };
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct sCFrame
    {
        public float r00, r01, r02;
        public float r10, r11, r12;
        public float r20, r21, r22;
        public float x, y, z;

        public Vector3 Position => new Vector3 { x = x, y = y, z = z };
        public Vector3 RightVector => new Vector3 { x = r00, y = r10, z = r20 };
        public Vector3 UpVector => new Vector3 { x = r01, y = r11, z = r21 };
        public Vector3 LookVector => new Vector3 { x = -r02, y = -r12, z = -r22 };

        public static sCFrame operator +(sCFrame a, sCFrame b) => new sCFrame { r00 = a.r00 + b.r00, r01 = a.r01 + b.r01, r02 = a.r02 + b.r02, r10 = a.r10 + b.r10, r11 = a.r11 + b.r11, r12 = a.r12 + b.r12, r20 = a.r20 + b.r20, r21 = a.r21 + b.r21, r22 = a.r22 + b.r22, x = a.x + b.x, y = a.y + b.y, z = a.z + b.z };
        public static sCFrame operator *(sCFrame a, float s) => new sCFrame { r00 = a.r00 * s, r01 = a.r01 * s, r02 = a.r02 * s, r10 = a.r10 * s, r11 = a.r11 * s, r12 = a.r12 * s, r20 = a.r20 * s, r21 = a.r21 * s, r22 = a.r22 * s, x = a.x * s, y = a.y * s, z = a.z * s };

        public static sCFrame LookAt(Vector3 pos, Vector3 target, Vector3 up)
        {
            Vector3 f = (target - pos).Normalize();
            Vector3 r = f.Cross(up).Normalize();
            Vector3 u = r.Cross(f);
            return new sCFrame { r00 = r.x, r01 = u.x, r02 = f.x, r10 = r.y, r11 = u.y, r12 = f.y, r20 = r.z, r21 = u.z, r22 = f.z, x = pos.x, y = pos.y, z = pos.z };
        }
    }
    internal static class MathF
    {
        public static float Sqrt(float x) => (float)System.Math.Sqrt(x);
        public static float Pow(float x, float y) => (float)System.Math.Pow(x, y);
        public static float Acos(float x) => (float)System.Math.Acos(x);
        public static float Sin(float x) => (float)System.Math.Sin(x);
        public static float Clamp(float value, float min, float max) => value < min ? min : value > max ? max : value;
    }
}