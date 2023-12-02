using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell3D;

public class BulletHellDemo1_1 : BHCustomUpdater
{
    [SerializeField]
    private float speed;
    [SerializeField, Range(0, 1)]
    private float rotateStrenth;

    protected override void UpdateCustom(float deltaTime)
    {
        int length = bullets.Count;

        for (int i = 0; i < length; i++)
        {
            Vector3 position = bullets[i].position;
            bullets[i].SetPosition(position + forwards[i] * speed * deltaTime);
            forwards[i] = Vector3.Slerp(forwards[i], transform.forward, rotateStrenth);
        }
    }
}
