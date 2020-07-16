using Rocket.Core.Utils;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Modules;
using UnityEngine;

namespace ShimmysAdminTools.Models
{
    public class MapJumpingSession : MonoBehaviour
    {
        public UnturnedPlayer Player;

        private bool awake = false;

        public GameObject MyGameObject;

        private Vector3 LastPos;

        //private object LockObject = new object();
        private bool HasPlayer = false;
        //private bool GotPlayer = false;

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
            Destroy(MyGameObject);
        }

        private void FixedUpdate()
        {
            if (awake && HasPlayer)
            {
                //System.Console.Write(" <update> ");
                if (Player.Player.quests.isMarkerPlaced)
                {
                    if (Player.Player.quests.markerPosition != LastPos)
                    {
                        LastPos = Player.Player.quests.markerPosition;
                        System.Console.WriteLine("try marker tp");
                        RaycastResult Raycast = RaycastUtility.Raycast(new Vector3(LastPos.x, 1024, LastPos.z), Vector3.down, RayMasks.GROUND | RayMasks.GROUND2 | RayMasks.STRUCTURE | RayMasks.ENVIRONMENT | RayMasks.LARGE
                            | RayMasks.MEDIUM | RayMasks.SMALL | RayMasks.RESOURCE | RayMasks.BARRICADE, 1024);
                        if (Raycast.RaycastHit)
                        {
                            System.Console.WriteLine($"try tp");
                            Player.Teleport(new Vector3(Raycast.Raycast.point.x, Raycast.Raycast.point.y + 1, Raycast.Raycast.point.z), Player.Rotation);
                            System.Console.WriteLine("tped");
                        } else
                        {
                            System.Console.WriteLine($"did not hit!");
                        }
                    }
                }
            }
        }
    }
}