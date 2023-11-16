using System.Linq;
using UnityEngine;
using VContainer;
using Bogay.SceneAudioManager;
using UnityEngine.VFX;
using System.Collections;

public class Portal : MonoBehaviour
{
    public System.Guid id;

    [SerializeField]
    private VisualEffectAsset afterTeloportedVFX;
    [SerializeField]
    private float yOffset;

    [Inject]
    private readonly PortalRepository repository;

    public bool used { get; private set; }

    void Start()
    {
        transform.Translate(Vector3.up * this.yOffset);
        this.used = false;
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

        var anotherPortal = this.repository.Portals.FirstOrDefault(portal => portal.id != this.id && !portal.used);
        if (anotherPortal == null)
        {
            Debug.LogWarning("No other portal found");
            return;
        }

        this.used = true;

        var delta = Random.onUnitSphere * 3;
        delta.y = 0;
        var targetPosition = anotherPortal.transform.position + delta;
        player.SetPosition(targetPosition);
        SceneAudioManager.instance.PlayByName("Portal");

        var go = new GameObject("AfterTeleported");
        go.transform.position = targetPosition;
        var vfx = go.AddComponent<VisualEffect>();
        vfx.visualEffectAsset = this.afterTeloportedVFX;

        GetComponent<Collider>().enabled = false;
        StartCoroutine(this.destroyOnVFXEnd());
    }

    private IEnumerator destroyOnVFXEnd()
    {
        var vfx = GetComponentInChildren<VisualEffect>();
        var size = vfx.GetFloat("Size");
        while (size > 0.0001f)
        {
            size = Mathf.Lerp(size, 0, 0.01f);
            vfx.SetFloat("Size", size);
            yield return null;
        }
        Destroy(gameObject);
    }
}
