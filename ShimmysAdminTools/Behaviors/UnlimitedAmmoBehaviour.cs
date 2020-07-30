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

        private void FixedUpdate()
        {
            if (awake && Ready)
            {
                if (Player.equipment.asset == null || Player.equipment.asset.type != EItemType.GUN || Player.equipment.state == null || Player.equipment.state.Length < 10)
                {
                    return;
                }
                byte Ammo = Player.equipment.state[10];
                if (AmountOverrideEnabled)
                {
                    if (Ammo != 255)
                    {
                        SendAmmo(AmountOverride);
                    }
                }
                else
                {
                    if (Player.equipment.asset is ItemGunAsset)
                    {
                        ItemGunAsset Gun = (ItemGunAsset)Player.equipment.asset;
                        if (Ammo < Gun.ammoMax)
                        {
                            SendAmmo(Gun.ammoMax);
                        }
                    }
                    else if (Ammo < Player.equipment.asset.countMax)
                    {
                        SendAmmo(Player.equipment.asset.countMax);
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

        private void OnDestroy()
        {
            Stop();
        }
    }
}