using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShimmysAdminTools.Modules
{
    public static class RaycastUtility
    {
        public static RaycastResult RayCastPlayer(UnturnedPlayer Player, int Raycasts = RayMasks.BARRICADE | RayMasks.VEHICLE | RayMasks.STRUCTURE, int MaxDistance = 100)
        {
            if (Physics.Raycast(Player.Player.look.aim.position, Player.Player.look.aim.forward, out RaycastHit ray, MaxDistance, Raycasts))
            {
                return new RaycastResult(ray, true);
            }
            else
            {
				return new RaycastResult(ray, false);
            }
        }

        public static RaycastResult RayCastPlayerAll(UnturnedPlayer Player, int MaxDistance = 100)
        {
            if (Physics.Raycast(Player.Player.look.aim.position, Player.Player.look.aim.forward, out RaycastHit ray, MaxDistance))
            {
                return new RaycastResult(ray, true);
            }
            else
            {
                return new RaycastResult(ray, false);
            }
        }



        public static RaycastResult Raycast(Vector3 Origin, Vector3 Direction, int Raycasts = RayMasks.BARRICADE | RayMasks.VEHICLE | RayMasks.STRUCTURE, int MaxDistance = 100)
        {
            if (Physics.Raycast(Origin, Direction, out RaycastHit ray, MaxDistance, Raycasts))
            {
                return new RaycastResult(ray, true);
            }
            else
            {
                return new RaycastResult(ray, false);
            }
        }
        public static RaycastResult RaycastAll(Vector3 Origin, Vector3 Direction, int MaxDistance = 100)
        {
            if (Physics.Raycast(Origin, Direction, out RaycastHit ray, MaxDistance))
            {
                return new RaycastResult(ray, true);
            }
            else
            {
                return new RaycastResult(ray, false);
            }
        }


    }
}
