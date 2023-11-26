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

        [SerializeField]
        private SkillTriggerSetting skillTriggerSetting;
        private SkillTrigger<Vector3> skillTrigger;

        // TODO: maybe use DI to collect these config in one place?
        [SerializeField]
        private float distance;
        [SerializeField]
        private float speed;

        private System.Guid groupId;
        public float ShootCooldownSeconds { get => this.skillTrigger.Setting.CooldownSeconds; }
        public int Cost { get => this.skillTrigger.Setting.Cost; }
        public float ShootTimer { get => this.skillTrigger.TriggerTimer; }

        [Inject]
        private readonly ISubscriber<System.Guid, CollisionEvent> subscriber;
        [Inject]
        private readonly Character owner;
        [Inject]
        private readonly CollisionGroups collisionGroups;

        private void Start()
        {
            this.groupId = System.Guid.NewGuid();
            Debug.Log($"main weapon guid: {this.groupId}");
            this.subscriber.Subscribe(this.groupId, evt =>
            {
                var layer = 1 << evt.contact.layer;
                // TODO: make it usable on enemy (i.e. don't hard-coded enemy mask / owner)
                if ((layer & this.collisionGroups.enemyMask) != 0)
                {
                    Debug.Log("Hit enemy");
                    var controller = evt.contact.GetComponent<EnemyController>();
                    controller.character.Hurt(this.owner.Power.Value());
                }
            });

            this.skillTrigger = new SkillTrigger<Vector3>(
                this.skillTriggerSetting,
                this.owner
            );
            this.skillTrigger.Subscribe(fwd => StartCoroutine(this.shootCoro(fwd)));
            var ct = this.GetCancellationTokenOnDestroy();
            this.skillTrigger.Start(ct);
        }

        public void Shoot(Vector3 forward)
        {
            this.skillTrigger.Trigger(forward);
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
