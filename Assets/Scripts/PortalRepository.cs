using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using System.Linq;

public class PortalRepository
{
    private readonly GameObject portalPrefab;
    private readonly LifetimeScope scope;

    private Dictionary<Guid, Portal> portals;

    public PortalRepository(GameObject portalPrefab, LifetimeScope scope)
    {
        Debug.Assert(portalPrefab != null);
        Debug.Assert(scope != null);

        this.portalPrefab = portalPrefab;
        this.scope = scope;

        this.portals = new Dictionary<Guid, Portal>();
    }

    public Portal Create(Vector3 position)
    {
        var go = this.scope.Container.Instantiate(this.portalPrefab, position, Quaternion.identity);
        var portal = go.GetComponent<Portal>();
        Debug.Assert(portal != null);
        portal.id = Guid.NewGuid();
        this.portals.Add(portal.id, portal);
        return portal;
    }

    public void Remove(Guid id)
    {
        this.portals.Remove(id);
    }

    public IReadOnlyCollection<Portal> Portals => this.portals.Values.ToList().AsReadOnly();
}
