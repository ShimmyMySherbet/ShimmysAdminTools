using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Models;
using ShimmysAdminTools.Modules;

namespace ShimmysAdminTools.Commands
{
    public class CommandSpyCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "CommandSpy";

        public string Help => "Allows you to see other player's commands.";

        public string Syntax => "CommandSpy (Player)";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.CommandSpy" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            PlayerSession Session = PlayerSessionStore.GetPlayerData((UnturnedPlayer)caller);
            if (command.Length == 0)
            {
                if (Session.IsSpyingCommands)
                {
                    Session.DisableCommandSpy();
                    UnturnedChat.Say(caller, "CommandSpy_Disabled".Translate());
                } else
                {
                    Session.CommandSpyGlobalEnabled = true;
                    UnturnedChat.Say(caller, "CommandSpy_Enabled_Global".Translate());
                }
            } else
            {
                UnturnedPlayer Target = UnturnedPlayer.FromName(command[0]);
                if (Target == null)
                {
                    UnturnedChat.Say(caller, "Error_PlayerNotFound".Translate());
                } else
                {
                    ulong PlayerID = Target.CSteamID.m_SteamID;
                    if (Session.CommandSpyPlayers.Contains(PlayerID))
                    {
                        UnturnedChat.Say(caller, "CommandSpy_Disabled_Player".Translate(Target.DisplayName));
                        Session.CommandSpyPlayers.Remove(PlayerID);
                    } else
                    {
                        UnturnedChat.Say(caller, "CommandSpy_Enabled_Player".Translate(Target.DisplayName));
                        Session.CommandSpyPlayers.Add(PlayerID);
                    }
                }
            }
        }
    }
}
