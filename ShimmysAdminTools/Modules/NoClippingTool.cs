using System.Threading.Tasks;
using Rocket.Core.Utils;
using Rocket.Unturned.Player;
using SDG.Framework.Landscapes;
using SDG.Unturned;
using ShimmysAdminTools.Models;
using UnityEngine;

namespace ShimmysAdminTools.Modules
{
    public class NoClippingTool
    {
        public UnturnedPlayer Player;
        public bool Active = false;
        public int ClipUpdateRate = 300;

        public NoClippingTool(UnturnedPlayer Player)
        {
            this.Player = Player;
        }

        public void Start()
        {
            Active = true;
            Task.Run(NoClipTick);
        }

        public void Stop()
        {
            Active = false;
        }

        private async Task NoClipTick()
        {
            while (Active)
            {
                var ClipCast = RaycastUtility.RayCastPlayer(Player, RayMasks.AGENT | RayMasks.BARRICADE | RayMasks.STRUCTURE | RayMasks.SMALL | RayMasks.MEDIUM | RayMasks.LARGE | RayMasks.ENVIRONMENT, 10);

                if (ClipCast.RaycastHit && ClipCast.Raycast.distance <= 2)
                {
                    Vector3 TargetPoint = new Vector3(ClipCast.Raycast.point.x, ClipCast.Raycast.point.y - Player.Player.look.heightLook, ClipCast.Raycast.point.z);

                    if (VectorInWorld(TargetPoint))
                    {
                        TaskDispatcher.QueueOnMainThread(delegate { Player.Player.teleportToLocationUnsafe(TargetPoint, Player.Rotation); });
                    }
                }
                await Task.Delay(ClipUpdateRate);
            }
        }

        private bool VectorInWorld(Vector3 Vector)
        {
            return Landscape.getWorldHeight(Vector, out _);
        }
    }
}