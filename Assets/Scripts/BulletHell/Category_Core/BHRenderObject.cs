using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell3D
{
    [CreateAssetMenu(menuName = "Bullet Hell/Render Object")]
    public class BHRenderObject : ScriptableObject
    {
        [SerializeField]
        private Mesh _mesh;
        public Mesh mesh { get { return _mesh; } }
        [SerializeField]
        private Material _material;
        public Material material { get { return _material; } }
        [SerializeField]
        private float _radius;
        public float radius { get { return _radius; } }
    }
}