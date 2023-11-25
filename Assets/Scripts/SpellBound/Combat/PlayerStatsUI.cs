using System.Collections;
using System.Collections.Generic;
using SpellBound.Core;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField]
    private Slider hpSlider;
    [SerializeField]
    private Slider mpSlider;

    [Inject]
    private Character character;

    void Update()
    {
        this.hpSlider.maxValue = this.character.MaxHP.Value();
        this.hpSlider.value = this.character.HP;
        this.mpSlider.maxValue = this.character.MaxMP.Value();
        this.mpSlider.value = this.character.MP;
    }
}
