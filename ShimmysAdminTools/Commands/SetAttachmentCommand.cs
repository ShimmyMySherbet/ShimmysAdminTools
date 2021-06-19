using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Components;

namespace ShimmysAdminTools.Commands
{
    public class SetAttachmentCommand : IRocketCommand
    {
        readonly static EItemType[] Types = new EItemType[] { EItemType.BARREL, EItemType.GRIP, EItemType.MAGAZINE, EItemType.SIGHT, EItemType.TACTICAL };
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "SetAttachment";

        public string Help => "Forcibly gives your gun an attachment or magazine.";

        public string Syntax => "SetAttachment [Attachment Name/ID]";

        public List<string> Aliases => new List<string>() { "Attachment", "Att" };

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.SetAttachment" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (AdminToolsPlugin.Config.DisableAbusableCommands)
            {
                UnturnedChat.Say(caller, "Fail_Command_Disabled".Translate());
                return;
            }
            UnturnedPlayer Player = (UnturnedPlayer)caller;
            if (command.Length > 0)
            {
                if (Player.Player.equipment.state == null || Player.Player.equipment.state.Length < 12 || Player.Player.equipment.asset == null || Player.Player.equipment.asset.type != EItemType.GUN)
                {
                    UnturnedChat.Say(caller, "SetAttachment_Fail_Gun".Translate());
                    return;
                }

                ItemAsset Item = null;

                if (ushort.TryParse(command[0], out ushort ItemID))
                {
                    Asset SelectAsset = Assets.find(EAssetType.ITEM, ItemID);
                    if (SelectAsset != null && typeof(ItemAsset).IsAssignableFrom(SelectAsset.GetType()) && Types.Contains(((ItemAsset)SelectAsset).type))
                    {
                        Item = (ItemAsset)SelectAsset;
                    }
                }

                if (Item == null)
                {
                    ItemAsset[] Ast = Assets.find(EAssetType.ITEM).Where(x => typeof(ItemAsset).IsAssignableFrom(x.GetType()) && 
                        Types.Contains(((ItemAsset)x).type) && 
                        ((ItemAsset)x).itemName.ToLower().Contains(command[0].ToLower()))
                        .Cast<ItemAsset>()
                        .ToArray();
                    if (Ast.Length != 0) Item = Ast[0];
                }

                if (Item != null)
                {
                    if (AdminToolsPlugin.Config.BlacklistedAttachments.Contains(Item.id))
                    {
                        UnturnedChat.Say(caller, "SetAttachment_Fail_Blacklist".Translate());
                        return;
                    }

                    byte pos = 255;
                    if (Item.type == EItemType.SIGHT)
                    {
                        pos = 0;
                    }
                    else if (Item.type == EItemType.TACTICAL)
                    {
                        pos = 2;
                    }
                    else if (Item.type == EItemType.GRIP)
                    {
                        pos = 4;
                    }
                    else if (Item.type == EItemType.BARREL)
                    {
                        pos = 6;
                    }
                    else if (Item.type == EItemType.MAGAZINE)
                    {
                        pos = 8;
                    }
                    if (pos == 255) return;
                    byte[] ID = BitConverter.GetBytes(Item.id);
                    Array.Copy(ID, 0, Player.Player.equipment.state, pos, 2);
                    Player.Player.equipment.sendUpdateState();
                    UnturnedChat.Say(caller, "SetAttachment_GaveAttachment".Translate(Item.itemName));
                }
                else
                {
                    UnturnedChat.Say(caller, "SetAttachment_Fail_Item".Translate());
                }
            }
            else
            {
                UnturnedChat.Say(caller, Syntax);
            }
        }
    }
}