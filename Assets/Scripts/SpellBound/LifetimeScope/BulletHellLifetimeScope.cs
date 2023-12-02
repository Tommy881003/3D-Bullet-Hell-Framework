using VContainer;
using VContainer.Unity;
using UnityEngine;
using BulletHell3D;

public class BulletHellLifetimeScope : LifetimeScope
{
    [SerializeField]
    private BHTrailPool trailPool;

    protected override void Configure(IContainerBuilder builder)
    {
        Debug.Assert(this.trailPool != null);
        builder.RegisterInstance(this.trailPool);
        builder.RegisterFactory<GameObject, BHTracerUpdater>(container =>
        {
            return go =>
            {
                BHTracerUpdater updater = go.AddComponent<BHTracerUpdater>();
                container.Inject(updater);
                updater.InitUpdater();
                return updater;
            };
        }, Lifetime.Scoped);
        builder.RegisterFactory<GameObject, BulletHellDemo5_1>(container =>
        {
            return go =>
            {
                var demo51 = go.AddComponent<BulletHellDemo5_1>();
                container.Inject(demo51);
                return demo51;
            };
        }, Lifetime.Scoped);
    }
}
