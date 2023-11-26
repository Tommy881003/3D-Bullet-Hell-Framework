using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SpellBound.Combat;
using SpellBound.Core;
using UnityEngine;
using VContainer;

public class Dash : MonoBehaviour
{
    [SerializeField]
    private float distance;

    [field: SerializeField]
    public SkillTriggerSetting skillTriggerSetting { get; private set; }
    private SkillTrigger<Vector3> skillTrigger;

    // TODO: DI
    [SerializeField]
    private PlayerController playerController;

    [Inject]
    private readonly Character owner;

    void Start()
    {
        this.skillTrigger = new SkillTrigger<Vector3>(
            this.skillTriggerSetting,
            this.owner
        );

        this.skillTrigger.Subscribe(fwd =>
        {
            var stopsAt = this.playerController.transform.position + fwd * this.distance;
            this.playerController.SetPosition(stopsAt);
        });

        var ct = this.GetCancellationTokenOnDestroy();
        this.skillTrigger.Start(ct);
    }

    public void Cast(Vector3 forward)
    {
        forward.y = 0;
        forward = forward.normalized;
        this.skillTrigger.Trigger(forward);
    }
}
