using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collision Groups")]
public class CollisionGroups : ScriptableObject
{
    // This is used to specify and categorize different "set" of objects. (For example: obstacles, player)
    // Feel free to add/modify/remove layermask by need.

    // Define which layer(s) are categorized as obstacles.
    [SerializeField]
    private LayerMask _obstacleMask;
    public LayerMask obstacleMask { get { return _obstacleMask; } }

    // Define which layer(s) are categorized as enemies.
    [SerializeField]
    private LayerMask _enemyMask;
    public LayerMask enemyMask { get { return _enemyMask; } }

    // Define which layer(s) are categorized as players.
    [SerializeField]
    private LayerMask _playerMask;
    public LayerMask playerMask { get { return _playerMask; } }
}
