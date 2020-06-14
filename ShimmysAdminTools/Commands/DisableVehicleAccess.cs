using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShimmysAdminTools.Commands
{
    public class DisableVehicleAccess : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "DisableVehicleAccess";

        public string Help => "Toggles a player's ability to enter and use vehicles.";

        public string Syntax => "DisabeVehicleAccess <Player>";

        public List<string> Aliases => new List<string>() { "EnableVehicleAccess", "DVA", "EVA" };

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.DisableVehicleAccess" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                UnturnedChat.Say(caller, Syntax);
            } else
            {
                UnturnedPlayer Player = UnturnedPlayer.FromName(command[0]);
                if (Player == null)
                {
                    UnturnedChat.Say(caller, "DisableVehicleAccess_NoPlayer".Translate());
                } else
                {
                    var Store = PlayerDataStore.GetPlayerData(Player);
                    Store.CanEnterVehicle = !Store.CanEnterVehicle;
                    if (Store.CanEnterVehicle)
                    {
                        UnturnedChat.Say(caller, "DisableVehicleAccess_AccessEnabled".Translate());
                    } else
                    {
                        if (Player.IsInVehicle)
                        {
                            Player.CurrentVehicle.forceRemovePlayer(out byte seat, Player.CSteamID, out Vector3 pos, out byte angle);
                            SDG.Unturned.VehicleManager.sendExitVehicle(Player.CurrentVehicle, seat, pos, angle, true);
                        }
                        UnturnedChat.Say(caller, "DisableVehicleAccess_AccessDisabled".Translate());
                    }
                }
            }
        }
    }
}
