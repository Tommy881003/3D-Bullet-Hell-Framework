using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell3D
{
    // Memo: Behaviour of BHCustomUpdater
    // 1. Can add bullets as long as it's not destroy.
    // 2. Does not auto-destroy (even when it has no bullets), must call Destroy manually.
    public class BHCustomUpdater : MonoBehaviour, IBHBulletUpdater
    {
        public List<BHBullet> bullets { get; protected set; } = new List<BHBullet>();
        protected List<Vector3> forwards = new List<Vector3>();
        private Queue<BHBullet> addQueue = new Queue<BHBullet>();

        void Start()
        {
            InitUpdater();
        }

        #region IBHBulletUpdater Implementation

        public void InitUpdater() { BHUpdaterHelper.DefaultInit(this); }

        public void UpdateBullets(float deltaTime)
        {
            while(addQueue.Count > 0)
            {
                var bullet = addQueue.Dequeue();
                bullets.Add(bullet);
            }

            UpdateCustom(deltaTime);
        }        

        public virtual void RemoveBullets()
        {
            BHUpdaterHelper.DefaultRemoveBullets(this, ref forwards);
        }

        public void DestroyUpdater()
        {
            BHUpdaterHelper.DefaultDestroy(this);
        }

        #endregion

        /*
        // We use a queue to store the bullet instead of add it into the list immediately to prevent list length mismatch in BHManager.
        // These will be added later into list in the UpdateBullet function.
        //
        // "Why does the mismatch happens anyway?" you ask? Well, let's take a look at order of execution in BHManager.
        // The simplified execution order looks like this:
        //  
        // [ 1. Gather raycast result & check bullet collisions. ]
        //  ↓                                                   ↑
        // [ 2. Update bullets. ]                              loop
        //  ↓                                                   |
        // [ 3. Assign and kick raycast jobs. ] ----------------┘
        //
        // [ 4. (By another script) Add bullets into an Updatable IMMEDIATELY ] ← Would cause error in step 1.
        //
        // In step 1 the number of raycasts should equals to the number of total bullets. (One bullet, one raycast.)
        // Step 2 removes invalid bullets, and updates positions of remaining bullets.
        // Step 3 creates the same amount of raycast commands of the remaining bullets (step 2), then dispatchs the jobs.
        // On its own, these three steps should not create any error. However, consider step 4 from another script.
        // Step 4 alter the total amount of bullets AFTER the jobs are dispatched. (More precisely, AFTER step 3, BEFORE step 1 in new loop.)
        // Now the number of raycast does not equals to the number of total bullets anymore, thus create error in step 1.
        //
        // So, after knowing the cause of error, the solution would be simple.
        // When adding bullets (step 4), we delayed the request till the collision checking process is completed. (AFTER step 1 is done)
        */
        public void AddBullet(BHRenderObject renderObject, Vector3 position, Vector3 forward)
        {
            addQueue.Enqueue(new BHBullet(position, renderObject));
            forwards.Add(forward);
        }

        protected virtual void UpdateCustom(float deltaTime) {}

        private void OnDestroy() 
        {
            DestroyUpdater();    
        }
    }
}