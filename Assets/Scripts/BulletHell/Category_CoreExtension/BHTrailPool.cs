using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell3D
{
    public class BHTrailPool : MonoBehaviour
    {
        private const int maxTrailCount = 1000;
        private Queue<TrailRenderer> trQueue = new Queue<TrailRenderer>();

        // This gameobject needs a trail renderer attached to it.
        [SerializeField]
        private GameObject trPreset;

        public int stockCount { get { return trQueue.Count; } }

        void Start()
        {
            for (int i = 0; i < maxTrailCount; i++)
            {
                GameObject go = Instantiate(trPreset);
                go.transform.SetParent(transform);
                trQueue.Enqueue(go.GetComponent<TrailRenderer>());
                go.SetActive(false);
            }
        }

        public TrailRenderer RequestTrail()
        {
            if (trQueue.Count == 0)
                return null;
            TrailRenderer tr = trQueue.Dequeue();
            tr.gameObject.SetActive(true);
            return tr;
        }

        public void ReturnTrail(TrailRenderer tr)
        {
            tr.gameObject.SetActive(false);
            trQueue.Enqueue(tr);
        }
    }
}