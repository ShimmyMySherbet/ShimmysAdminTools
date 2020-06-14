using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimmysAdminTools.Commands
{
    public class NoClipCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "NoClip";

        public string Help => "Toggles NoClip for use with AFly";

        public string Syntax => Name;

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.NoClip" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            PlayerSession Session = caller.GetSession();
            if (Session.NoClipSessionActive)
            {
                Session.NoClipSessionActive = false;
                Session.NoClip.Stop();
                UnturnedChat.Say(caller, "Noclip_Disabled".Translate());
            } else
            {
                Session.NoClipSessionActive = true;
                Session.NoClip = new Modules.NoClippingTool((UnturnedPlayer)caller);
                Session.NoClip.Start();
                UnturnedChat.Say(caller, "Noclip_Enabled".Translate());
            }
        }
    }
}
