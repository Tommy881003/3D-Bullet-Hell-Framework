using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellBound.Combat
{
    [System.Serializable]
    public struct SkillTriggerSetting
    {
        [field: SerializeField]
        public float CooldownSeconds { get; private set; }
        [field: SerializeField]
        public int Cost { get; private set; }
    }
}
