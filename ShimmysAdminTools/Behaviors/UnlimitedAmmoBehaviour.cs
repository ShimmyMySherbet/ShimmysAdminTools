using System;
using System.Collections.Generic;
using SDG.Unturned;
using UnityEngine;

namespace ShimmysAdminTools.Behaviors
{
    public class UnlimitedAmmoBehaviour : MonoBehaviour
    {
        public Player Player;
        private bool awake = false;
        public bool Ready { get; protected set; } = false;

        public byte AmountOverride = 255;
        public bool AmountOverrideEnabled = false;
        public int LastSendAmmo = -1;
        public ushort LastKnownMag = 0;


        public void Awake()
        {
            awake = true;
            Player ParentPlayer = GetComponentInParent<Player>();
            if (ParentPlayer != null)
            {
                Ready = true;
                Player = ParentPlayer;
            }
        }

        public void FixedUpdate()
        {
            if (awake && Ready)
            {
                if (Player.equipment.asset == null || Player.equipment.asset.type != EItemType.GUN || Player.equipment.state == null || Player.equipment.state.Length < 10)
                {
                    return;
                }
                byte Ammo = Player.equipment.state[10];
                ushort MagID = BitConverter.ToUInt16(Player.equipment.state, 8);
                if (LastKnownMag != MagID && MagID != 0) { LastKnownMag = MagID;
                }
                if (AmountOverrideEnabled)
                {
                    if (Ammo != AmountOverride)
                    {
                        if (MagID == 0)
                        {
                            byte[] ShortBuffer = BitConverter.GetBytes(LastKnownMag);
                            Array.Copy(ShortBuffer, 0, Player.equipment.state, 8, 2);
                        }
                        SendAmmo(AmountOverride);
                    }
                }
                else
                {
                    ItemAsset AST = (ItemAsset)Assets.find(EAssetType.ITEM, MagID);
                    if (AST != null && AST is ItemMagazineAsset Mag)
                    {
                        if (Ammo < Mag.amount)
                        {
                            SendAmmo(Mag.amount);
                        }
                    }
                    else if (MagID == 0)
                    {
                        ItemAsset LastKnownAsset = (ItemAsset)Assets.find(EAssetType.ITEM, LastKnownMag);
                        if (LastKnownAsset != null && LastKnownAsset is ItemMagazineAsset RepMag)
                        {
                            byte[] ShortBuffer = BitConverter.GetBytes(LastKnownMag);
                            Array.Copy(ShortBuffer, 0, Player.equipment.state, 8, 2);
                            SendAmmo(RepMag.amount);
                        }
                    }
                    else if (Ammo < Player.equipment.asset.countMax)
                    {
                        if (LastSendAmmo != Player.equipment.asset.countMax)
                        {
                            LastSendAmmo = Player.equipment.asset.countMax;
                            SendAmmo(Player.equipment.asset.countMax);
                        }
                    }
                }
            }
        }

        public void SendAmmo(byte Ammo)
        {
            Player.equipment.state[10] = Ammo;
            Player.equipment.sendUpdateState();
        }

        public void Stop()
        {
            awake = false;
        }

        public void OnDestroy()
        {
            Stop();
        }
    }
}