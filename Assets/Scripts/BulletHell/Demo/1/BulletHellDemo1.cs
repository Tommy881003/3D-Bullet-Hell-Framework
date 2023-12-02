using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell3D;
using VContainer;
using MessagePipe;
using System;
using Cysharp.Threading.Tasks.Triggers;

public class BulletHellDemo1 : MonoBehaviour
{
    [SerializeField]
    private bool showDemo = false;
    [SerializeField, Range(0, 1)]
    private float lookAtStrenth;

    [Space(10)]
    [SerializeField]
    private BHCustomUpdater demoUpdater;
    [SerializeField]
    private BHRenderObject demoRenderObj;
    [SerializeField]
    private float burstGap = 0.1f;
    [SerializeField]
    private int burstCount = 6;
    [SerializeField]
    private float burstRotate = 11;

    private float timer = 0;
    private float angle = 0;

    [Space(10)]
    [SerializeField]
    private BHPattern tracePattern;
    [SerializeField]
    private float traceSpeed = 25;
    [SerializeField]
    private float traceDelay;
    [SerializeField]
    private float traceBurstGap = 2.5f;

    private float traceTimer = 0;

    private BHTracerUpdater tracerUpdater;

    [Inject]
    private Player player;
    [Inject]
    private ISubscriber<System.Guid, CollisionEvent> subscriber;
    [Inject]
    private Func<GameObject, BHTracerUpdater> createUpdater;

    void Start()
    {
        this.tracerUpdater = this.createUpdater(gameObject);
        Vector3 toPlayer = player.transform.position - transform.position;
        Vector3 newFoward = new Vector3(toPlayer.x, 0, toPlayer.z);
        transform.rotation = Quaternion.LookRotation(newFoward, Vector3.up);
        this.demoUpdater.groupId = System.Guid.NewGuid();
        this.subscriber.Subscribe(this.demoUpdater.groupId.Value, evt =>
        {
            var player = evt.contact.GetComponentInChildren<PlayerController>();
            if (player != null)
            {
                player.Character.Hurt(1);
            }
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            showDemo = !showDemo;
    }

    private void FixedUpdate()
    {
        Vector3 toPlayer = player.transform.position - transform.position;
        Vector3 newFoward = Vector3.Slerp(transform.forward, new Vector3(toPlayer.x, 0, toPlayer.z), lookAtStrenth);
        transform.rotation = Quaternion.LookRotation(newFoward, Vector3.up);

        if (showDemo)
        {
            if (timer <= 0)
            {
                float startAngle = angle * Mathf.Deg2Rad;
                float deltaAngle = Mathf.PI * 2 / burstCount;
                for (int i = 0; i < burstCount; i++)
                {
                    float finalAngle = startAngle + deltaAngle * i;
                    demoUpdater.AddBullet(
                        demoRenderObj,
                        transform.position,
                        Mathf.Cos(finalAngle) * transform.right + Mathf.Sin(finalAngle) * transform.up,
                        this.demoUpdater.groupId
                    );
                }
                timer += burstGap;
                angle = (angle + burstRotate) % 360;
            }

            if (traceTimer <= 0)
            {
                Debug.Assert(this.tracerUpdater != null);
                this.tracerUpdater.AddPattern(tracePattern, transform.position, transform.forward, 0, traceSpeed, traceDelay);
                traceTimer += traceBurstGap;
            }

            timer -= Time.fixedDeltaTime;
            traceTimer -= Time.fixedDeltaTime;
        }
    }
}
