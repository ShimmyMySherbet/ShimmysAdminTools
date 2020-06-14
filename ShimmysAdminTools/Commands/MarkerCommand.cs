using Rocket.API;
using Rocket.Unturned.Chat;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimmysAdminTools.Commands
{
    public class MarkerCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "Marker";

        public string Help => "Places a marker on your current position";

        public string Syntax => "Marker [name]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.Marker" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            PlayerSession Session = caller.GetSession();
            if (command.Length >= 1)
            {
                string MarkerName = command[0];
                Session.Markers[MarkerName.ToLower()] = caller.UPlayer().Position;
                UnturnedChat.Say(caller, "Marker_Placed".Translate(MarkerName));
            } else
            {
                UnturnedChat.Say(caller, Syntax);
            }
        }
    }
}
