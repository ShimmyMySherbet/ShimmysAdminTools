using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using ShimmysAdminTools.Models;
using ShimmysAdminTools.Modules;

namespace ShimmysAdminTools.Commands
{
    public class SimFireCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "SimFire";
        public string Help => "Triggers the shoot effect of a player's gun";
        public string Syntax => "SimFire [player]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.SimFire" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer targetPlayer;

            if (command.Length > 0)
            {
                targetPlayer = PlayerUtil.GetPlayer(command[0]);
            }
            else
            {
                if (caller is UnturnedPlayer pl)
                {
                    targetPlayer = pl;
                }
                else
                {
                    UnturnedChat.Say(caller, "Must supply target played when in console.");
                    return;
                }
            }

            if (targetPlayer == null)
            {
                UnturnedChat.Say(caller, "Failed to find player.");
                return;
            }

            var asset = targetPlayer.Player.equipment.useable;
            if (asset != null && asset is UseableGun gun)
            {
                var send = typeof(UseableGun).DynGet<ClientInstanceMethod>("SendPlayShoot");
                send.Invoke(gun.GetNetId(), ENetReliability.Unreliable, gun.channel.GetOwnerTransportConnection());
            }
        }
    }
}