using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;

namespace ShimmysAdminTools.Commands
{
    public class DescendCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "Descend";
        public string Help => "Teleports you down a certain distance";
        public string Syntax => "Descend [dist]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.Descend" };

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

            var newPos = upl.Position + new Vector3(0, dist * -1, 0);

            upl.Player.teleportToLocationUnsafe(newPos, upl.Rotation);
        }
    }
}
