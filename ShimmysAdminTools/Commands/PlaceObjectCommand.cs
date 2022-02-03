using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Models;
using UnityEngine;

namespace ShimmysAdminTools.Commands
{
    public class PlaceObjectCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "PlaceObject";
        public string Help => "Creates and places a structure/barricade";
        public string Syntax => "PlaceObject [Barricade/Structure]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.PlaceObject" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 1)
            {
                UnturnedChat.Say(caller, Syntax);
                return;
            }
            var upl = caller as UnturnedPlayer;
            var handle = command[0];

            var brAst = FindBarricade(handle);
            if (brAst != null)
            {
                if (PlaceBarricade(upl, brAst) != null)
                {
                    UnturnedChat.Say(caller, $"Placed {brAst.itemName}");
                }
                else
                {
                    UnturnedChat.Say(caller, $"Failed to place {brAst.itemName}");
                }
                return;
            }

            var struc = FindStructure(handle);
            if (struc != null)
            {
                if (PlaceStructure(upl, struc))
                {
                    UnturnedChat.Say(caller, $"Placed {struc.itemName}");
                }
                else
                {
                    UnturnedChat.Say(caller, $"Failed to place {struc.itemName}");
                }
            }
        }

        private bool PlaceStructure(UnturnedPlayer upl, ItemStructureAsset structureAsset)
        {
            var struc = new Structure(structureAsset, structureAsset.health);

            return StructureManager.dropReplicatedStructure(struc, upl.Position, Quaternion.Euler(-90f, upl.Rotation, 0f), upl.CSteamID.m_SteamID, upl.Player.quests.groupID.m_SteamID);
        }

        private Transform PlaceBarricade(UnturnedPlayer upl, ItemBarricadeAsset barricadeAsset)
        {
            var b = new Barricade(barricadeAsset);

            var transform = BarricadeManager.dropNonPlantedBarricade(b, upl.Position, Quaternion.Euler(-90f, upl.Rotation, 0f), upl.CSteamID.m_SteamID, upl.Player.quests.groupID.m_SteamID);
            if (transform == null)
            {
                return null;
            }
            if (transform.gameObject.GetComponent<Interactable>() != null)
            {
                var store = transform.gameObject.GetComponent<Interactable>();

                store.TryDynSet("_owner", upl.CSteamID);
                store.TryDynSet("_group", upl.Player.quests.groupID);
                store.TryDynInvoke("rebuildState");

                if (store is InteractableDoor door)
                {
                    var nState = new byte[17];
                    Array.Copy(BitConverter.GetBytes(upl.CSteamID.m_SteamID), 0, nState, 0, 8);
                    Array.Copy(BitConverter.GetBytes(upl.Player.quests.groupID.m_SteamID), 0, nState, 8, 8);
                    nState[16] = door.isOpen ? (byte)1 : (byte)0;
                    b.state = nState;
                }
                else if (store is InteractableSign sign)
                {
                    var nState = new byte[17];
                    Array.Copy(BitConverter.GetBytes(upl.CSteamID.m_SteamID), 0, nState, 0, 8);
                    Array.Copy(BitConverter.GetBytes(upl.Player.quests.groupID.m_SteamID), 0, nState, 8, 8);
                    nState[16] = 0;
                    b.state = nState;
                }
                else if (store is InteractableSentry)
                {
                    var baseState = new byte[16];
                    Array.Copy(BitConverter.GetBytes(upl.CSteamID.m_SteamID), 0, baseState, 0, 8);
                    Array.Copy(BitConverter.GetBytes(upl.Player.quests.groupID.m_SteamID), 0, baseState, 8, 8);
                    b.state = baseState;
                }

                BarricadeManager.updateReplicatedState(transform, b.state, b.state.Length);
            }

            return transform;
        }

        private ItemBarricadeAsset FindBarricade(string handle)
        {
            if (ushort.TryParse(handle, out var itemID))
            {
                var ast = Assets.find(EAssetType.ITEM, itemID);

                if (ast == null || !(ast is ItemBarricadeAsset bca))
                {
                    return null;
                }
                else
                {
                    return bca;
                }
            }
            else
            {
                var src = Assets.find(EAssetType.ITEM);
                return (ItemBarricadeAsset)src.FirstOrDefault(x => x is ItemBarricadeAsset bca && bca.itemName != null && bca.itemName.IndexOf(handle, 0, StringComparison.InvariantCultureIgnoreCase) != -1);
            }
        }

        private ItemStructureAsset FindStructure(string handle)
        {
            if (ushort.TryParse(handle, out var itemID))
            {
                var ast = Assets.find(EAssetType.ITEM, itemID);

                if (ast == null || !(ast is ItemStructureAsset bca))
                {
                    return null;
                }
                else
                {
                    return bca;
                }
            }
            else
            {
                var src = Assets.find(EAssetType.ITEM);
                return (ItemStructureAsset)src.FirstOrDefault(x => x is ItemStructureAsset bca && bca.itemName != null && bca.itemName.IndexOf(handle, 0, StringComparison.InvariantCultureIgnoreCase) != -1);
            }
        }
    }
}