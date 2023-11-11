using System;
using VContainer;
using VContainer.Unity;
using UnityEngine;

public class MainLifetimeScope : LifetimeScope
{
    [SerializeField]
    private GameObject portalPrefab;

    [SerializeField]
    private GameObject player;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(this.portalPrefab);
        builder.Register<PortalRepository>(Lifetime.Singleton);
        builder.RegisterBuildCallback(container =>
        {
            container.InjectGameObject(player);
        });
    }
}
