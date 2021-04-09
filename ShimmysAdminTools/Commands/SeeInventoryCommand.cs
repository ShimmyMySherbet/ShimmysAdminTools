using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
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

            if (player.Inventory.items.Length > slot)
            {
                var items = player.Inventory.items[slot];

                ucaller.Player.inventory.updateItems(7, items);
                ucaller.Player.inventory.sendStorage();
            }
            else
            {
                UnturnedChat.Say(caller, "SeeInv_Fail_BadPage".Translate());
            }
        }
    }
}