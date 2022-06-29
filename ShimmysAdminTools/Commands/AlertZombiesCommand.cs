using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using ShimmysAdminTools.Models;
using ShimmysAdminTools.Modules;

namespace ShimmysAdminTools.Commands
{
    public class AlertZombiesCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "AlertZombies";
        public string Help => "Alerts all zombies in the area to a player";
        public string Syntax => "AlertZombies [Player]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.AlertZombies" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                UnturnedChat.Say(caller, Syntax);
                return;
            }

            var targetPlayer = PlayerUtil.GetPlayer(command[0]);

            if (targetPlayer == null)
            {
                UnturnedChat.Say(caller, "Failed to find player.");
                return;
            }

            var player = targetPlayer.Player;
            var movement = player.movement;

            if (movement.nav == byte.MaxValue)
                return;

            foreach (var zombie in ZombieManager.regions[movement.nav].zombies)
            {
                if (!zombie.isDead && zombie.checkAlert(player))
                {
                    zombie.alert(player);
                    zombie.DynSet("path", EZombiePath.RUSH);
                }
            }
        }
    }
}