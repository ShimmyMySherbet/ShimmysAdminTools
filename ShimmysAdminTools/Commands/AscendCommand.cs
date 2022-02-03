using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;

namespace ShimmysAdminTools.Commands
{
    public class AscendCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "Ascend";
        public string Help => "Teleports you up a certain distance";
        public string Syntax => "Ascend [dist]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.Ascend" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length > 1)
            {
                UnturnedChat.Say(caller, Syntax);
                return;
            }
            if (!float.TryParse(command[0], out var dist))
            {
                UnturnedChat.Say("Invalid distance");
                return;
            }

            var upl = caller as UnturnedPlayer;

            var newPos = upl.Position + new Vector3(0, dist, 0);

            upl.Player.teleportToLocationUnsafe(newPos, upl.Rotation);
        }
    }
}