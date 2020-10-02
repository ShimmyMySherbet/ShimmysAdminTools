using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Models;
using ShimmysAdminTools.Modules;
using UnityEngine;

namespace ShimmysAdminTools.Behaviors
{
    public class PointToolInputListener : MonoBehaviour
    {
        public Player Player;
        public UnturnedPlayer UPlayer;

        private bool awake = false;
        public bool Ready { get; private set; } = false;

        public bool LPunchDown = false;
        public bool RPunchDown = false;

        public void SetPlayer(Player Player)
        {
            this.Player = Player;
            UPlayer = UnturnedPlayer.FromPlayer(Player);
            Ready = true;
        }

        public void Awake()
        {
            awake = true;
            if (GetComponentInParent<Player>() != null)
            {
                Player = GetComponentInParent<Player>();
                UPlayer = UnturnedPlayer.FromPlayer(Player);
                Ready = true;
            }
        }

        public void Stop()
        {
            awake = false;
        }

        public void OnDestroy()
        {
            Stop();
        }

        public void FixedUpdate()
        {
            if (awake && Ready)
            {
                PlayerInput PInput = Player.input;
                bool LPunchState = PInput.keys[(byte)UnturnedKey.LPunch];
                bool RPunchState = PInput.keys[(byte)UnturnedKey.RPunch];
                if (LPunchState != LPunchDown)
                {
                    if (LPunchState)
                    {
                        // send fire
                        if (!Player.equipment.isSelected)
                            PointToolManager.ManageGestureUpdate(UPlayer, Rocket.Unturned.Events.UnturnedPlayerEvents.PlayerGesture.PunchLeft);
                    }
                    LPunchDown = LPunchState;
                }
                if (RPunchState != RPunchDown)
                {
                    if (RPunchState)
                    {
                        // send fire
                        if (!Player.equipment.isSelected)

                            PointToolManager.ManageGestureUpdate(UPlayer, Rocket.Unturned.Events.UnturnedPlayerEvents.PlayerGesture.PunchRight);
                    }
                    RPunchDown = RPunchState;
                }
            }
        }
    }
}