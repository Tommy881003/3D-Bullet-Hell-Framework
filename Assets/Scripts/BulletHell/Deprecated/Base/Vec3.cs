using System;

/*namespace BulletHell3D.Base
{
    /// <summary>
    /// A custom class represent 3D vector.
    /// </summary>
    [System.Serializable]
    public struct Vec3
    {
        public readonly float x;
        public readonly float y;
        public readonly float z;
        public readonly float magnitude;
        public readonly float sqrMagnitude;

        public Vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            sqrMagnitude = x * x + y * y + z * z;
            magnitude = (float)Math.Sqrt(sqrMagnitude);
        }

        public static readonly Vec3 zero = new Vec3(0,0,0);
        public static readonly Vec3 one = new Vec3(1,1,1);
        public static Vec3 operator +(Vec3 a, Vec3 b) { return new Vec3(a.x + b.x, a.y + b.y, a.z + b.z); }
        public static Vec3 operator -(Vec3 a, Vec3 b) { return new Vec3(a.x - b.x, a.y - b.y, a.z - b.z); }
        public static Vec3 operator *(Vec3 a, float b) { return new Vec3(a.x * b, a.y * b, a.z * b); }
        public static Vec3 operator *(float b, Vec3 a) { return new Vec3(a.x * b, a.y * b, a.z * b); }
        public static Vec3 operator /(Vec3 a, float b) { return new Vec3(a.x / b, a.y / b, a.z / b); }

        public static float Dot(Vec3 a, Vec3 b) { return a.x * b.x + a.y * b.y + a.z * b.z; }
        public static Vec3 Cross(Vec3 a, Vec3 b) { return new Vec3(a.y*b.z-a.z*b.y, a.z*b.x-a.x*b.z, a.x*b.y-a.y*b.x); }
        public static Vec3 Normalize(Vec3 vec) { return (vec.magnitude == 0)? vec : vec / vec.magnitude; }
        public static UnityEngine.Vector3 ToUnity(Vec3 vec) { return new UnityEngine.Vector3(vec.x, vec.y, vec.z); }
        public static Vec3 FromUnity(UnityEngine.Vector3 vec) { return new Vec3(vec.x, vec.y, vec.z); }
    }
}*/