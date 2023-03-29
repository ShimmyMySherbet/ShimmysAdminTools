using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Modules;

namespace ShimmysAdminTools.Commands
{
    public class ForceEquipCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "ForceEquip";
        public string Help => "Forcibly equips an item";
        public string Syntax => "ForceEquip [Item Slot] {Player}";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.Experimental.ForceEquip" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedChat.Say(caller, "Warning: This is an experimental command that can cause bugs and broken states.");
            Player player;
            if (command.Length >= 2)
            {
                player = PlayerUtil.GetPlayer(command[1])?.Player;
                if (player == null)
                {
                    UnturnedChat.Say(caller, "Failed to find player");
                    return;
                }
            }
            else if (caller is UnturnedPlayer upl)
            {
                player = upl.Player;
            }
            else
            {
                UnturnedChat.Say(caller, "Player must be specified.");
                return;
            }

            var eq = player.equipment;

            byte equipSlot = 0;
            byte page = 0;
            ItemJar selectItem;

            if (command.Length >= 1)
            {
                if (!byte.TryParse(command[0], out equipSlot))
                {
                    UnturnedChat.Say(caller, "Invalid slot.");
                    return;
                }
            }

            if (equipSlot > 9)
            {
                UnturnedChat.Say(caller, "Invalid Slot");
                return;
            }

            switch (equipSlot)
            {
                case 0:
                case 1:
                    selectItem = player.inventory.items[equipSlot].items.FirstOrDefault();
                    page = equipSlot;
                    break;

                default:
                    var hk = eq.hotkeys[equipSlot - 2];
                    page = hk.page;
                    selectItem = player.inventory.items[hk.page].items.FirstOrDefault(x => x.x == hk.x && x.y == hk.y);
                    break;
            }

            if (selectItem == null)
            {
                UnturnedChat.Say(caller, "No item in selected slot.");
                return;
            }

            var vehicle = player.movement.getVehicle();

            if (vehicle != null)
            {
                var itemID = selectItem.item.id;
                var itemState = selectItem.item.state;
                eq.turretEquipServer(itemID, itemState);
            }
            else
            {
                eq.tryEquip(page, selectItem.x, selectItem.y);
            }
        }
    }
}