using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell3D;

public class BulletHellDemo5_1 : BHTransformUpdater
{
    public Vector2 GetMaxMinY()
    {
        int count = bullets.Count;
        float maxY = float.MinValue;
        float minY = float.MaxValue;

        for(int i = 0; i < count; i++)
        {
            if(localPos[i].y > maxY)
                maxY = localPos[i].y; 
            if(localPos[i].y < minY)
                minY = localPos[i].y;
        }
        return new Vector2(maxY, minY);
    }

    public void SpawnRandom(int spawnCount, BHRenderObject tracerObj, float tracerSpeed, float tracerDelay)
    {
        int count = bullets.Count;
        if(count == 0)
            return;

        int i = 0;
        while(i < spawnCount)
        {
            int j = 0;
            int pick = Random.Range(0,count);
            while(!bullets[pick].isAlive && j < 10)
            {
                pick = Random.Range(0,count);
                j++;
            }
            if(bullets[pick].isAlive)
            {
                bullets[pick].isAlive = false;
                BHTracerUpdater.instance.AddBullet( tracerObj, 
                                                    bullets[pick].position,
                                                    transform.TransformDirection(localPos[pick]),
                                                    tracerSpeed,
                                                    tracerDelay
                                                    );
            }
            i++;
        }
        
    }
}
