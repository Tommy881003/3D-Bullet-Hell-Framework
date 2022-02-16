using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDroneFollow : MonoBehaviour
{
    private Camera mainCam;
    [SerializeField]
    private Transform follow;
    [SerializeField, Range(0f,1f)]
    private float followStrenth;
    [SerializeField, Range(0f,1f)]
    private float rotateStrenth;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        transform.SetParent(null);
    }

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, follow.position, followStrenth);
        transform.rotation = Quaternion.Slerp(transform.rotation, mainCam.transform.rotation, rotateStrenth);
    }
}
