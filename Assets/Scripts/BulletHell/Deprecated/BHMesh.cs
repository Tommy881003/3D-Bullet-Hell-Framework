using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*using BulletHell3D.Base;

namespace BulletHell3D
{
    public class BHMesh : GameBehaviour
    {
        [SerializeField]
        private Mesh mesh;
        private List<Triangle> triangles = new List<Triangle>();

        private Vector3 lastPosition;
        private Vector3 lastRotation;
        private Vector3 lastScale;

        public override void GameStart()
        {
            int layer = gameObject.layer;
            for(int i = 0; i < mesh.triangles.Length; i += 3)
                triangles.Add(new Triangle());
            GenerateTriangles();
            foreach(Triangle tri in triangles)
                RaycastManager.Add(tri, layer);

            lastPosition = transform.position;
            lastRotation = transform.rotation.eulerAngles;
            lastScale = transform.lossyScale;
        }

        public override void GameFixedUpdate() 
        {
            if(lastPosition != transform.position || lastRotation != transform.rotation.eulerAngles || lastScale != transform.lossyScale)
                GenerateTriangles();
            lastPosition = transform.position;
            lastRotation = transform.rotation.eulerAngles;
            lastScale = transform.lossyScale;
        }

        private void GenerateTriangles()
        {
            for(int i = 0; i < mesh.triangles.Length; i += 3)
            {
                triangles[i / 3].SetVertices
                (
                    Vec3.FromUnity(transform.TransformPoint(mesh.vertices[mesh.triangles[i + 0]])),
                    Vec3.FromUnity(transform.TransformPoint(mesh.vertices[mesh.triangles[i + 1]])),
                    Vec3.FromUnity(transform.TransformPoint(mesh.vertices[mesh.triangles[i + 2]]))
                );
            }
        }

        private void OnDrawGizmos() 
        {
            foreach(Triangle tri in triangles)
            {
                Gizmos.DrawWireCube
                (
                    new Vector3
                    (
                        (tri.bound.minX + tri.bound.maxX) / 2,
                        (tri.bound.minY + tri.bound.maxY) / 2,
                        (tri.bound.minZ + tri.bound.maxZ) / 2
                    ),
                    new Vector3
                    (
                        tri.bound.maxX - tri.bound.minX,
                        tri.bound.maxY - tri.bound.minY,
                        tri.bound.maxZ - tri.bound.minZ
                    )
                );
            }    
        }
    }
}*/