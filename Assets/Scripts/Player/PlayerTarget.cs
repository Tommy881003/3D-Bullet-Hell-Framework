using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTarget : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    private float followStrength;

    private Vector3 offset;
    private Transform target;

    void Start()
    {
        this.offset = transform.localPosition;
        this.target = transform.parent;
        transform.SetParent(null);
    }

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position - this.offset, this.target.transform.position, this.followStrength) + this.offset;
    }
}
