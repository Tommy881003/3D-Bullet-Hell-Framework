using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*using BulletHell3D.Base;

namespace BulletHell3D
{
    public class BHManager : GameBehaviour
    {
        private static BHManager _instance = null;
        public static BHManager instance { get { return _instance; } }

        public override void GameAwake()
        {
            if(_instance == null)
                _instance = this;
            else
            {
                KillBehaviour(true);
                return;
            }

            RaycastManager.Init();
        }

        public override void GameFixedUpdate()
        {
            RaycastManager.Update();
        }
    }
}*/