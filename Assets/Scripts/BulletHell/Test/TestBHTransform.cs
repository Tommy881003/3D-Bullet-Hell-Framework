using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BulletHell3D;

public class TestBHTransform : BHTransformUpdater
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float size;
    [SerializeField]
    private float expandTime;
    private float timer = 0;

    protected override void UpdateTransform(float deltaTime)
    {
        transform.position += Vector3.down * timer * 0.05f;
        transform.localScale = Vector3.one * timer * 10;

        //angleInDeg += 30 * deltaTime;
        //BHHelper.LookRotationSolver(relativeForward, angleInDeg, out relativeUp, out relativeRight);
        //transform.rotation = Quaternion.LookRotation(relativeForward, relativeUp);

        timer += deltaTime; 
    }
}
