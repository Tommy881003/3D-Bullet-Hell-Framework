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
}