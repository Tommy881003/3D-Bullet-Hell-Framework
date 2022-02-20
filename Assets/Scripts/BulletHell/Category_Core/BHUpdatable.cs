using System;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell3D
{
    public interface IBHBulletUpdater
    {
        List<BHBullet> bullets { get; }

        /// <summary>
        /// Define what should be done to the updater upon its creation. <br/>
        /// DO NOT invoke this manually, unless you're writing a "base" Updater class using the <see cref="IBHBulletUpdater"/> interface. <br/>
        /// For example: <see cref="BHTransformUpdater"/>, <see cref="BHCustomUpdater"/>, <see cref="BHTracerUpdater"/>.
        /// </summary>
        void InitUpdater();

        /// <summary>
        /// Define what should be done to the bullets when an updater get called. <br/>
        /// DO NOT invoke this manually. (Usually this should only get called by <see cref="BHManager"/>.)
        /// </summary>
        void UpdateBullets(float deltaTime);

        /// <summary>
        /// Define what should be done to the updater when some of its bullets get destroyed. <br/>
        /// DO NOT invoke this manually. (Usually this should only get called by <see cref="BHManager"/>.)
        /// </summary>
        void RemoveBullets();

        /// <summary>
        /// Define what should be done to the updater when it's no longer needed. <br/>
        /// DO NOT invoke this manually, unless you're writing a "base" Updater class using the <see cref="IBHBulletUpdater"/> interface. <br/>
        /// For example: <see cref="BHTransformUpdater"/>, <see cref="BHCustomUpdater"/>, <see cref="BHTracerUpdater"/>.
        /// </summary>
        void DestroyUpdater();
    }

    public interface IBHRayUpdater
    {
        List<BHRay> rays { get; }

        /// <summary>
        /// Define what should be done to the updater upon its creation. <br/>
        /// DO NOT invoke this manually, unless you're writing a "base" Updater class using the <see cref="IBHRayUpdater"/> interface. <br/>
        /// For example: <see cref="BHLaser"/>, <see cref="BHRing"/>.
        /// </summary>
        void InitUpdater();

        /// <summary>
        /// Define what should be done to the rays when an updater get called. <br/>
        /// DO NOT invoke this manually. (Usually this should only get called by <see cref="BHManager"/>.)
        /// </summary>
        void UpdateRays(float deltaTime);

        /// <summary>
        /// Define what should be done to the updater upon its creation. <br/>
        /// DO NOT invoke this manually, unless you're writing a "base" Updater class using the <see cref="IBHRayUpdater"/> interface. <br/>
        /// For example: <see cref="BHLaser"/>, <see cref="BHRing"/>.
        /// </summary>
        void DestroyUpdater();
    }
}

