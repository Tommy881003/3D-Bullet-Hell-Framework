using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell3D
{
    [CreateAssetMenu(menuName = "Bullet Hell/Pattern")]
    public class BHPattern : ScriptableObject
    {
        [SerializeField]
        private BHRenderObject _renderObject;
        public BHRenderObject renderObject { get { return _renderObject; } }
        
        // Currently we assume that there will only be a single type of render object in a pattern.
        // So, a List<Vector3> should be enough.
        // TODO: Fuck I forgot tuple cannot be serialized. 
        [SerializeField]
        private List<Vector3> _bullets = new List<Vector3>();
        public List<Vector3> bullets { get { return _bullets; } }

        public void AddBullets(List<Vector3> vectors)
        {
            foreach(Vector3 vector in vectors)
                _bullets.Add(vector);
        }

        public void Clear()
        {
            _bullets.Clear();
        }
    }
}