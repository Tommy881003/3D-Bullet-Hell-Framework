using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell3D
{
    public class BHTrailPool : MonoBehaviour
    {
        private static BHTrailPool _instance = null;
        public static BHTrailPool instance { get { return _instance; } }

        private const int maxTrailCount = 1000;
        private Queue<TrailRenderer> trQueue = new Queue<TrailRenderer>();

        void Awake() 
        {
            if(_instance == null)
                _instance = this;
            else
            {
                Destroy(gameObject);
                return;
            } 
        }

        // Start is called before the first frame update
        void Start()
        {
            for(int i = 0; i < maxTrailCount; i++)
            {
                GameObject go = new GameObject("Trail");
                go.transform.SetParent(transform);
                trQueue.Enqueue(go.AddComponent<TrailRenderer>());
                go.SetActive(false);
            }
        }

        public TrailRenderer RequestTrail()
        {
            if(trQueue.Count == 0)
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