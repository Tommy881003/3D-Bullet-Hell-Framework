using VContainer;
using VContainer.Unity;
using UnityEngine;
using SpellBound.Core;
using System.Collections.Generic;

public class CharacterLifetimeScope : LifetimeScope
{
    [SerializeField]
    private Character character;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(this.character);
    }
}
