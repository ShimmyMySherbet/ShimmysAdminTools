using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Modules;

namespace ShimmysAdminTools.Commands
{
    public class ForceTPCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "FTP";

        public string Help => "Forcibly teleports to a player";

        public string Syntax => "FTP [Player] (Player)";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.ForceTP" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                UnturnedChat.Say(caller, Syntax);
            }
            else if (command.Length == 1)
            {
                if (caller is ConsolePlayer)
                {
                    UnturnedChat.Say(caller, "ForceTP_Fail_IsConsole".Translate());
                    return;
                }
                if (PlayerSelector.GetPlayer(command[0], out UnturnedPlayer player))
                {
                    ((UnturnedPlayer)caller).Player.teleportToLocationUnsafe(player.Position, player.Rotation);
                    UnturnedChat.Say(caller, "ForceTP_Pass_TeleportSelfTo".Translate(player.DisplayName));
                }
                else
                {
                    UnturnedChat.Say(caller, "ForceTP_Fail_NoPlayer".Translate());
                }
            }
            else
            {
                if (PlayerSelector.GetPlayer(command[0], out UnturnedPlayer player1) && PlayerSelector.GetPlayer(command[1], out UnturnedPlayer player2))
                {
                    player2.Player.teleportToLocationUnsafe(player1.Position, player1.Rotation);
                    UnturnedChat.Say(caller, "ForceTP_Pass_TeleportOtherTo".Translate(player1.DisplayName, player2.DisplayName));
                }
                else
                {
                    UnturnedChat.Say(caller, "ForceTP_Fail_NoPlayer".Translate());
                }
            }
        }
    }
}