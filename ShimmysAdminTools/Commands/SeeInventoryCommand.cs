using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Behaviors;
using ShimmysAdminTools.Components;

namespace ShimmysAdminTools.Commands
{
    public class SeeInventoryCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "SeeInv";

        public string Help => "Opens another player's inventory";

        public string Syntax => "SeeInv [Player] (Page)";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.SeeInventory" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (!AdminToolsPlugin.Config.EnableExperimentalCommands)
            {
                UnturnedChat.Say(caller, "Experimental_Disabled".Translate());
                return;
            }

            UnturnedChat.Say(caller, "SeeInv_Experimental".Translate());

            if (command.Length < 1)
            {
                UnturnedChat.Say(caller, Syntax);
                return;
            }
            UnturnedPlayer ucaller = (UnturnedPlayer)caller;

            UnturnedPlayer player = UnturnedPlayer.FromName(command[0]);
            if (player == null)
            {
                UnturnedChat.Say(caller, "SeeInv_Fail_NoPlayer".Translate());
                return;
            }

            byte slot = 3;

            if (command.Length >= 2)
            {
                string slotHandle = command[1];
                if (!byte.TryParse(slotHandle, out slot))
                {
                    UnturnedChat.Say(caller, "SeeInv_Fail_BadPage".Translate());
                    return;
                }
            }
            if (player.Inventory.items.Length > slot)
            {
                SeeInventoryBehaviour b = ucaller.Player.gameObject.AddComponent<SeeInventoryBehaviour>();
                b.SendOpenInventory(player.Player, slot);
            }
            else
            {
                UnturnedChat.Say(caller, "SeeInv_Fail_BadPage".Translate());
            }
        }
    }
}