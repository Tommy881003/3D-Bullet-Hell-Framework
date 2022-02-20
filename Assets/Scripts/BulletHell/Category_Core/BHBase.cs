using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell3D
{
    public class BHBullet
    {
        // Position in world coordinate.
        public Vector3 position { get; private set; }
        // Position difference within single timestep.
        // position - delta = Position of last frame.
        public Vector3 delta { get; private set; } = Vector3.zero;
        // Is this bullet alive?
        public bool isAlive = true;
        public BHRenderObject renderObject;

        public BHBullet(Vector3 pos, BHRenderObject render)
        {
            position = pos;
            renderObject = render;
            //CalculateOrthos(direction);
        }

        public void SetPosition(Vector3 newPos)
        {
            delta = newPos - position;
            position = newPos;
        }
    }

    public struct BHRay
    {
        public Vector3 origin;
        public Vector3 direction;
        public float length;
        public Vector3 end { get { return origin + direction * length; } }
        public BHRay(Vector3 origin, Vector3 direction, float length)
        {
            this.origin = origin;
            this.direction = direction.normalized;
            this.length = length;
        }
    }
}