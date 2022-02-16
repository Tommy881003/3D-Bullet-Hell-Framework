using System;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell3D
{
    public interface IBHUpdatable
    {
        List<BHBullet> bullets { get; }

        /// <summary>
        /// Define what should be done to the updatable upon its creation. <br/>
        /// DO NOT invoke this manually, unless you're writing a "base" Updater class using the <see cref="IBHUpdatable"/> interface. <br/>
        /// For example: <see cref="BHTransformUpdater"/>, <see cref="BHSimpleUpdater"/>, <see cref="BHTracerUpdater"/>.
        /// </summary>
        void InitUpdatable();

        /// <summary>
        /// Define what should be done to the bullets when an updatable get called. <br/>
        /// DO NOT invoke this manually. (Usually this should only get called by <see cref="BHManager"/>.)
        /// </summary>
        void UpdateBullets(float deltaTime);

        /// <summary>
        /// Define what should be done to the updatable when some of its bullets get destroyed. <br/>
        /// DO NOT invoke this manually. (Usually this should only get called by <see cref="BHManager"/>.)
        /// </summary>
        void RemoveBullets();

        /// <summary>
        /// Define what should be done to the updatable when it's no longer needed. <br/>
        /// DO NOT invoke this manually, unless you're writing a "base" Updater class using the <see cref="IBHUpdatable"/> interface. <br/>
        /// For example: <see cref="BHTransformUpdater"/>, <see cref="BHSimpleUpdater"/>, <see cref="BHTracerUpdater"/>.
        /// </summary>
        void DestroyUpdatable();
    }
}

