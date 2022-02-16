using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell3D
{
    public class BHParticlePool : MonoBehaviour
    {
        private static BHParticlePool _instance = null;
        public static BHParticlePool instance { get { return _instance; } }

        // This gameobject needs a particle system attached to it.
        [SerializeField]
        private GameObject psPreset;

        private const int maxParticleCount = 2000;
        private Queue<ParticleSystem> psQueue = new Queue<ParticleSystem>();

        private void Awake() 
        {
            if(_instance == null)
                _instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start() 
        {
            for(int i = 0; i < maxParticleCount; i++)
            {
                GameObject go = Instantiate(psPreset, Vector3.zero, Quaternion.identity, transform);
                psQueue.Enqueue(go.GetComponent<ParticleSystem>());
                go.SetActive(false);
            }
        }

        public void RequestParticlePlay(Vector3 position)
        {
            //TODO: Add options to control the start color of the particle.
            if(psQueue.Count == 0)
                return;

            ParticleSystem ps = psQueue.Dequeue();
            ps.gameObject.transform.position = position;
            ps.gameObject.SetActive(true);
            ps.Play();
            psQueue.Enqueue(ps);
        }
    }
}