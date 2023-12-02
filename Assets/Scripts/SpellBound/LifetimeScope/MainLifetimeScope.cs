using MessagePipe;
using MessagePipe.VContainer;
using VContainer;
using VContainer.Unity;
using UnityEngine;
using BulletHell3D;

public class MainLifetimeScope : LifetimeScope
{
    [SerializeField]
    private SpellBound.Config gameConfig;
    [SerializeField]
    private GameObject portalPrefab;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(this.gameConfig);
        builder.RegisterInstance(this.gameConfig.CollisionGroups);

        // setup MessagePipe
        // see more: https://github.com/Cysharp/MessagePipe#unity
        // TODO: provide helper in BHManager?
        var options = builder.RegisterMessagePipe();
        builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));
        builder.RegisterMessageBroker<System.Guid, CollisionEvent>(options);
    }
}
