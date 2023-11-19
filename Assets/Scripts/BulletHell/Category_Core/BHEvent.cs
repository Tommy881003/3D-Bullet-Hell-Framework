using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell3D
{
    public struct CollisionEvent
    {
        public BHBullet bullet;
        public GameObject contact;
        public RaycastHit hit;
    }
}

