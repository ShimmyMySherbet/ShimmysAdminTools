using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Models;
using ShimmysAdminTools.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimmysAdminTools.Commands
{
    public class AFlyCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "AFly";

        public string Help => "Toggles Flight";

        public string Syntax => "AFly";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.Flight" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer Player = (UnturnedPlayer)caller;
            PlayerSession Session = PlayerSessionStore.GetPlayerData(Player);

            if (Session.FlySessionActive)
            {
                // Stop Session
                Session.FlySession.Stop();
                Session.FlySessionActive = false;
                UnturnedChat.Say(caller, "Flight_Disabled".Translate());
            } else
            {
                // Start Session
                Session.FlySession = new FlySession();
                Session.FlySession.Player = Player;
                Session.FlySession.Start();
                Session.FlySessionActive = true;
                UnturnedChat.Say(caller, "Flight_Enabled".Translate());
            }
        }
    }
}
