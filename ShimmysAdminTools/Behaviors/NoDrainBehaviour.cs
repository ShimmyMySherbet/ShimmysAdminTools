using System.Reflection;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace ShimmysAdminTools.Behaviors
{
    public class NoDrainBehaviour : MonoBehaviour
    {
        public static FieldInfo Heath = typeof(PlayerLife).GetField("_health", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo Water = typeof(PlayerLife).GetField("_water", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo Food = typeof(PlayerLife).GetField("_food", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo Virus = typeof(PlayerLife).GetField("_virus", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo Stamina = typeof(PlayerLife).GetField("_stamina", BindingFlags.NonPublic | BindingFlags.Instance);

        public static void SendHealth(PlayerLife player, byte Amount)
        {
            Heath.SetValue(player, Amount);
            player.serverModifyHealth(Amount);
        }

        public static void SendWater(PlayerLife player, byte Amount)
        {
            Water.SetValue(player, Amount);
            player.serverModifyWater(Amount);
        }

        public static void SendFood(PlayerLife player, byte Amount)
        {
            Food.SetValue(player, Amount);
            player.serverModifyFood(Amount);
        }

        public static void SendVirus(PlayerLife player, byte Amount)
        {
            Virus.SetValue(player, Amount);
            player.serverModifyVirus(Amount);
        }

        public UnturnedPlayer Player;

        public bool IsReady = false;

        public bool awake = false;

        public void Awake()
        {
            awake = true;
            Player pl = GetComponentInParent<Player>();
            if (pl != null)
            {
                IsReady = true;
                Player = UnturnedPlayer.FromPlayer(pl);
            }
        }

        public void OnDestroy()
        {
        }

        public void FixedUpdate()
        {
            if (awake && IsReady)
            {
                if (Player.Bleeding)
                {
                    Player.Bleeding = false;
                }
                if (Player.Broken)
                {
                    Player.Broken = false;
                }

                if (Player.Stamina < 90)
                {
                    Player.Player.life.serverModifyStamina(100);
                }
            }
        }
    }
}