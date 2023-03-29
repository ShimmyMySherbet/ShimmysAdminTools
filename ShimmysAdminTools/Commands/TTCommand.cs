using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using ShimmysAdminTools.Models;
using UnityEngine;

namespace ShimmysAdminTools.Commands
{
    public class TTCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "TTR";
        public string Help { get; } = "Experimental turret save state command";
        public string Syntax { get; } = "";
        public List<string> Aliases { get; } = new List<string>();
        public List<string> Permissions { get; } = new List<string>() { "ShimmysAdminTools.Experimental.TTR" };

        private static byte[] m_state;
        private static ushort m_item;

        public void Execute(IRocketPlayer caller, string[] command)
        {
			UnturnedChat.Say(caller, "Warning: This is an experimental command that can cause bugs and broken states.");

			var pl = caller as UnturnedPlayer;
            var player = pl.Player;
            var vehicle = pl.CurrentVehicle;
            var eq = player.equipment;

            if (pl.IsInVehicle)
            {
                vehicle.findPlayerSeat(pl.Player, out var seatIndex);

                var seat = vehicle.passengers[seatIndex];
            }

            if (command.Length == 0)
            {
                return;
            }
            switch (command[0].ToLower())
            {
                case "savestate":
                    m_state = eq.state;
                    m_item = eq.asset.id;
                    break;

                case "deploy":
                    eq.turretEquipServer(m_item, m_state);
                    break;

                case "deployman":
                    bool simServer = true;
                    bool serverPop = true;

                    if (command.Length >= 2)
                        simServer = bool.Parse(command[1]);

                    if (command.Length >= 3)
                        serverPop = bool.Parse(command[2]);

                    SendEquipManual(eq, m_item, m_state, simServer, serverPop);
                    break;
            }
        }

        private void SendEquipManual(PlayerEquipment eq, ushort id, byte[] state, bool simServerSide = false, bool serverPopulate = true)
        {
            Guid guid = Assets.find(EAssetType.ITEM, id)?.GUID ?? Guid.Empty;
            NetId netId = NetIdRegistry.Claim();
            var SendEquip = typeof(PlayerEquipment).DynGet<ClientInstanceMethod<byte, byte, byte, Guid, byte, byte[], NetId>>("SendEquip");
            SendEquip.Invoke(eq.GetNetId(), ENetReliability.Reliable, Provider.EnumerateClients_Remote(), 254, 254, 254, guid, 100, state, netId);

            if (serverPopulate)
            {
                if (simServerSide)
                {
                    ReceiveEquipMock(eq, 254, 254, 254, guid, 100, state, netId);
                }
                else
                {
                    eq.ReceiveEquip(254, 254, 254, guid, 100, state, netId);
                }
            }
        }

        public void ReceiveEquipMock(PlayerEquipment eq, byte page, byte x, byte y, Guid newAssetGuid, byte newQuality, byte[] newState, NetId useableNetId)
        {
            var asset = Assets.find(newAssetGuid) as ItemAsset;
            eq.DynSet("slot", page);
            eq.DynSet("_equippedPage", page);
            eq.DynSet("_equipped_x", x);
            eq.DynSet("_equipped_y", y);
            eq.DynSet("_asset", asset);
            eq.DynSet("_quality", newQuality);
            eq.DynSet("_state", newState);
            if (asset.useableType != null)
            {

                var useable = eq.gameObject.AddComponent(asset.useableType) as Useable;
                eq.DynSet("_useable", useable);
                if (eq.TryDynInvoke("AssignNetId", useableNetId))
                {
                    Console.WriteLine($"Set Net ID!");
                }
                else
                {
                    Console.WriteLine($"Failed to find net set method.");
                }
            }
            eq.DynSet("lastEquipped", Time.realtimeSinceStartup);
        }
    }
}