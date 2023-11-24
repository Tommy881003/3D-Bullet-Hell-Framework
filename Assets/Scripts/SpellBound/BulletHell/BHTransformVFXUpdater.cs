using System;
using System.Collections.Generic;
using UnityEngine;
using BulletHell3D;

namespace SpellBound.BulletHell
{
    public class BHTransformVFXUpdater : MonoBehaviour, IBHBulletUpdater
    {
        public Guid groupId;
        public List<BHBullet> bullets { get; protected set; } = new List<BHBullet>();
        private List<GameObject> vfxList = new List<GameObject>();
        protected List<Vector3> localPos = new List<Vector3>();

        [SerializeField]
        public GameObject vfxPrefab;

        private bool canSetPattern = true;
        [SerializeField]
        protected BHPattern mainPattern;

        void Start()
        {
            canSetPattern = false;
            InitUpdater();

            transform.localScale = Vector3.zero;
            if (mainPattern != null)
            {
                foreach (Vector3 pos in mainPattern.bullets)
                {
                    bullets.Add(new BHBullet(
                        transform.TransformPoint(pos),
                        mainPattern.renderObject,
                        this.groupId));
                    vfxList.Add(Instantiate(this.vfxPrefab, pos, Quaternion.identity));
                    localPos.Add(pos);
                }
            }
        }

        #region IBHBulletUpdater Implementation

        public void InitUpdater() { BHUpdaterHelper.DefaultInit(this); }

        public void UpdateBullets(float deltaTime)
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                Vector3 bulletPos = transform.TransformPoint(localPos[i]);
                bullets[i].SetPosition(bulletPos);
                vfxList[i].transform.position = bulletPos;
                if (bullets[i].delta.sqrMagnitude > float.Epsilon)
                    vfxList[i].transform.forward = bullets[i].delta;
            }
            if (bullets.Count == 0)
                Destroy(gameObject);
        }

        public void RemoveBullets()
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                if (!bullets[i].isAlive)
                {
                    bullets.RemoveAt(i);
                    localPos.RemoveAt(i);
                    Destroy(vfxList[i]);
                    vfxList.RemoveAt(i);
                }
            }
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