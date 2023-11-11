using System.Linq;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Bogay.SceneAudioManager;

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

        var delta = Random.onUnitSphere * 3;
        delta.y = 0;
        player.SetPosition(anotherPortal.transform.position + delta);
        SceneAudioManager.instance.PlayByName("Portal");
        Destroy(gameObject);
    }
}
