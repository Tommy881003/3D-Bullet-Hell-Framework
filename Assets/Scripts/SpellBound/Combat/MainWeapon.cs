using System.Collections;
using System.Collections.Generic;
using Bogay.SceneAudioManager;
using BulletHell3D;
using MessagePipe;
using SpellBound.BulletHell;
using UnityEngine;
using VContainer;

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
        // TODO: maybe use DI to collect these config in one place?
        [SerializeField]
        private float distance;
        [SerializeField]
        private float speed;

        private System.Guid groupId;
        [Inject]
        private readonly ISubscriber<System.Guid, CollisionEvent> subscriber;

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
                    controller.character.Hurt(30);
                }
            });
        }

        public void Shoot(Vector3 forward)
        {
            StartCoroutine(this.shootCoro(forward));
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
