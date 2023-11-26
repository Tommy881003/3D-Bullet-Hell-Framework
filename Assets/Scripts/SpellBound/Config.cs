using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellBound
{
    [CreateAssetMenu(menuName = "SpellBound/Config")]
    public class Config : ScriptableObject
    {
        [field: SerializeField]
        public CollisionGroups CollisionGroups { get; private set; }
    }
}

