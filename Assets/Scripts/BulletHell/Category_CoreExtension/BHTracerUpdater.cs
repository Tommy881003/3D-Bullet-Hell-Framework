using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace BulletHell3D
{
    public sealed class BHTracerUpdater : MonoBehaviour, IBHBulletUpdater
    {
        private class TracerAddon
        {
            // Trail
            public TrailRenderer tr;
            // Direction of velocity
            public Vector3 forward;
            // Magnitude of velocity
            public float speed;

            // Can this bullet trace the player? (Prevent over-tracing.)
            // Over-tracing happens on bullets which have passes though player, which then turn around and strike back.
            public bool canTrace = true;
            // Is the bullet suffering from "fake" over-tracing?
            public bool fakeOverTrace = true;
            // Current continuous frames that have over-tracing.
            public int overTraceCounter = 0;
            // Maximum frames allowed for over-tracing.
            public const int overTraceFrameLimit = 3;

            // Time delayed for tracer bullet to start tracing.
            public float traceWaitTime;
        }

        // For prototyping purpose. (Will refactor later)
        private const float tracerSpeed = 20;
        private const float tracerTrailTime = 0.5f;
        private const float tracerRotateStrenth = 0.05f;
        private const float overTraceThreshold = 0.5f;

        [Inject]
        private BHTrailPool trailPool;
        [Inject]
        private Player player;

        public List<BHBullet> bullets { get; private set; } = new List<BHBullet>();
        private List<TracerAddon> addons = new List<TracerAddon>();

        private Queue<(BHBullet, TracerAddon)> addQueue = new Queue<(BHBullet, TracerAddon)>();

        #region IBHBulletUpdater Implementation

        public void InitUpdater() { BHUpdaterHelper.DefaultInit(this); }

        public void UpdateBullets(float deltaTime)
        {
            int length = bullets.Count;

            while (addQueue.Count > 0)
            {
                var pair = addQueue.Dequeue();
                bullets.Add(pair.Item1);
                addons.Add(pair.Item2);
            }

            /*
            // Current method of checking over-tracing is by comparing dot value with a threshold.
            // Which is basically checking whether the bullet is "steering sharply" or not.
            // This could lead to false-positive. (AKA the algorithm thinks the bullet is over-tracing, but its actually not.)
            // For example: the bullet is fired in the opposite direction of which it should go.
            // To cope with this, we need to observe the behaviour of tracing bullets.
            // There are two possible cases for a traceing bullet:
            //
            // 1. Steering lightly → Steering sharply ("actual" over-tracing)
            // 2. Steering sharply ("fake" over-tracing) → Steering lightly → Steering sharply ("actual" over-tracing)
            //
            // We want to prevent "fake" over-tracing, which only happens at the "early" stage of a tracing bullet.
            */
            for (int i = 0; i < length; i++)
            {
                bullets[i].SetPosition(bullets[i].position + addons[i].forward * addons[i].speed * deltaTime);
                addons[i].tr.transform.position = bullets[i].position;
                addons[i].traceWaitTime -= deltaTime;

                if (addons[i].traceWaitTime > 0 || !addons[i].canTrace)
                    continue;

                Vector3 bulletToPlayer = (player.transform.position - bullets[i].position).normalized;

                float dot = Vector3.Dot(addons[i].forward, bulletToPlayer);

                if (addons[i].fakeOverTrace && dot >= overTraceThreshold)
                    addons[i].fakeOverTrace = false;

                addons[i].overTraceCounter = (!addons[i].fakeOverTrace && dot < overTraceThreshold) ? ++addons[i].overTraceCounter : 0;
                if (addons[i].overTraceCounter >= TracerAddon.overTraceFrameLimit)
                {
                    addons[i].canTrace = false;
                    continue;
                }

                addons[i].forward = Vector3.Slerp(addons[i].forward, bulletToPlayer, tracerRotateStrenth);
            }
        }

        public void RemoveBullets()
        {
            int length = bullets.Count;
            for (int i = 0; i < length; i++)
            {
                if (!bullets[i].isAlive)
                    trailPool.ReturnTrail(addons[i].tr);
            }
            BHUpdaterHelper.DefaultRemoveBullets<TracerAddon>(this, ref addons);
        }

        public void DestroyUpdater()
        {
            BHUpdaterHelper.DefaultDestroy(this);
        }

        #endregion

        // Policy: If there's short storage of trail renderers, dont spawn tracer bullets.
        public void AddBullet(BHRenderObject renderObject, Vector3 position, Vector3 forward, float speed = 20, float timeDelay = 0)
        {
            if (trailPool.stockCount == 0)
                return;
            CreateTracerBullet(renderObject, position, forward, speed, timeDelay);
        }

        // Policy: If there's short storage of trail renderers, dont spawn tracer bullets.
        public void AddPattern(BHPattern pattern, Vector3 position, Vector3 forwardAxis, float angleInDeg, float speed = 20, float timeDelay = 0)
        {
            int bulletCount = pattern.bullets.Count;
            if (trailPool.stockCount < bulletCount)
                return;

            // Orthonormal vectors in relative coordinate.
            // Forward: z-axis
            // Right: x-axis
            // Up: y-axis
            Vector3 relativeForward;
            Vector3 relativeRight;
            Vector3 relativeUp;

            relativeForward = forwardAxis.normalized;
            BHHelper.LookRotationSolver(forwardAxis, angleInDeg, out relativeUp, out relativeRight);

            // Ok, here comes the big brain time. We want the forward vector of a bullet to be the position of the bullet itself.
            // For example: if a bullet is in (1,1,0) in the bullet pattern, it's forward vector will also be (1,1,0).
            // But we also have to convert it into the relative coordinate. (The three relative vectors above.)
            // So that means we need a matrix multiplication. (A rotation matrix)
            // Here comes the magic:
            // Let rRight = (x1,y1,z1), rUp = (x2,y2,z2), rFoward = (x3,y3,z3), original bullet forward vector = (x,y,z)
            //
            //            ┌ x1 x2 x3 ┐
            // The matrix | y1 y2 y3 | IS the exact rotation matrix we need.
            //            └ z1 z2 z3 ┘
            //
            //                          ┌ x1 x2 x3 ┐┌ x ┐
            // So the answer we need is | y1 y2 y3 || y | = x * rRight + y * rUp + z * rFoward
            //                          └ z1 z2 z3 ┘└ z ┘
            //
            // Which is this thing: ────────────────────────────────────────┐
            foreach (Vector3 vector in pattern.bullets)                   // |
            {                                                            // ↓ 
                CreateTracerBullet(pattern.renderObject, position, vector.x * relativeRight +
                                                                   vector.y * relativeUp +
                                                                   vector.z * relativeForward,
                                                                   speed,
                                                                   timeDelay
                                  );
            }
        }

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
        private void CreateTracerBullet(BHRenderObject renderObject, Vector3 position, Vector3 forward, float traceSpeed, float timeDelay)
        {
            BHBullet newBullet = new BHBullet(position, renderObject);
            TracerAddon newAddon = new TracerAddon()
            {
                tr = trailPool.RequestTrail(),
                forward = forward.normalized,
                speed = traceSpeed,
                traceWaitTime = timeDelay
            };
            newAddon.tr.transform.position = position;
            newAddon.tr.startWidth = renderObject.radius * 2;
            newAddon.tr.endWidth = 0;
            newAddon.tr.time = tracerTrailTime;
            addQueue.Enqueue((newBullet, newAddon));
        }

        private void OnDestroy()
        {
            DestroyUpdater();
        }
    }
}