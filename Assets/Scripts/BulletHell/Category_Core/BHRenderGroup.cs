using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell3D
{
    public class BHRenderGroup
    {
        private struct BulletRenderData
        {
            public readonly Vector4 position;
            public readonly Color? color;

            public BulletRenderData(Vector3 position, Color? color)
            {
                // The "1" in w factor is for the bottom right element in a transform matrix.
                this.position = new Vector4(position.x, position.y, position.z, 1);
                this.color = color;
            }
        }

        public struct BatchedBulletRenderData
        {
            public Matrix4x4[] matrices;
            public MaterialPropertyBlock propertyBlock;

            public BatchedBulletRenderData(BHRenderGroup renderGroup) 
            { 
                matrices = new Matrix4x4[1023];
                Matrix4x4 initialMatrix = Matrix4x4.Scale(Vector3.one * renderGroup.renderObject.radius * 2);
                for(int i = 0; i < 1023; i++)
                    matrices[i] = initialMatrix;
                
                // MaterialPropertyBlock array size is fixed, so you must create max capacity at start-up. (Thank you unity.)
                propertyBlock = new MaterialPropertyBlock();
                Vector4[] colorArray = new Vector4[1023];
                propertyBlock.SetVectorArray("_Color", colorArray);
            }
        }
        
        public const int RENDER_NUM_PER_BATCH = 1023;

        public readonly BHRenderObject renderObject;
        private readonly Color defaultColor;
        public int count { get; private set; } = 0;
        public int batchCount { get { return Mathf.CeilToInt(count / (float)RENDER_NUM_PER_BATCH); } }
        
        private List<BulletRenderData> unbatchedData = new List<BulletRenderData>();
        public List<BatchedBulletRenderData> batches { get; private set; }= new List<BatchedBulletRenderData>();

        public BHRenderGroup(BHRenderObject renderObj)
        {
            renderObject = renderObj;
            defaultColor = renderObject.material.GetColor("_Color");
        }

        public void SetBulletRenderData(Vector3 position, Color? color = null)
        {
            unbatchedData.Add(new BulletRenderData(position,color));
        }

        // TODO: This need some optimization.
        public void FinalizeBatchedRenderData()
        {
            count = unbatchedData.Count;
            
            while(batches.Count < batchCount)
                batches.Add(new BatchedBulletRenderData(this));
            
            for(int i = 0; i < batchCount; i++)
            {
                var batch = batches[i];
                Vector4[] colors = new Vector4[RENDER_NUM_PER_BATCH];
                for(int j = 0; j < RENDER_NUM_PER_BATCH; j++)
                {
                    int index = i * RENDER_NUM_PER_BATCH + j;
                    if(index >= count)
                        break;
                    batch.matrices[j].SetColumn(3,unbatchedData[index].position);
                    colors[j] = unbatchedData[index].color.GetValueOrDefault(defaultColor);
                }
                batches[i].propertyBlock.SetVectorArray("_Color",colors);
            }
            unbatchedData.Clear();
        }

        public void InvalidateOldBatchedRenderData() { count = 0; }
    }
}