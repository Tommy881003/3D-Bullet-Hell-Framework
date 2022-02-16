using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell3D
{
    public sealed class BHTracerUpdater : MonoBehaviour, IBHUpdatable
    {
        private struct Tracer
        {
            // Trail
            public TrailRenderer tr;
            // Direction of velocity
            public Vector3 forward;
            // Magnitude of velocity
            public float speed;
            // TODO: Seems to lack something...?
        }

        private static BHTracerUpdater _instance = null;
        public static BHTracerUpdater instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = BHManager.instance.gameObject.AddComponent<BHTracerUpdater>();
                    _instance.InitUpdatable();
                }
                return _instance;
            }
        }

        public List<BHBullet> bullets { get; private set; } = new List<BHBullet>();
        public List<TrailRenderer> trails { get; private set; } = new List<TrailRenderer>();

        #region IBHUpdatable Implementation

        public void InitUpdatable() { BHUpdatableHelper.DefaultInit(this); }

        public void UpdateBullets(float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveBullets()
        {
            throw new System.NotImplementedException();
        }

        public void DestroyUpdatable() 
        { 
            BHUpdatableHelper.DefaultDestroy(this);
        }

        #endregion

        public void AddBullet() {}

        public void AddPattern(BHPattern pattern, Vector3 position, Vector3 forwardAxis, float angleInDeg) {}

        private void OnDestroy() 
        {
            DestroyUpdatable(); 
            _instance = null; 
        }
    }
}