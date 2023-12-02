using VContainer;
using VContainer.Unity;
using UnityEngine;
using BulletHell3D;

public class PlayerLifetimeScope : LifetimeScope
{
    [SerializeField]
    private Player player;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(this.player);
    }
}
