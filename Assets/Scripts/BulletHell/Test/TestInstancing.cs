using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

public class TestInstancing : MonoBehaviour
{
    [SerializeField]
    private bool useJob;
    [SerializeField]
    private Mesh mesh;
    [SerializeField]
    private Material material;
    [SerializeField]
    private int batchCount;
    [SerializeField]
    private float radius;

    private Matrix4x4[][] matrices;
    private Vector3[][] positions;

    // Start is called before the first frame update
    void Start()
    {
        matrices = new Matrix4x4[batchCount][];
        positions = new Vector3[batchCount][];

        for(int j = 0; j < batchCount; j++)
        {
            matrices[j] = new Matrix4x4[1023];
            positions[j] = new Vector3[1023];
            for(int i = 0; i < 1023 ; i++)
            {
                Vector3 position = Random.insideUnitSphere * radius;
                matrices[j][i] = Matrix4x4.TRS(Random.insideUnitSphere * radius, Quaternion.identity, Vector3.one);
                positions[j][i] = position;
            }
        }
    }

    void FixedUpdate()
    {
        if(useJob)
            JobRaycast();
        else
            NoJobRaycast();
    }

    void NoJobRaycast()
    {
        for(int j = 0; j < batchCount; j++)
        {
            for(int i = 0; i < 1023 ; i++)
            {
                if(Physics.Raycast(positions[j][i],Vector3.forward,Time.fixedDeltaTime))
                    positions[j][i] = Random.insideUnitSphere * radius;
                positions[j][i] += Vector3.forward * Time.fixedDeltaTime;

                Vector4 column = (Vector4)positions[j][i] + new Vector4(0,0,0,1); 
                matrices[j][i].SetColumn(3, column);
            }
        }
    }

    void JobRaycast()
    {
        // Perform a single sphere cast using SpherecastCommand and wait for it to complete
        // Set up the command and result buffers
        var results = new NativeArray<RaycastHit>(1023 * batchCount, Allocator.TempJob);
        var commands = new NativeArray<SpherecastCommand>(1023 * batchCount, Allocator.TempJob);

        // Set the data of the first command
        Vector3 direction = Vector3.forward;

        for(int j = 0; j < batchCount; j++)
            for(int i = 0; i < 1023 ; i++)
                commands[j * 1023 + i] = new SpherecastCommand(positions[j][i], 0.5f, direction, Time.fixedDeltaTime);

        
        // Schedule the batch of sphere casts
        var rayHandle = SpherecastCommand.ScheduleBatch(commands, results, 256, default(JobHandle));

        // Wait for the batch processing job to complete
        rayHandle.Complete();

        // Copy the result. If batchedHit.collider is null, there was no hit
        for(int j = 0; j < batchCount; j++)
        {
            for(int i = 0; i < 1023 ; i++)
            {
                if(results[j * 1023 + i].collider != null)
                    positions[j][i] = Random.insideUnitSphere * radius;
                positions[j][i] += Vector3.forward * Time.fixedDeltaTime;

                Vector4 column = (Vector4)positions[j][i] + new Vector4(0,0,0,1); 
                matrices[j][i].SetColumn(3, column);
            }
        }

        // Dispose the buffers
        results.Dispose();
        commands.Dispose();
    }

    void Update() 
    {
        for(int i = 0; i < batchCount; i ++)
            Graphics.DrawMeshInstanced(mesh,0,material,matrices[i],1023, null, UnityEngine.Rendering.ShadowCastingMode.Off, false);
    }
}

public struct PositionUpdateJob : IJobParallelFor
{
    public NativeArray<Vector3> positions;
    public NativeArray<Matrix4x4> matrices;
    public NativeArray<RaycastHit> raycasts;

    [ReadOnly]
    public float deltaTime;

    public void Execute(int index)
    {
        if(raycasts[index].collider != null)
            positions[index] -= new Vector3(0,0,10);
        positions[index] += new Vector3(0,0,deltaTime); 

        Vector4 column = (Vector4)positions[index] + new Vector4(0,0,0,1); 
        matrices[index].SetColumn(3, column);
    }
}