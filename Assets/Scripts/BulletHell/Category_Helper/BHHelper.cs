using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell3D
{
    public static class BHHelper
    {
        public static void LookRotationSolver(Vector3 forward, float angleInDeg, out Vector3 up, out Vector3 right)
        {
            // X * Y = Z
            // Z * X = Y
            // Y * Z = X
            forward = forward.normalized;
            float angleInRad = angleInDeg * Mathf.Deg2Rad;
            Vector3 rawUp = Vector3.zero;
            Vector3 rawRight = Vector3.zero;
            if(forward == Vector3.up || forward == Vector3.down)
                rawRight = Vector3.right;
            else
                rawRight = Vector3.Cross(Vector3.up, forward);
            rawUp = Vector3.Cross(forward, rawRight);
            
            right = Mathf.Cos(angleInRad) * rawRight + Mathf.Sin(angleInRad) * rawUp;
            up = Mathf.Cos(angleInRad) * rawUp - Mathf.Sin(angleInRad) * rawRight;
        }
    }

    public static class BHUpdatableHelper
    {
        private static BHManager manager = null;

        public static void DefaultInit(IBHBulletUpdater updatable)
        {
            if(manager == null)
            {
                manager = BHManager.instance;
                if(manager == null)
                    BHExceptionRaiser.RaiseException(BHException.ManagerNotFound);
            }
            manager.AddUpdatable(updatable);
        }

        public static void DefaultDestroy(IBHBulletUpdater updatable)
        {
            updatable.bullets.Clear();
            if(manager == null)
                manager = BHManager.instance;
            //If manager REALLY is null, that almost certainly means the scene get destroyed.
            //It really doesn't matter if the updatable actually get removed or not in this case.
            if(manager != null)
                manager.RemoveUpdatable(updatable);
        }

        public static void DefaultRemoveBullets(IBHBulletUpdater updatable)
        {
            var bullets = updatable.bullets;
            for(int i = bullets.Count - 1; i >= 0; i --)
            {
                if(!bullets[i].isAlive)
                {
                    bullets.RemoveAt(i);
                } 
            }
        }

        public static void DefaultRemoveBullets<T1>(IBHBulletUpdater updatable, ref List<T1> list1)
        {
            var bullets = updatable.bullets;
            for(int i = bullets.Count - 1; i >= 0; i --)
            {
                if(!bullets[i].isAlive)
                {
                    bullets.RemoveAt(i);
                    list1.RemoveAt(i);
                } 
            }
        }

        public static void DefaultRemoveBullets<T1, T2>(IBHBulletUpdater updatable, ref List<T1> list1, ref List<T2> list2)
        {
            var bullets = updatable.bullets;
            for(int i = bullets.Count - 1; i >= 0; i --)
            {
                if(!bullets[i].isAlive)
                {
                    bullets.RemoveAt(i);
                    list1.RemoveAt(i);
                    list2.RemoveAt(i);
                } 
            }
        }

        public static void DefaultRemoveBullets<T1, T2, T3>(IBHBulletUpdater updatable, ref List<T1> list1, ref List<T2> list2, ref List<T3> list3)
        {
            var bullets = updatable.bullets;
            for(int i = bullets.Count - 1; i >= 0; i --)
            {
                if(!bullets[i].isAlive)
                {
                    bullets.RemoveAt(i);
                    list1.RemoveAt(i);
                    list2.RemoveAt(i);
                    list3.RemoveAt(i);
                } 
            }
        }
    }
}