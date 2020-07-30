using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace ShimmysAdminTools.Behaviors
{
    public class FiremodeUnlocker : MonoBehaviour
    {
        public Player Player;

        private bool awake = false;
        public bool Ready { get; private set; } = false;

        private byte LastFireMode = 0;
        private bool SetStateNextUpdate = true;

        public void SetPlayer(Player Player)
        {
            this.Player = Player;
            Ready = true;
        }

        public void Awake()
        {
            awake = true;
            if (GetComponentInParent<Player>() != null)
            {
                Player = GetComponentInParent<Player>();
                Ready = true;   
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

                if (!Player.equipment.isEquipped)
                {
                    SetStateNextUpdate = true;
                    return;
                }

                byte CurrentFireMode = Player.equipment.state[11];
                if (SetStateNextUpdate)
                {
                    LastFireMode = CurrentFireMode;
                    SetStateNextUpdate = false;
                }

                if (CurrentFireMode != LastFireMode)
                {
                    OnFiremodeChanged(LastFireMode, CurrentFireMode, out byte NewFireMode);
                    LastFireMode = NewFireMode;
                }
            }
        }

        public void OnFiremodeChanged(byte LastFireMode, byte NewFireMode, out byte SetFireMode)
        {
            SetFireMode = NewFireMode;
            byte NextFireMode = GetNextFireMode(LastFireMode);
            if (NextFireMode == NewFireMode)
            {
                return;
            };
            SetFireMode = NewFireMode;
            SendFireMode(NextFireMode);
        }

        private byte GetNextFireMode(byte FireMode)
        {
            if (FireMode >= 3) return 0;
            return (byte)(FireMode + 1);
        }

        public void SendFireMode(byte FireMode)
        {
            if (Player.equipment.state == null || Player.equipment.state.Length < 10) return;
            Player.equipment.state[11] = FireMode;
            SetStateNextUpdate = true;
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