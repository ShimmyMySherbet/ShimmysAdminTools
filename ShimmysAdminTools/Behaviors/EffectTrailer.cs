using SDG.Unturned;
using UnityEngine;

namespace ShimmysAdminTools.Behaviors
{
    public class EffectTrailer : MonoBehaviour
    {
        public ushort EffectID { get; set; }

        public float Rate { get; set; } = 0.05f;

        public float Radius { get; set; } = EffectManager.LARGE;

        private float LastTick = 0f;

        public void FixedUpdate()
        {
            if (!enabled)
                return;

            if (EffectID == 0)
                return;

            var now = Time.realtimeSinceStartup;
            var timeSinceLastTick = now - LastTick;

            if (timeSinceLastTick >= Rate)
            {
                LastTick = now;
                EffectManager.sendEffect(EffectID, Radius, transform.position);
            }
        }
    }
}