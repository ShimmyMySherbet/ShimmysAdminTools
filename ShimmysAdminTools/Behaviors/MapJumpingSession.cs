using Rocket.Core.Utils;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Models;
using ShimmysAdminTools.Modules;
using UnityEngine;

namespace ShimmysAdminTools.Behaviors
{
    public class MapJumpingSession : MonoBehaviour
    {
        public UnturnedPlayer Player;

        private bool awake = false;

        private Vector3 LastPos;

        
        private bool HasPlayer = false;

        public void SetPlayer(UnturnedPlayer player)
        {
            Player = player;
            LastPos = Player.Player.quests.markerPosition;
            HasPlayer = true;
        }

        public void Awake()
        {
            awake = true;
        }

        public void Stop()
        {
            awake = false;
            Destroy(gameObject);
        }

        public void FixedUpdate()
        {
            if (awake && HasPlayer)
            {
                if (Player.Player.quests.isMarkerPlaced)
                {
                    if (Player.Player.quests.markerPosition != LastPos)
                    {
                        LastPos = Player.Player.quests.markerPosition;
                        RaycastResult Raycast = RaycastUtility.Raycast(new Vector3(LastPos.x, 1024, LastPos.z), Vector3.down, RayMasks.GROUND | RayMasks.GROUND2 | RayMasks.STRUCTURE | RayMasks.ENVIRONMENT | RayMasks.LARGE
                            | RayMasks.MEDIUM | RayMasks.SMALL | RayMasks.RESOURCE | RayMasks.BARRICADE, 1024);
                        if (Raycast.RaycastHit)
                        {
                            Player.Teleport(new Vector3(Raycast.Raycast.point.x, Raycast.Raycast.point.y + 1, Raycast.Raycast.point.z), Player.Rotation);
                        } 
                    }
                }
            }
        }
    }
}