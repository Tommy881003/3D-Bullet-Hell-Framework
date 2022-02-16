using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell3D
{
    // Memo: Behaviour of BHTransformUpdater
    // 1. Can add bullets as long as it's not destroy.
    // 2. Does not auto-destroy (even when it has no bullets), must call Destroy manually.
    public class BHSimpleUpdater : MonoBehaviour, IBHUpdatable
    {
        public List<BHBullet> bullets { get; protected set; } = new List<BHBullet>();
        protected List<Vector3> fowards = new List<Vector3>();

        public void Awake()
        {
            InitUpdatable();
        }

        #region IBHUpdatable Implementation

        public void InitUpdatable() { BHUpdatableHelper.DefaultInit(this); }

        public void UpdateBullets(float deltaTime)
        {
            BulletUpdate(deltaTime);
        }        

        public void RemoveBullets()
        {
            BulletRemove();
        }

        public void DestroyUpdatable()
        {
            BHUpdatableHelper.DefaultDestroy(this);
        }

        #endregion

        public virtual void AddBullet(BHRenderObject renderObject, Vector3 position, Vector3 forward)
        {
            bullets.Add(new BHBullet(position, renderObject));
            fowards.Add(forward);
        }

        protected virtual void BulletUpdate(float deltaTime) {}

        protected virtual void BulletRemove()
        {
            BHUpdatableHelper.DefaultRemoveBullets(this, ref fowards);
        }

        private void OnDestroy() 
        {
            DestroyUpdatable();    
        }
    }
}