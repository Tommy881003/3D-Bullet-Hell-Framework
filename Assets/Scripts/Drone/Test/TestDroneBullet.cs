using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDroneBullet : MonoBehaviour
{
    private TrailRenderer tr;

    [SerializeField]
    private float speed = 50;
    private LayerMask collisionMask;
    private LayerMask obstacleMask;
    private LayerMask enemyMask;

    // "Raycast" type bullet has two bullet position: visual and logical.
    // Visual position represent what player sees, logical position represent the actual collision calculation.
    // We will use the transform of this object as viusal position. (Since it has trail renderer)
    private Vector3 logicalPosition; 
    private Vector3 direction;

    void Awake() 
    {
        tr = GetComponent<TrailRenderer>();
        obstacleMask = CollisionGroups.instance.obstacleMask;
        enemyMask = CollisionGroups.instance.enemyMask;
        collisionMask = obstacleMask | enemyMask;    
    }

    public void SetBullet(Vector3 logical, Vector3 dir)
    {
        logicalPosition = logical;
        direction = dir.normalized;
    }

    private void FixedUpdate() 
    {
        RaycastHit hit;
        float distance = speed * Time.fixedDeltaTime;
        if(Physics.Raycast(logicalPosition, direction, out hit, distance, collisionMask))
        {
            transform.position = hit.point;
            Destroy(gameObject,tr.time);
            enabled = false;
            return;
        }

        logicalPosition += direction * distance;
        transform.position += direction * distance;
        transform.position = Vector3.Lerp(transform.position, logicalPosition, 0.1f);
    }
}
