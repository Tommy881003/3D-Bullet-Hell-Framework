using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRingRatio : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lr;
    [SerializeField, Range(0.1f,30)]
    private float ringRadius;
    [SerializeField, Range(1,10)]
    private float ratio;

    private const int minSegmentCount = 4;

    private void Start() 
    {
        lr = GetComponent<LineRenderer>();
    }

    private void FixedUpdate() 
    {
        int segmentCount = minSegmentCount + Mathf.CeilToInt(ringRadius * ratio);
        lr.positionCount = segmentCount;

        float angle = Mathf.PI * 2 / segmentCount;

        for(int i = 0; i < segmentCount; i ++)
            lr.SetPosition(i, transform.position + new Vector3(Mathf.Cos(angle * i), 0, Mathf.Sin(angle * i)) * ringRadius);
    }
}
