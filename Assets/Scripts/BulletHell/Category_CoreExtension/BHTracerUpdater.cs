using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell3D
{
    public sealed class BHTracerUpdater : MonoBehaviour, IBHUpdatable
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
            // Over-tracing are those bullets which have passes though player, which then turn around and strike back.
            public bool canTrace = true;
            // Is the bullet suffering from "fake" over-tracing?
            public bool fakeOverTrace = true; 
            // Current continuous frames that have over-tracing.
            public int overTraceCounter = 0;
            // Maximum frames allowed for over-tracing.
            public const int overTraceFrameLimit = 2;

            // TODO: Seems to miss something...?
        }

        private static BHTracerUpdater _instance = null;
        public static BHTracerUpdater instance
        {
            get
            {
                if(_instance == null)
                {
                    var manager = BHManager.instance;
                    if(manager == null)
                        BHExceptionRaiser.RaiseException(BHException.ManagerNotFound);
                    _instance = manager.gameObject.AddComponent<BHTracerUpdater>();
                    _instance.InitUpdatable();
                }
                return _instance;
            }
        }

        // For prototyping purpose. (Will refactor later)
        private const float tracerSpeed = 20;
        private const float tracerTrailTime = 0.5f;
        private const float tracerRotateStrenth = 0.05f;

        private BHTrailPool trailPool;
        private Player player;

        public List<BHBullet> bullets { get; private set; } = new List<BHBullet>();
        private List<TracerAddon> addons = new List<TracerAddon>();

        private Queue<(BHBullet, TracerAddon)> addQueue = new Queue<(BHBullet, TracerAddon)>();

        void Awake() 
        { 
            trailPool = BHTrailPool.instance;
            player = DependencyContainer.GetDependency<Player>() as Player; 
        }

        #region IBHUpdatable Implementation

        public void InitUpdatable() { BHUpdatableHelper.DefaultInit(this); }

        public void UpdateBullets(float deltaTime)
        {
            int length = bullets.Count;
            
            while(addQueue.Count > 0)
            {
                var pair = addQueue.Dequeue();
                bullets.Add(pair.Item1);
                addons.Add(pair.Item2);
            }

            for(int i = 0; i < length; i++)
            {
                bullets[i].SetPosition(bullets[i].position + addons[i].forward * addons[i].speed * deltaTime);
                addons[i].tr.transform.position = bullets[i].position;
                if(!addons[i].canTrace)
                    continue;
                
                Vector3 bulletToPlayer = (player.transform.position - bullets[i].position).normalized;
                
                // Shit, this is not working...
                float dot = Vector3.Dot(addons[i].forward, bulletToPlayer);
                if(addons[i].fakeOverTrace && dot > 0)
                    addons[i].fakeOverTrace = false;

                addons[i].overTraceCounter = (!addons[i].fakeOverTrace && dot < 0)? addons[i].overTraceCounter++ : 0;
                if(addons[i].overTraceCounter >= TracerAddon.overTraceFrameLimit)
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
            for(int i = 0; i < length; i++)
            {
                if(!bullets[i].isAlive)
                    trailPool.ReturnTrail(addons[i].tr);
            }
            BHUpdatableHelper.DefaultRemoveBullets<TracerAddon>(this, ref addons);
        }

        public void DestroyUpdatable() 
        { 
            BHUpdatableHelper.DefaultDestroy(this);
        }

        #endregion

        // Policy: If there's short storage of trail renderers, dont spawn tracer bullets.
        public void AddBullet(BHRenderObject renderObject, Vector3 position, Vector3 forward) 
        {
            if(trailPool.stockCount == 0)
                return;
            CreateTracerBullet(renderObject, position, forward);
        }

        // Policy: If there's short storage of trail renderers, dont spawn tracer bullets.
        public void AddPattern(BHPattern pattern, Vector3 position, Vector3 forwardAxis, float angleInDeg) 
        {
            int bulletCount = pattern.bullets.Count;
            if(trailPool.stockCount < bulletCount)
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
            foreach(Vector3 vector in pattern.bullets)                   // |
            {                                                            // ↓ 
                CreateTracerBullet(pattern.renderObject, position, vector.x * relativeRight + 
                                                                   vector.y * relativeUp + 
                                                                   vector.z * relativeForward
                                  );
            }
        }

        // We use a queue to store the bullet instead of add it into the list immediately to prevent list length mismatch in BHManager.
        // These will be added later into list in the UpdateBullet function.
        private void CreateTracerBullet(BHRenderObject renderObject, Vector3 position, Vector3 forward)
        {
            BHBullet newBullet = new BHBullet(position, renderObject);
            TracerAddon newAddon = new TracerAddon() 
            { 
                tr = trailPool.RequestTrail(),
                forward = forward.normalized,
                speed = tracerSpeed
            };
            newAddon.tr.transform.position = position;
            newAddon.tr.startWidth = renderObject.radius * 2;
            newAddon.tr.endWidth = 0;
            newAddon.tr.time = tracerTrailTime;
            addQueue.Enqueue((newBullet,newAddon));
        }

        private void OnDestroy() 
        {
            DestroyUpdatable(); 
            _instance = null; 
        }
    }
}