using Rocket.Core.Utils;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            new Thread(NoClipTick).Start();
        }

        public void Stop()
        {
            Active = false;
        }


        private void NoClipTick()
        {
            while(Active)
            {
                RaycastResult ClipCast = RaycastUtility.RayCastPlayer(Player, RayMasks.AGENT | RayMasks.BARRICADE | RayMasks.STRUCTURE | RayMasks.SMALL | RayMasks.MEDIUM | RayMasks.LARGE | RayMasks.ENVIRONMENT, 10) ;

                if (ClipCast.RaycastHit && ClipCast.Raycast.distance <= 2)
                {
                    Vector3 TargetPoint = new Vector3(ClipCast.Raycast.point.x, ClipCast.Raycast.point.y - Player.Player.look.heightLook, ClipCast.Raycast.point.z);


                    if (VectorInWorld(TargetPoint))
                    {
                        TaskDispatcher.QueueOnMainThread(delegate { Player.Teleport(TargetPoint, Player.Rotation); });
                    }
                }
                Thread.Sleep(ClipUpdateRate);
            }
        }



        private bool VectorInWorld(Vector3 Vector)
        {
            RaycastResult DownCast = RaycastUtility.Raycast(new Vector3(Vector.x, 900, Vector.z), Vector3.down, RayMasks.GROUND , 1500);
            if (DownCast.RaycastHit)
            {
                return Vector.y > (DownCast.Raycast.point.y + 0.1);
            } else
            {
                return false;
            }
        }
    }
}
