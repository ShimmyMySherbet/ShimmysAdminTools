using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SDG.Unturned;
using UnityEngine;

namespace ShimmysAdminTools.Behaviors
{
    public class FireworkBehaviour : MonoBehaviour
    {
        public Player Player { get; private set; }
        public float FlightTime => Time.realtimeSinceStartup - Launched;
        public bool IsLaunched { get; private set; }
        public float Fuse { get; set; } = 2f;
        private float Launched { get; set; } = 0f;

        public ushort TrailEffectID { get; set; } = 139;
        public float TrailRate { get; set; } = 0.05f;

        public ushort ExplosionEffectID { get; set; } = 20;
        public ushort ExplosionDamage { get; set; } = 200;
        public ushort ExplosionRadius { get; set; } = 10;
        public ushort ExplosionEffect { get; set; } = 20;
        public ushort ExplosionEffectCount { get; set; } = 70;
        public List<ushort> ExplosionEffects { get; } = new List<ushort>() { 124, 130, 312, 134, 139 };

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
            Player.movement.sendPluginGravityMultiplier(-2f);

            // Get the player up off the ground, so gravity kicks in
            var resetPosition = transform.position;
            Player.teleportToLocationUnsafe(resetPosition + new Vector3(0, 0.4f, 0), Player.look.rot);

            // Enable trail effects
            Trailer.EffectID = TrailEffectID;
            Trailer.Rate = TrailRate;
            Trailer.enabled = true;

            // Set launched
            Launched = Time.realtimeSinceStartup;
            IsLaunched = true;
        }

        public void Abort()
        {
            if (IsLaunched)
            {
                Stop();
                Destroy(Trailer);
                Destroy(this);
            }
        }

        public void FixedUpdate()
        {
            if (IsLaunched)
            {
                if (FlightTime >= Fuse)
                {
                    EffectManager.sendEffect(ExplosionEffectID, EffectManager.INSANE, transform.position);
                    EffectManager.sendEffect(123, EffectManager.INSANE, transform.position);
                    StartCoroutine(DoExplosionEffects());
                    DamageTool.explode(transform.position, ExplosionRadius, EDeathCause.MISSILE, Player.channel.owner.playerID.steamID,
                        ExplosionDamage, ExplosionDamage, ExplosionDamage, ExplosionDamage, ExplosionDamage, ExplosionDamage, ExplosionDamage,
                        ExplosionDamage, out _);
                    AlertTool.alert(transform.position, 200f);
                    Stop();
                }
            }
        }

        private void Stop()
        {
            Player.movement.sendPluginGravityMultiplier(m_PrevGravity);
            IsLaunched = false;
            Trailer.enabled = false;
        }

        private IEnumerator DoExplosionEffects(float timeToComplete = 0.2f, bool destroyPost = true)
        {
            if (ExplosionEffects.Count == 0)
            {
                yield break;
            }

            var effectDelay = timeToComplete / ExplosionEffectCount;
            var frameTime = 1f / 50f;
            var effectsPerFrame = 1;
            var initialDelay = effectDelay;

            // If the delay is too short, it will be rounded to the next frame.
            // So run more effects per frame and increase the effect delay instead.
            while (effectDelay < frameTime)
            {
                effectDelay += initialDelay;
                effectsPerFrame += 1;
            }

            var effects = new List<(Vector3 pos, ushort effect)>();

            for (int i = 0; i < ExplosionEffectCount; i++)
            {
                var randomEffect = ExplosionEffects[Random.Range(0, ExplosionEffects.Count)];

                var position = transform.position + (Random.insideUnitSphere * ExplosionRadius);

                effects.Add((position, randomEffect));
            }

            // Order effects inside to out of the sphere to simulate an outward explosion
            var ordered = effects.OrderBy(x => (transform.position - x.pos).sqrMagnitude);

            var queue = new Queue<(Vector3 pos, ushort effect)>();
            foreach (var order in ordered)
            {
                queue.Enqueue(order);
            }

            while(queue.Count > 0)
            {
                for (int i = 0; i < effectsPerFrame; i++)
                {
                    if (effects.Count == 0)
                        break;
                    var effect = queue.Dequeue();
                    EffectManager.sendEffect(effect.effect, EffectManager.INSANE, effect.pos);
                }
                yield return new WaitForSeconds(effectDelay);
            }

            // If set, destroy the object once the coroutine is finished
            if (destroyPost)
            {
                Destroy(Trailer);
                Destroy(this);
            }
        }
    }
}