using System;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell3D
{
    // Memo: Behaviour of BHTransformUpdater
    // 1. CANNOT add bullets once initialized.
    // 2. Auto-destroy once there are no bullet left. (That is, if you create a transformUpdater with no bullets, it'll auto-destroy very soon.)
    public class BHTransformUpdater : MonoBehaviour, IBHBulletUpdater
    {
        public List<BHBullet> bullets { get; protected set; } = new List<BHBullet>();
        protected List<Vector3> localPos = new List<Vector3>();

        [SerializeField]
        protected BHPattern mainPattern;

        public void Init(Vector3 position, Vector3 forwardAxis, float angleInDeg)
        {
            InitUpdater();

            // Orthonormal vectors in relative coordinate.
            // Forward: z-axis
            // Right: x-axis
            // Up: y-axis
            Vector3 relativeForward;
            Vector3 relativeRight;
            Vector3 relativeUp;
            
            relativeForward = forwardAxis.normalized;
            BHHelper.LookRotationSolver(forwardAxis, angleInDeg, out relativeUp, out relativeRight);

            transform.position = position;
            transform.rotation = Quaternion.LookRotation(relativeForward, relativeUp);
            transform.localScale = Vector3.zero;

            if(mainPattern != null)
            {
                foreach(Vector3 pos in mainPattern.bullets)
                {
                    bullets.Add(new BHBullet(transform.TransformPoint(pos), mainPattern.renderObject));
                    localPos.Add(pos);
                }
            }
        }

        #region IBHBulletUpdater Implementation

        public void InitUpdater() { BHUpdatableHelper.DefaultInit(this); }

        public void UpdateBullets(float deltaTime)
        {
            UpdateTransform(deltaTime);
            for(int i = 0 ; i < bullets.Count; i ++)
                bullets[i].SetPosition(transform.TransformPoint(localPos[i]));
            if(bullets.Count == 0)
                Destroy(gameObject);
        }

        public void RemoveBullets()
        {
            BHUpdatableHelper.DefaultRemoveBullets(this, ref localPos);
        }

        public void DestroyUpdater()
        {
            BHUpdatableHelper.DefaultDestroy(this);
        }

        #endregion

        protected virtual void UpdateTransform(float deltaTime) { }

        private void OnDestroy() 
        {
            DestroyUpdater();
        }
    }
}