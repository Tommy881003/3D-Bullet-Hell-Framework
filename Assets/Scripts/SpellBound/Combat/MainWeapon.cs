using System.Collections;
using System.Collections.Generic;
using Bogay.SceneAudioManager;
using BulletHell3D;
using MessagePipe;
using SpellBound.BulletHell;
using SpellBound.Core;
using UnityEngine;
using VContainer;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace SpellBound.Combat
{
    public class MainWeapon : MonoBehaviour
    {
        [SerializeField]
        private BHPattern pattern;
        [SerializeField]
        private GameObject vfxPrefab;
        [SerializeField]
        private string sfxName;
        [field: SerializeField]
        public float ShootCooldownSeconds { get; private set; }
        [field: SerializeField]
        public int Cost { get; private set; }

        // TODO: maybe use DI to collect these config in one place?
        [SerializeField]
        private float distance;
        [SerializeField]
        private float speed;

        private System.Guid groupId;

        [Inject]
        private readonly ISubscriber<System.Guid, CollisionEvent> subscriber;
        [Inject]
        private readonly Character owner;

        private ISubscriber<Vector3> shootSubscriber;
        private IPublisher<Vector3> shootPublisher;

        private Vector3? shootDirection;
        public float ShootTimer { get; private set; }

        private void Start()
        {
            this.groupId = System.Guid.NewGuid();
            Debug.Log($"main weapon guid: {this.groupId}");
            this.subscriber.Subscribe(this.groupId, evt =>
            {
                var layer = 1 << evt.contact.layer;
                // TODO: make it usable on enemy (i.e. don't hard-coded enemy mask / owner)
                if ((layer & CollisionGroups.instance.enemyMask) != 0)
                {
                    Debug.Log("Hit enemy");
                    var controller = evt.contact.GetComponent<EnemyController>();
                    controller.character.Hurt(this.owner.Power.Value());
                }
            });

            (this.shootPublisher, this.shootSubscriber) = GlobalMessagePipe.CreateEvent<Vector3>();
            this.shootSubscriber.Subscribe(v =>
            {
                this.shootDirection = v;
            });

            this.ShootTimer = this.ShootCooldownSeconds;
            var ct = this.GetCancellationTokenOnDestroy();
            this.shootTask(ct).Forget();
        }

        private async UniTaskVoid shootTask(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (this.ShootTimer > 0f)
                {
                    this.ShootTimer = Mathf.Max(this.ShootTimer - Time.deltaTime, 0f);
                    await UniTask.Yield();
                    continue;
                }

                if (this.owner.MP < this.Cost)
                {
                    continue;
                }

                if (this.shootDirection != null)
                {
                    StartCoroutine(this.shootCoro(this.shootDirection.Value));
                    this.shootDirection = null;
                    this.ShootTimer = this.ShootCooldownSeconds;
                    this.owner.Cast(this.Cost);
                }
                await UniTask.Yield();
            }
        }

        public void Shoot(Vector3 forward)
        {
            this.shootPublisher.Publish(forward);
        }

        private IEnumerator shootCoro(Vector3 forward)
        {
            var go = new GameObject("Bullet");

            go.transform.position = transform.position + forward * distance;
            go.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            var updater = go.AddComponent<BHTransformVFXUpdater>();
            updater.groupId = this.groupId;
            updater.SetPattern(this.pattern);
            updater.vfxPrefab = this.vfxPrefab;

            SceneAudioManager.instance.PlayByName(this.sfxName);

            while (go != null)
            {
                go.transform.position += forward * (this.speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
