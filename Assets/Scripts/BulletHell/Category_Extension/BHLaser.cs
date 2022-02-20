using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell3D
{
    //TODO: Make this compatible with BHManager update pipeline...?
    public class BHLaser : MonoBehaviour
    {
        private LineRenderer lr;
        [ColorUsage(true, true)]
        private Color outerColor;
        [ColorUsage(true, true)]
        private Color innerColor;
        private float thickness;

        private Vector3 laserOrigin;
        private Vector3 laserDirection;

        void FixedUpdate() 
        {
            
        }
    }
}

