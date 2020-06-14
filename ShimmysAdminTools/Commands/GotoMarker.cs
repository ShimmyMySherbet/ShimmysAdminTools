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
    public class GotoMarker : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "GotoMarker";

        public string Help => "Teleports you to one of your markers";

        public string Syntax => "GotoMarker [Name]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.Marker" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            PlayerSession Session = caller.GetSession();
            if (command.Length >= 1)
            {
                string Name = command[0];
                if (Session.Markers.ContainsKey(Name.ToLower()))
                {
                    caller.UPlayer().Teleport(Session.Markers[Name.ToLower()], caller.UPlayer().Rotation);
                    UnturnedChat.Say(caller, "GotoMarker_Teleported".Translate(Name));
                } else
                {
                    UnturnedChat.Say(caller, $"GotoMarker_NoMarker".Translate(Name));
                }
            }
        }
    }
}
