using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAngleAxis : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float angle = 0.0f;
        Vector3 axis = Vector3.zero;
        transform.rotation.ToAngleAxis(out angle, out axis);
        Debug.Log("Axis : " + axis.ToString() + ", Angle : " + angle.ToString());
    }
}
