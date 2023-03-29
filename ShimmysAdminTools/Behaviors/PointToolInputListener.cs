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
        public FieldAccessor<PlayerInput, object> LeftPunch { get; private set; }
        public FieldAccessor<PlayerInput, object> RightPunch { get; private set; }

		public void SetPlayer(Player Player)
        {
            this.Player = Player;
            UPlayer = UnturnedPlayer.FromPlayer(Player);
            Ready = true;
        }

        public void Awake()
        {
            if (GetComponentInParent<Player>() != null)
            {
                Player = GetComponentInParent<Player>();
                UPlayer = UnturnedPlayer.FromPlayer(Player);
                Ready = true;

				LeftPunch = new FieldAccessor<PlayerInput, object>("pendingPrimaryAttackInput", Player.input);
				RightPunch = new FieldAccessor<PlayerInput, object>("pendingSecondaryAttackInput", Player.input);
			}

			awake = true;
		}

		public void Stop()
        {
            awake = false;
        }

        public void OnDestroy()
        {
            Stop();
        }

        private int m_Delay = 0;
        public void FixedUpdate()
        {
            if (awake && Ready)
            {
                m_Delay++;
                if (m_Delay < 3)
                    return;
                m_Delay = 0;

                var LPunchState = (int)LeftPunch.Value > 0;
                var RPunchState = (int)RightPunch.Value > 0;

                if (LPunchState != LPunchDown)
                {
                    if (LPunchState)
                    {
                        // send fire
                        if (!Player.equipment.isSelected || Player.equipment.asset.id == 333) // allow the use of binoculars
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