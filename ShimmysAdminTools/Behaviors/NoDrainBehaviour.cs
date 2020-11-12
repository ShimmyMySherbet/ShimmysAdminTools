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
            player.channel.send("tellHealth", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, Amount);
        }

        public static void SendWater(PlayerLife player, byte Amount)
        {
            Water.SetValue(player, Amount);
            player.channel.send("tellWater", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, Amount);
        }


        public static void SendFood(PlayerLife player, byte Amount)
        {
            Food.SetValue(player, Amount);
            player.channel.send("tellFood", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, Amount);
        }

     
        public static void SendVirus(PlayerLife player, byte Amount)
        {
            Virus.SetValue(player, Amount);
            player.channel.send("tellVirus", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, Amount);

        }

  


        public UnturnedPlayer Player;

        public bool IsReady = false;

        public bool awake = false;
        public bool IsGodMode = false;
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

                
                if (IsGodMode)
                {
                    if (Player.Thirst < 240)
                    {
                        SendWater(Player.Player.life, 250);
                    }
                    if (Player.Health < 240)
                    {
                        SendHealth(Player.Player.life, 250);
                    }

                    if (Player.Hunger < 240)
                    {
                        SendFood(Player.Player.life, 250);
                    }

                    if (Player.Player.life.virus < 240)
                    {
                        SendVirus(Player.Player.life, 250);
                    }
                    if (Player.Stamina <= 95)
                    {
                        Player.Player.life.serverModifyStamina(100);
                    }
                } else
                {
                    if (Player.Stamina < 90)
                    {
                        Player.Player.life.serverModifyStamina(100);
                    }
                }
            }
        }
    }
}