using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace ShimmysAdminTools.Commands
{
    public class ClearInventoryCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "ClearInv";
        public string Help => "Clears your inventory";
        public string Syntax => Name;
        public List<string> Aliases => new List<string>() { "ci" };
        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.ClearInventory" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var uplayer = caller as UnturnedPlayer;

            if (command.Length > 0 && caller.HasPermission("ShimmysAdminTools.ClearInventory.Other"))
            {
                uplayer = UnturnedPlayer.FromName(command[0]);
                if (uplayer == null)
                {
                    UnturnedChat.Say(caller, "Unknown player.");
                    return;
                }
            }

            Clear(uplayer.Player.inventory);
        }

        public void Clear(PlayerInventory playerInv)
        {
            var player = playerInv.player;
            var clothing = player.clothing;
            HideWeaponModels(player);
            ClearItems(playerInv);
            ClearClothes(clothing);
        }

        private void ClearItems(PlayerInventory playerInv)
        {
            for (byte page = 0; page < 6; page++)
            {
                if (page == PlayerInventory.AREA)
                    continue;

                var count = playerInv.getItemCount(page);

                for (byte index = 0; index < count; index++)
                {
                    playerInv.removeItem(page, 0);
                }
            }
        }

        private void ClearClothes(PlayerClothing cloth)
        {
            cloth.askWearBackpack(0, 0, new byte[0], true);
            cloth.askWearGlasses(0, 0, new byte[0], true);
            cloth.askWearHat(0, 0, new byte[0], true);
            cloth.askWearPants(0, 0, new byte[0], true);
            cloth.askWearMask(0, 0, new byte[0], true);
            cloth.askWearShirt(0, 0, new byte[0], true);
            cloth.askWearVest(0, 0, new byte[0], true);

            for (byte i = 0; i < cloth.player.inventory.getItemCount(2); i++)
            {
                cloth.player.inventory.removeItem(2, 0);
            }
        }

        private void HideWeaponModels(Player player)
        {
#pragma warning disable CS0612
            player.channel.send("tellSlot", (ESteamCall)1, (ESteamPacket)15,
                new object[] {
                    0,
                    0,
                    new byte[0]
});

            player.channel.send("tellSlot", (ESteamCall)1, (ESteamPacket)15,
            new object[] {
                    1,
                    0,
                    new byte[0]
            });
        }
    }
}