using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Modules;

namespace ShimmysAdminTools.Behaviors
{
    public class BoomCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "Boom";
        public string Help => "Creates an explosion where you're looking";
        public string Syntax => Name;
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.Boom" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var player = caller as UnturnedPlayer;

            var eyeLookAdjusted = player.Player.look.aim.position + player.Player.look.aim.forward;

            var cast = RaycastUtility.Raycast(eyeLookAdjusted, player.Player.look.aim.forward,
                RayMasks.BARRICADE | RayMasks.STRUCTURE | RayMasks.GROUND | RayMasks.GROUND2 | RayMasks.AGENT, RayMasks.PLAYER | RayMasks.RESOURCE |
                RayMasks.SMALL | RayMasks.MEDIUM | RayMasks.LARGE);

            if (!cast.RaycastHit)
            {
                return;
            }

            var dmg = 200;

            EffectManager.sendEffect(20, EffectManager.INSANE, cast.Raycast.point);
            DamageTool.explode(cast.Raycast.point, 10f, EDeathCause.GRENADE, player.CSteamID, dmg, dmg, dmg, dmg, dmg, dmg, dmg, dmg, out _);
        }
    }
}
