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
        public Guid groupId;
        public List<BHBullet> bullets { get; protected set; } = new List<BHBullet>();
        protected List<Vector3> localPos = new List<Vector3>();

        private bool canSetPattern = true;
        [SerializeField]
        protected BHPattern mainPattern;

        void Start()
        {
            canSetPattern = false;
            InitUpdater();

            // Orthonormal vectors in relative coordinate.
            // Forward: z-axis
            // Right: x-axis
            // Up: y-axis
            /*Vector3 relativeForward;
            Vector3 relativeRight;
            Vector3 relativeUp;
            
            relativeForward = forwardAxis.normalized;
            BHHelper.LookRotationSolver(forwardAxis, angleInDeg, out relativeUp, out relativeRight);

            transform.position = position;
            transform.rotation = Quaternion.LookRotation(relativeForward, relativeUp);*/
            transform.localScale = Vector3.zero;
            if (mainPattern != null)
            {
                foreach (Vector3 pos in mainPattern.bullets)
                {
                    bullets.Add(new BHBullet(
                        transform.TransformPoint(pos),
                        mainPattern.renderObject,
                        this.groupId));
                    localPos.Add(pos);
                }
            }
        }

        #region IBHBulletUpdater Implementation

        public void InitUpdater() { BHUpdaterHelper.DefaultInit(this); }

        public void UpdateBullets(float deltaTime)
        {
            for (int i = 0; i < bullets.Count; i++)
                bullets[i].SetPosition(transform.TransformPoint(localPos[i]));
            if (bullets.Count == 0)
                Destroy(gameObject);
        }

        public void RemoveBullets()
        {
            BHUpdaterHelper.DefaultRemoveBullets(this, ref localPos);
        }

        public void DestroyUpdater()
        {
            BHUpdaterHelper.DefaultDestroy(this);
        }

        #endregion

        /// <summary>
        /// Set the main pattern of the updater, you should call this RIGHT AFTER its instantiation. <br/>
        /// </summary>
        public void SetPattern(BHPattern pattern)
        {
            if (!canSetPattern)
                BHExceptionRaiser.RaiseException(BHException.SetupAfterInitialization);
            mainPattern = pattern;
        }

        private void OnDestroy()
        {
            DestroyUpdater();
        }
    }
}