using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Models;
using ShimmysAdminTools.Modules;

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
            if (command.Length > 0)
            {
                UnturnedPlayer TargetPlayer = UnturnedPlayer.FromName(command[0]);
                if (TargetPlayer != null)
                {
                    PlayerSession Session = PlayerSessionStore.GetPlayerData(TargetPlayer);
                    if (Session == null) return;
                    if (Session.FlySessionActive)
                    {
                        // Stop Session
                        Session.StartFlightSession();
                        UnturnedChat.Say(caller, "Flight_Disabled_Other".Translate(TargetPlayer.DisplayName));
                        UnturnedChat.Say(TargetPlayer, "Flight_Disabled".Translate());
                    }
                    else
                    {
                        // Start Session
                        Session.StartFlightSession();
                        UnturnedChat.Say(caller, "Flight_Enabled_Other".Translate(TargetPlayer.DisplayName));
                        UnturnedChat.Say(TargetPlayer, "Flight_Enabled".Translate());
                    }
                } else
                {
                    UnturnedChat.Say(caller, "Error_PlayerNotFound".Translate());
                }
            }
            else
            {
                UnturnedPlayer Player = (UnturnedPlayer)caller;
                PlayerSession Session = PlayerSessionStore.GetPlayerData(Player);
                if (Session.FlySessionActive)
                {
                    // Stop Session
                    Session.StopFlightSession();
                    UnturnedChat.Say(caller, "Flight_Disabled".Translate());
                }
                else
                {
                    // Start Session
                    Session.StartFlightSession();
                    UnturnedChat.Say(caller, "Flight_Enabled".Translate());
                }
            }
        }
    }
}