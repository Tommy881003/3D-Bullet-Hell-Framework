using System.Collections;
using System.Collections.Generic;
using BulletHell3D;
using UnityEngine;

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

        public void Shoot(Vector3 forward)
        {
            StartCoroutine(this.shootCoro(forward));
        }

        private IEnumerator shootCoro(Vector3 forward)
        {
            var go = new GameObject("PlayerBullet");

            go.transform.position = transform.position + forward * distance;
            go.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            go.AddComponent<BHTransformUpdater>().SetPattern(this.pattern);

            while (go != null)
            {
                go.transform.position += forward * (this.speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
