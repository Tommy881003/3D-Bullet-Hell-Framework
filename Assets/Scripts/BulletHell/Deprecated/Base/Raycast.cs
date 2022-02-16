using System;
using System.Collections.Generic;
using UnityEngine;

/*namespace BulletHell3D.Base
{
    public static class RaycastManager
    {
        private class AABBNode
        {
            //This should always be positive.
            public const float margin = 0.2f; 

            public Triangle triangle { get; private set; } = null;

            /// <summary>
            /// The bounding box, this property has two possible meaning: <br/>
            /// In leaf node: Represent the "fat" AABB, call <see cref="AABBNode.triangle">triangle.bound</see> to get the actual AABB. <br/>
            /// In non-leaf node: Represent the actual AABB.
            /// </summary>
            public AABB bound { get; private set; }
            
            public AABBNode parentNode { get; private set; } = null;
            public AABBNode leftNode { get; private set; } = null;
            public AABBNode rightNode { get; private set; } = null;
            public bool isLeaf { get; private set; }
            public bool isRoot { get; private set; }

            /// <summary>
            /// Create a leaf AABBNode.
            /// </summary>
            /// <param name="tri">The triangle.</param>
            /// <param name="parent">The parent of this node.</param>
            public AABBNode(Triangle tri, AABBNode parent)
            {
                if(tri == null)
                    throw new Exception("Cannot assign a null triangle when creating a leaf AABBNode!");
                
                triangle = tri;

                bound = new AABB
                (
                    tri.bound.minX - margin,
                    tri.bound.maxX + margin,
                    tri.bound.minY - margin,
                    tri.bound.maxY + margin,
                    tri.bound.minZ - margin,
                    tri.bound.maxZ + margin
                );
                isLeaf = true;
                SetNodes(null, null, parent);
            }

            /// <summary>
            /// Create a non-leaf AABBNode.
            /// </summary>
            /// <param name="aabb">The bounding box.</param>
            /// <param name="left">The left child of this node.</param>
            /// <param name="right">The right child of this node.</param>
            /// <param name="parent">The parent of this node.</param>
            public AABBNode(AABBNode left, AABBNode right, AABBNode parent)
            {
                SetNodes(left, right, parent);
                isLeaf = false;
            }

            /// <summary>
            /// Update the node links of the node, this will also trigger AABB refitting for non-leaf node.
            /// </summary>
            /// <param name="left">The new left child node.</param>
            /// <param name="right">The new right child node.</param>
            /// <param name="parent">The new parent node.</param>
            public void SetNodes(AABBNode left, AABBNode right, AABBNode parent)
            {
                //If violates the rules of AABBNode, throw exception.
                //Both child nodes must be both null or non-null.
                if((left == null && right != null) || (left != null && right == null))
                    throw new Exception("Child nodes of an AABBNode must both be null or non-null!");
                if(parent == this || left == this || right == this)
                    throw new Exception("Cannot assign self as parent / left node / right node!");

                leftNode = left;
                rightNode = right;
                parentNode = parent;
                isLeaf = (leftNode == null && rightNode == null);
                isRoot = (parentNode == null);

                //If a node "turn into" a non-leaf node (usually happens when merging nodes), set triangle to null.
                if(!isLeaf && triangle != null)
                    triangle = null;

                //If this is a non-leaf node (aka: child nodes are not null), create an AABB based on the union of childrens' AABB.
                if(!isLeaf)
                    bound = AABB.Union(leftNode.bound, rightNode.bound);
            }
        }

        private class AABBTree
        {
            public float leafCount { get; private set; } = 0;
            public AABBNode root { get; private set; } = null;
            private Dictionary<Triangle, AABBNode> nodeMap = new Dictionary<Triangle, AABBNode>();
            private Queue<Triangle> reinsertNodeQueue = new Queue<Triangle>();
            
            /// <summary>
            /// Create an AABBNode with a given triangle and add it into the tree.
            /// </summary>
            /// <param name="tri">The added triangle.</param>
            /// <returns>ID of the newly created AABBNode.</returns>
            public void AddNode(Triangle tri)
            {
                //Create a new leaf node.
                AABBNode newNode = new AABBNode(tri, null);
                leafCount++;

                nodeMap.Add(tri,newNode);
                
                //If this is the first node, assigned it as root and return immediately.
                //No need to find sibling or refit ancestors. (Since there are no siblings or ancestors.)
                if(root == null)
                {
                    root = newNode;
                    return;
                }

                //Find best sibling to combine with.
                AABBNode bestSibling = FindBestSibling(newNode);
                //Merge.
                AABBNode siblingCopy = (bestSibling.isLeaf)? new AABBNode(bestSibling.triangle, bestSibling) : 
                                                             new AABBNode(bestSibling.leftNode, bestSibling.rightNode, bestSibling);
                
                //If we're going to merge a leaf node (aka: bestSibling is leaf), update the node map.
                //Since that leaf node will become a non-leaf node after merging.
                if(bestSibling.isLeaf)
                    nodeMap[bestSibling.triangle] = siblingCopy;
                
                newNode.SetNodes(null,null,bestSibling);
                bestSibling.SetNodes(newNode, siblingCopy, bestSibling.parentNode);

                //Refit all ancestors.
                AABBNode current = bestSibling.parentNode;
                while(current != null)
                {
                    current.SetNodes(current.leftNode, current.rightNode, current.parentNode);
                    current = current.parentNode;
                }   

                return;
            }

            /// <summary>
            /// Remove an AABBNode from the tree.
            /// </summary>
            /// <param name="tri">Triangle of the AABBNode.</param>
            public void RemoveNode(Triangle tri)
            {
                AABBNode node = null;
                if(!nodeMap.ContainsKey(tri))
                    throw new Exception("Triangle does not exists in AABB tree.");
                node = nodeMap[tri];

                AABBNode parent = node.parentNode;

                //Node is not root.
                if(parent != null)
                {
                    AABBNode grandparent = parent.parentNode;
                    AABBNode sibling = (parent.leftNode == node)? parent.rightNode : parent.leftNode;

                    //If there is a grandparent.
                    if(grandparent != null)
                    {
                        //Assign sibling and the sibling of parent as the child of grandparent.
                        sibling.SetNodes(sibling.leftNode, sibling.rightNode, grandparent);
                        if(grandparent.leftNode == parent)
                            grandparent.SetNodes(sibling, grandparent.rightNode, grandparent.parentNode);
                        else
                            grandparent.SetNodes(grandparent.leftNode, sibling, grandparent.parentNode);
                    }
                    //If there is no grandparent.
                    else
                    {
                        //Assign sibling as root.
                        sibling.SetNodes(sibling.leftNode, sibling.rightNode, null);
                        root = sibling;
                    }

                    node.SetNodes(null,null,null);
                    parent.SetNodes(null,null,null);
                }
                //Node is root.
                else
                    root = null;

                //Remove node from dictionary.
                leafCount--;
                nodeMap.Remove(tri);
            }

            /// <summary>
            /// Update the tree by re-inserting dirty nodes. <br/> 
            /// This should be called right after objects update their positions.
            /// </summary>
            public void UpdateTree()
            {
                foreach(Triangle tri in nodeMap.Keys)
                {
                    AABBNode node = nodeMap[tri];
                    if(!tri.bound.Inside(node.bound))
                        reinsertNodeQueue.Enqueue(tri);
                }

                Triangle dirtyTriangle;

                //Process every dirty node.
                while(reinsertNodeQueue.Count > 0)
                {
                    dirtyTriangle = reinsertNodeQueue.Dequeue();
                    //If a node has been destoryed, simply skip it.
                    if(!nodeMap.ContainsKey(dirtyTriangle))
                        continue;
                    
                    //Remove and re-insert the AABB.
                    RemoveNode(dirtyTriangle);
                    AddNode(dirtyTriangle);
                }
            }

            /// <summary>
            /// Check whether the tree contains the given triangle.
            /// </summary>
            /// <param name="tri">The given triangle.</param>
            public bool Contains(Triangle tri)
            {
                return nodeMap.ContainsKey(tri);
            }

            /// <summary>
            /// Check and return every triangle that has its bound intersect with the given aabb.
            /// </summary>
            /// <param name="bound">The given aabb.</param>
            /// <returns>A list of triangles which their bound intersects with the aabb.</returns>
            public List<Triangle> CheckIntersections(AABB bound)
            {
                List<Triangle> result = new List<Triangle>();
                Queue<AABBNode> candidates = new Queue<AABBNode>();
                AABBNode candidate = null;

                //If the root is null (aka: there's no node in the tree), or the given bound does not intersect with root's bound. Return immediately.
                if(root == null) return result;
                if(!bound.IntersectWith(root.bound)) return result;

                candidates.Enqueue(root);

                while(candidates.Count > 0)
                {
                    candidate = candidates.Dequeue();
                    //If the given bound intersects with the triangle's bound, add the triangle into the list.
                    if(candidate.isLeaf && bound.IntersectWith(candidate.triangle.bound))
                        result.Add(candidate.triangle);
                    else
                    {
                        //Check whether the bound of child node intersect with the given bound.
                        if(bound.IntersectWith(candidate.leftNode.bound))
                            candidates.Enqueue(candidate.leftNode);
                        if(bound.IntersectWith(candidate.rightNode.bound))
                            candidates.Enqueue(candidate.rightNode);
                    }
                }
                return result;
            }

            private AABBNode FindBestSibling(AABBNode node)
            {
                //We want to find a sibling that can give us the "least AABB surface area increment".

                //Create a queue of possible candidates.
                Queue<AABBNode> candidates = new Queue<AABBNode>();
                AABBNode bestSibling = null;
                float cost = float.MaxValue;

                //Enqueue root node as candidate.
                candidates.Enqueue(root);

                int i = 0;

                while(candidates.Count > 0)
                {
                    //Calculate the cost of choosing the current candidate.
                    AABBNode candidate = candidates.Dequeue();
                    float candidateCost = CalculateCost(node, candidate);

                    i++;
                    if(i > 100)
                        break;

                    //Update information if the cost is smaller.
                    if(candidateCost < cost)
                    {
                        cost = candidateCost;
                        bestSibling = candidate;
                    }

                    if(candidate.isLeaf)
                        continue;

                    //Calculate the low bound cost of choosing its subtree.
                    float subtreeLowBound = CalculateSubtreeLowBound(node, candidate);
                    //Enqueue child nodes if the low bound cost is smaller.
                    if(subtreeLowBound < cost)
                    {
                        candidates.Enqueue(candidate.leftNode);
                        candidates.Enqueue(candidate.rightNode);
                    }
                }

                return bestSibling;
            }

            private float CalculateCost(AABBNode node, AABBNode candidate)
            {
                //Cost = direct cost + inherited cost
                //Direct cost: The surface area of the newly created AABB.
                //Inherited cost: The increased surface area caused by refitting ancestors.

                //Direct cost 
                float cost = AABB.Union(node.bound, candidate.bound).surfaceArea;

                //Inherited cost
                AABBNode current = candidate.parentNode;
                while(current != null && current != current.parentNode)
                {
                    cost += AABB.Union(node.bound, current.bound).surfaceArea - current.bound.surfaceArea;
                    current = current.parentNode;
                }

                return cost;
            }

            private float CalculateSubtreeLowBound(AABBNode node, AABBNode candidate)
            {
                //Cost = OPTIMAL direct cost + inherited cost
                //OPT direct cost: The surface area of the node's AABB. (Any AABB created by merging with the node must have a non-lesser surface area than the original.)
                //Inherited cost: The increased surface area caused by refitting ancestors.

                //OPT direct cost
                float cost = node.bound.surfaceArea;

                //Inherited cost
                AABBNode current = candidate;

                int i = 0;

                while(current != null && current != current.parentNode)
                {
                    i++;
                    if(i > 10)
                        break;
                    cost += AABB.Union(node.bound, current.bound).surfaceArea - current.bound.surfaceArea;
                    current = current.parentNode;
                }

                return cost;
            }
        }    

        private static AABBTree[] trees;

        public static void Init() 
        { 
            trees = new AABBTree[32];
            for(int i = 0; i < 32; i++)
                trees[i] = new AABBTree();
        }

        public static void Add(Triangle tri, int layer)
        {
            if(layer < 0 || layer >= 32)
                throw new Exception("Invalid layer.");

            trees[layer].AddNode(tri);
        }

        public static void Remove(Triangle tri)
        {
            for(int i = 0; i < 32; i++)
            {
                if(trees[i].Contains(tri))
                {
                    trees[i].RemoveNode(tri);
                    break;
                }
            }
        }

        public static void ChangeLayer(Triangle tri, int newLayer)
        {
            if(newLayer < 0 || newLayer >= 32)
                throw new Exception("Invalid layer.");

            for(int i = 0; i < 32; i++)
            {
                if(trees[i].Contains(tri))
                {
                    trees[i].RemoveNode(tri);
                    trees[newLayer].AddNode(tri);
                    break;
                }
            }
        }

        /// <summary>
        /// Update the raycast manager. <br/> 
        /// This should be called right after objects update their positions.
        /// </summary>
        public static void Update()
        {
            for(int i = 0; i < 32; i++)
                trees[i].UpdateTree();
        }

        /// <summary>
        /// Cast a ray and return the first type of object it hits.
        /// </summary>
        /// <param name="origin">Origin of the ray.</param>
        /// <param name="ray">Direction of the ray.</param>
        /// <param name="distance">Maximum distance of the ray.</param>
        /// <param name="layerMask">Culling mask of the scan layer.</param>
        /// <returns>First type of object it hits. (-1 if it doesn't hit anything.)</returns>
        public static int Raycast(Vec3 origin, Vec3 ray, float distance, int layerMask)
        {
            List<Triangle> triangles;
            AABB bound = new AABB
            (
                Math.Min(origin.x, origin.x + ray.x * distance),
                Math.Max(origin.x, origin.x + ray.x * distance),
                Math.Min(origin.y, origin.y + ray.y * distance),
                Math.Max(origin.y, origin.y + ray.y * distance),  
                Math.Min(origin.y, origin.y + ray.y * distance),
                Math.Max(origin.y, origin.y + ray.y * distance)
            );

            int layer = -1;
            float bestIntersect = float.MaxValue;
            float intersect;

            for(int i = 0; i < 32; i++)
            {
                if(((1 << i) & layerMask) == 0)
                    continue;
                triangles = trees[i].CheckIntersections(bound);
                foreach(Triangle tri in triangles)
                {
                    intersect = RayTriangleIntersection(origin, ray, distance, tri);
                    if(intersect >= 0 && intersect < bestIntersect)
                    {
                        bestIntersect = intersect;
                        layer = i;
                    }
                }
            }

            return layer;
        }

        /// <summary>
        /// Check whether a ray and a triangle plane intersects.
        /// </summary>
        /// <param name="origin">Origin of the ray.</param>
        /// <param name="ray">Direction of the ray.</param>
        /// <param name="distance">Maximum distance of the ray.</param>
        /// <param name="tri">The triangle plane.</param>
        /// <returns>Distance of where intersection happens. (-1 if there's no intersection.)</returns>
        private static float RayTriangleIntersection(Vec3 origin, Vec3 ray, float distance, Triangle tri)
        {
            //If the ray is parallel along the plane, return false.
            if(Vec3.Dot(ray, tri.normal) == 0)
                return -1;

            //t is the distance needed for origin to travel along the ray vector to reach to plane.
            float t = (-tri.plane.d - origin.x - origin.y - origin.z)/(ray.x + ray.y + ray.z);
            //If t is negative (aka: the ray is facing towards the wrong way) or it's greater than the given distance. Return false.
            if(t < 0 || t > distance)
                return -1;
            
            // Cramer's rule.
            // i.x = ab.x * u + bc.x * v
            // i.y = ab.y * u + bc.y * v
            // u = detU / det
            // v = detV / det
            Vec3 i = (origin + ray * t) - tri.a;
            Vec3 ab = tri.b - tri.a;
            Vec3 bc = tri.c - tri.b;
            float det = ab.x * bc.y - bc.x * ab.y;
            float detU = i.x * bc.y - bc.x * i.y;
            float detV = ab.x * i.y - i.x * ab.y;
            float u = detU / det;
            float v = detV / det;

            if(u < 0 || v < 0 || u + v > 1)
                return -1;

            return t;
        }
    }

    /// <summary>
    /// A structure representing AABB. (Axis-Aligned Bounding Box)
    /// </summary>
    [System.Serializable]
    public struct AABB
    {
        public readonly float minX;
        public readonly float maxX;
        public readonly float minY;
        public readonly float maxY;
        public readonly float minZ;
        public readonly float maxZ;
        public readonly float surfaceArea;
        public readonly float volume;

        public AABB(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;
            this.minZ = minZ;
            this.maxZ = maxZ;

            float x = maxX - minX;
            float y = maxY - minY;
            float z = maxZ - minZ;
            volume = x * y * z;
            surfaceArea = 2 * (x*y + y*z + z*x);
        }

        public bool IntersectWith(AABB other)
        {
            return  other.maxX > minX && 
                    maxX > other.minX &&
                    other.maxY > minY && 
                    maxY > other.minY &&
                    other.maxZ > minZ && 
                    maxZ > other.minZ;
        }

        public bool Inside(AABB other)
        {
            return  other.maxX >= maxX && 
                    minX >= other.minX &&
                    other.maxY >= maxY && 
                    minY >= other.minY &&
                    other.maxZ >= maxZ && 
                    minZ >= other.minZ;
        }

        public static AABB Union (AABB boundA, AABB boundB)
        {
            return new AABB
            (
                Math.Min(boundA.minX, boundB.minX),
                Math.Max(boundA.maxX, boundB.maxX),
                Math.Min(boundA.minY, boundB.minY),
                Math.Max(boundA.maxY, boundB.maxY),
                Math.Min(boundA.minZ, boundB.minZ),
                Math.Max(boundA.maxZ, boundB.maxZ)
            );
        }
    }
}*/