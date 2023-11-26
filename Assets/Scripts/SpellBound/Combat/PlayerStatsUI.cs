using System.Collections;
using System.Collections.Generic;
using SpellBound.Combat;
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

    // TODO: extract to component
    // TODO: DI
    [SerializeField]
    private Image cooldownImage;
    [SerializeField]
    private MainWeapon weapon;

    [Inject]
    private Character character;

    void Update()
    {
        this.hpSlider.maxValue = this.character.MaxHP.Value();
        this.hpSlider.value = this.character.HP;
        this.mpSlider.maxValue = this.character.MaxMP.Value();
        this.mpSlider.value = this.character.MP;
        this.cooldownImage.fillAmount = 1 - this.weapon.ShootTimer / this.weapon.ShootCooldownSeconds;
    }
}
