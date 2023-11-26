using System.Collections;
using System.Collections.Generic;
using SpellBound.Core;
using UnityEngine;
using VContainer;

public class Dash : MonoBehaviour
{
    [SerializeField]
    private float distance;
    [SerializeField]
    public int Cost { get; private set; }
    [SerializeField]
    private PlayerController playerController;

    [Inject]
    private readonly Character owner;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Cast(Vector3 forward)
    {

    }
}
