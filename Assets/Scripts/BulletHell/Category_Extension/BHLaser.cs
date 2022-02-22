using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell3D
{
    //TODO: Make this compatible with BHManager update pipeline...?
    [RequireComponent(typeof(LineRenderer))]
    public class BHLaser : MonoBehaviour, IBHRayUpdater
    {
        private LineRenderer laserLr;
        private const float maxLaserLength = 100;

        [SerializeField]
        private Material laserMaterial;
        [SerializeField]
        private float _rayRadius;
        [SerializeField, Tooltip("Auto destroy when radius of ray is 0?")]
        private bool autoDestroy;

        public BHRay[] rays { get; } = new BHRay[1];
        public float rayRadius { get { return _rayRadius; } }

        void Start() 
        {
            // Initialize laser parameters.
            laserLr = GetComponent<LineRenderer>();
            laserLr.material = laserMaterial;
            laserLr.startWidth = _rayRadius * 2;
            laserLr.endWidth = _rayRadius * 2;
            laserLr.loop = false;
            laserLr.positionCount = 2;
            laserLr.SetPosition(0, transform.position);
            laserLr.SetPosition(1, transform.position + transform.forward * maxLaserLength);
            InitUpdater();
        }

        #region IBHRayUpdater Implementation

        public void DestroyUpdater()
        {
            BHUpdaterHelper.DefaultDestroy(this);
        }

        public void InitUpdater()
        {
            BHUpdaterHelper.DefaultInit(this);
            rays[0] = new BHRay(transform.position, transform.forward, maxLaserLength);
        }

        public void UpdateRays(float deltaTime)
        {
            rays[0] = new BHRay(transform.position, transform.forward, maxLaserLength);
            laserLr.startWidth = _rayRadius * 2;
            laserLr.endWidth = _rayRadius * 2;
            laserLr.SetPosition(0, transform.position);
            laserLr.SetPosition(1, transform.position + transform.forward * maxLaserLength);

            if(rayRadius == 0)
                Destroy(gameObject);
        }

        #endregion

        public void SetRayRadius(float radius)
        {
            _rayRadius = radius;
        }

        private void OnDestroy() 
        {
            DestroyUpdater();    
        }
    }
}

