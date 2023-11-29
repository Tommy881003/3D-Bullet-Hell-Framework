using System.Collections;
using System.Collections.Generic;
using SpellBound.Core;
using UnityEngine;
using VContainer;

public class PlayerPresenter : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [Inject]
    private readonly Character character;
    private PlayerController controller;

    void Start()
    {
        this.controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        this.animator.SetFloat("Velocity", this.controller.horizontalSpeed);
    }
}
