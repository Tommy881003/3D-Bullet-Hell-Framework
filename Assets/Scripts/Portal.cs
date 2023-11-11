using System.Linq;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class Portal : MonoBehaviour
{
    public System.Guid id;

    [SerializeField]
    private float yOffset;

    [Inject]
    private readonly PortalRepository repository;

    void Start()
    {
        transform.Translate(Vector3.up * this.yOffset);
    }

    void OnDestroy()
    {
        this.repository.Remove(this.id);
    }

    void OnTriggerEnter(Collider collider)
    {
        var player = collider.gameObject.GetComponent<PlayerController>();
        if (player == null) return;

        Debug.Log("hit player");

        var anotherPortal = this.repository.Portals.FirstOrDefault(portal => portal.id != this.id);
        if (anotherPortal == null)
        {
            Debug.LogWarning("No other portal found");
            return;
        }

        // TODO: VFX, dispose
        var delta = Random.onUnitSphere * 2;
        delta.y = 0;
        player.SetPosition(anotherPortal.transform.position + delta);
        Destroy(gameObject);
    }
}
