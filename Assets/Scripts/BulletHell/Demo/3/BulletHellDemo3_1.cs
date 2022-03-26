using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell3D;

public class BulletHellDemo3_1 : BHCustomUpdater
{
    [SerializeField]
    private float minSpeed;
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float dropStrenth;

    private List<float> speeds = new List<float>();

    protected override void UpdateCustom(float deltaTime)
    {
        int length = bullets.Count;

        while(speeds.Count < bullets.Count)
            speeds.Add(Random.Range(minSpeed, maxSpeed));

        for(int i = 0; i < length; i++)
        {
            Vector3 position = bullets[i].position;
            bullets[i].SetPosition(position + forwards[i] * speeds[i] * deltaTime);
            forwards[i] = (forwards[i] + Vector3.down * dropStrenth).normalized;
        }
    }

    public override void RemoveBullets()
    {
        BHUpdaterHelper.DefaultRemoveBullets<Vector3,float>(this, ref forwards, ref speeds);
    }
}
