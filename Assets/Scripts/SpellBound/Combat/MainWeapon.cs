using System.Collections;
using System.Collections.Generic;
using BulletHell3D;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace SpellBound.Combat
{
    public class MainWeapon : MonoBehaviour
    {
        [SerializeField]
        private BHPattern pattern;
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
                if (((1 << evt.contact.layer) & CollisionGroups.instance.obstacleMask) != 0)
                {
                    Debug.Log("Hit obstacle");
                }
            });
        }

        public void Shoot(Vector3 forward)
        {
            StartCoroutine(this.shootCoro(forward));
        }

        private IEnumerator shootCoro(Vector3 forward)
        {
            var go = new GameObject("PlayerBullet");

            go.transform.position = transform.position + forward * distance;
            go.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            var updater = go.AddComponent<BHTransformUpdater>();
            updater.groupId = this.groupId;
            updater.SetPattern(this.pattern);


            while (go != null)
            {
                go.transform.position += forward * (this.speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
