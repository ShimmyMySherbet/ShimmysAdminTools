using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Models;
using ShimmysAdminTools.Modules;

namespace ShimmysAdminTools.Commands
{
    public class MapJumpCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "MapJump";

        public string Help => "Toggles waypoint teleportation.";

        public string Syntax => "Mapjump";

        public List<string> Aliases => new List<string>() { "MapJumping" };

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.MapJump" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (AdminToolsPlugin.Config.DisableAbusableCommands)
            {
                UnturnedChat.Say(caller, "Fail_Command_Disabled".Translate());
                return;
            }
            PlayerSession session = PlayerSessionStore.GetPlayerData((UnturnedPlayer)caller);

            if (session.MapJumpingSession == null)
            {
                session.StartMapJumpingSession();
                UnturnedChat.Say(caller, "MapJump_Enabled".Translate());
            }
            else
            {
                session.StopMapJumpingSession();
                UnturnedChat.Say(caller, "MapJump_Disabled".Translate());
            }
        }
    }
}