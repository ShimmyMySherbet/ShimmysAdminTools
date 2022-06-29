using SDG.Unturned;
using UnityEngine;

namespace ShimmysAdminTools.Behaviors
{
    public class FireworkBehaviour : MonoBehaviour
    {
        public Player Player { get; private set; }
        public float FlightTime => Time.realtimeSinceStartup - Launched;
        public bool IsLaunched { get; private set; }
        public float Fuse { get; set; } = 1f;
        private float Launched { get; set; } = 0f;

        public ushort TrailEffectID { get; set; } = 139;
        public float TrailRate { get; set; } = 0.05f;

        public ushort ExplosionEffectID { get; set; } = 20;
        public ushort ExplosionDamage { get; set; } = 200;
        public ushort ExplosionRadius { get; set; } = 30;

        public EffectTrailer Trailer { get; private set; }

        private float m_PrevGravity = 1f;

        public void Awake()
        {
            Player = GetComponent<Player>();
            Trailer = gameObject.AddComponent<EffectTrailer>();
            Trailer.Radius = EffectManager.INSANE;
        }

        public void Launch()
        {
            // Invert the player's gravity so they fly upward
            m_PrevGravity = Player.movement.pluginGravityMultiplier;
            Player.movement.sendPluginGravityMultiplier(-1.5f);

            // Get the player up off the ground, so gravity kicks in
            var resetPosition = transform.position;
            Player.teleportToLocationUnsafe(resetPosition + new Vector3(0, 0.4f, 0), Player.look.rot);

            // Enable trail effects
            Trailer.EffectID = TrailEffectID;
            Trailer.Rate = TrailRate;
            Trailer.enabled = true;

            Launched = Time.realtimeSinceStartup;
            IsLaunched = true;
        }

        public void Abort()
        {
            if (IsLaunched)
            {
                IsLaunched = false;
                Player.movement.sendPluginGravityMultiplier(m_PrevGravity);
            }
        }

        public void FixedUpdate()
        {
            if (IsLaunched)
            {
                if (FlightTime >= Fuse)
                {
                    EffectManager.sendEffect(ExplosionEffectID, EffectManager.INSANE, transform.position);
                    DamageTool.explode(transform.position, ExplosionRadius, EDeathCause.GRENADE, Player.channel.owner.playerID.steamID,
                        ExplosionDamage, ExplosionDamage, ExplosionDamage, ExplosionDamage, ExplosionDamage, ExplosionDamage, ExplosionDamage,
                        ExplosionDamage, out _);
                    AlertTool.alert(transform.position, 200f);
                    Player.movement.sendPluginGravityMultiplier(m_PrevGravity);
                    IsLaunched = false;
                    Trailer.enabled = false;
                    Destroy(Trailer);
                    Destroy(this);
                }
            }
        }
    }
}