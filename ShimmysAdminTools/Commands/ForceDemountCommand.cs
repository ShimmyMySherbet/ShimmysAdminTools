using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Components;
using System.Collections.Generic;
using UnityEngine;

namespace ShimmysAdminTools.Commands
{
    public class ForceDemountCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "ForceDismount";

        public string Help => "Forcibly removes a player from a vehicle";

        public string Syntax => "ForceDismount <Player>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.ForceDismount" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer TargetPlayer = (UnturnedPlayer)caller;
            if (command.Length >= 1) TargetPlayer = UnturnedPlayer.FromName(command[0]);
            if (TargetPlayer != null)
            {
                if (TargetPlayer.IsInVehicle)
                {
                    TargetPlayer.CurrentVehicle.forceRemovePlayer(out byte seat, TargetPlayer.CSteamID, out Vector3 pos, out byte angle);
                    VehicleManager.sendExitVehicle(TargetPlayer.CurrentVehicle, seat, pos, angle, true);
                    UnturnedChat.Say(caller, "ForceDismount_Dismounted".Translate());
                }
                else
                {
                    UnturnedChat.Say(caller, "ForceDismount_NoVehicle".Translate());
                }
            }
            else
            {
                UnturnedChat.Say(caller, "ForceDismount_NoPlayer".Translate());
            }
        }
    }
}