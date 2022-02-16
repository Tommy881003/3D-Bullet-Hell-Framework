using System;

/*namespace BulletHell3D.Base
{
    [System.Serializable]
    public class Triangle
    {
        public int layer { get; private set; }
        public Vec3 a { get; private set; }
        public Vec3 b { get; private set; }
        public Vec3 c { get; private set; }
        public Vec3 normal { get; private set; }
        public Plane plane { get; private set; }
        public AABB bound { get; private set; }

        public void SetVertices(Vec3 pointA, Vec3 pointB, Vec3 pointC)
        {
            a = pointA;
            b = pointB;
            c = pointC;
            CalculateData();
        }

        private void CalculateData()
        {
            normal = Vec3.Cross(b-a, c-a);
            plane = new Plane(normal.x, normal.y, normal.z, -Vec3.Dot(normal,a));

            float minX, maxX, minY, maxY, minZ, maxZ;
            minX = Math.Min(Math.Min(a.x, b.x), c.x);
            maxX = Math.Max(Math.Max(a.x, b.x), c.x);
            minY = Math.Min(Math.Min(a.y, b.y), c.y);
            maxY = Math.Max(Math.Max(a.y, b.y), c.y);
            minZ = Math.Min(Math.Min(a.z, b.z), c.z);
            maxZ = Math.Max(Math.Max(a.z, b.z), c.z);
            bound = new AABB(minX, maxX, minY, maxY, minZ, maxZ);
        }
    }

    /// <summary>
    /// A structure represents a 3D plane. (In the form of ax + by + cz + d = 0)
    /// </summary>
    [System.Serializable]
    public struct Plane
    {
        public float a;
        public float b;
        public float c;
        public float d;
        public const float epsilon = 0.0001f;

        public Plane(float a, float b, float c, float d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        public bool Contains(Vec3 point)
        {
            float result = a*point.x + b*point.y + c*point.z + d;
            return (result < epsilon && result > -epsilon);
        }
    }
}*/