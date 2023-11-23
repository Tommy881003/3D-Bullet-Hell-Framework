using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace SpellBound
{
    public class MainWeaponVFX : MonoBehaviour
    {
        private VisualEffect vfx;
        private Vector3 lastPosition;

        IEnumerator Start()
        {
            this.lastPosition = transform.position;
            this.vfx = GetComponent<VisualEffect>();
            yield return new WaitForSeconds(0.05f);
            this.vfx.SendEvent("OnPlay");
        }

        void FixedUpdate()
        {
            this.vfx.SetVector3("Velocity", (this.lastPosition - transform.position));
            this.vfx.SetVector3("Position", transform.position);
            this.lastPosition = transform.position;
        }
    }
}
