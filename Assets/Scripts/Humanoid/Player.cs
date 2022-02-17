using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    void Awake() 
    {
        DependencyContainer.AddDependency(this);    
    }
}
