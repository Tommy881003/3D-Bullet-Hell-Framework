using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell3D
{
    public class BHRing : MonoBehaviour, IBHRayUpdater
    {
        private LineRenderer ringLr;
        private const float minRingRadius = 0.05f;
        private const float maxRingRadius = 25;
        private const float radiusSegmentRatio = 4;
        private const int minSegmentCount = 4;

        [SerializeField]
        private Material ringMaterial;
        [SerializeField]
        private float _rayRadius;
        [SerializeField, Tooltip("Auto destroy when radius of ray is 0?")]
        private bool autoDestroy;

        private float ringRadius = minRingRadius;

        public BHRay[] rays { get; private set; }
        public float rayRadius { get { return _rayRadius; } }

        #region IBHRayUpdater Implementation

        void Start() 
        {
            // Initialize ring parameters.
            ringLr = GetComponent<LineRenderer>();
            ringLr.material = ringMaterial;
            ringLr.startWidth = _rayRadius * 2;
            ringLr.endWidth = _rayRadius * 2;
            ringLr.loop = true;
            InitUpdater();
        }

        public void InitUpdater()
        {
            BHUpdaterHelper.DefaultInit(this);
        }

        public void DestroyUpdater()
        {
            BHUpdaterHelper.DefaultDestroy(this);
        }

        public void UpdateRays(float deltaTime)
        {
            int segmentCount = minSegmentCount + Mathf.CeilToInt(ringRadius * radiusSegmentRatio);

            rays = new BHRay[segmentCount];
            ringLr.positionCount = segmentCount;

            float angle = Mathf.PI * 2 / segmentCount;
            for(int i = 0; i < segmentCount; i ++)
            {
                Vector3 from = transform.TransformPoint(new Vector3(Mathf.Cos(angle * i),Mathf.Sin(angle * i)) * ringRadius);
                Vector3 to = transform.TransformPoint(new Vector3(Mathf.Cos(angle * (i + 1)),Mathf.Sin(angle * (i + 1))) * ringRadius);
                ringLr.SetPosition(i,from);
                rays[i] = new BHRay(from, to - from, (to - from).magnitude);
            }

            if(rayRadius == 0)
                Destroy(gameObject);
        }

        #endregion

        public void SetRayRadius(float radius)
        {
            _rayRadius = radius;
        }

        public void SetRingRadius(float radius)
        {
            ringRadius = Mathf.Clamp(radius,minRingRadius,maxRingRadius);
        }

        private void OnDestroy() 
        {
            DestroyUpdater();    
        }
    }
}

