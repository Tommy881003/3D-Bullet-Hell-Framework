using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BulletHell3D;

public class TestBHTransformSpawner : MonoBehaviour
{
    [SerializeField]
    private BHTransformUpdater bHTransformA;
    [SerializeField]
    private BHTransformUpdater bHTransformB;

    [SerializeField]
    private BHPattern tracerPattern;

    private int counter = 0;

    private void Start() 
    {
        InvokeRepeating("Shoot", 0, 1f);    
    }

    void Shoot()
    {
        BHTracerUpdater.instance.AddPattern(tracerPattern, transform.position, Random.insideUnitSphere, 0);
        BHTransformUpdater newTransform = Instantiate((counter % 2 == 0)? bHTransformA.gameObject : bHTransformB.gameObject, transform.position, Quaternion.identity).GetComponent<BHTransformUpdater>();
        newTransform.Init(transform.position,Random.insideUnitSphere,0);
        counter++;
    }
}
