using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Components;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShimmysAdminTools.Commands
{
    public class GotoBedCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "gotobed";

        public string Help => "Teleports you to another player's bed.";

        public string Syntax => "gotobed [Player/Player ID]";

        public List<string> Aliases => new List<string>() { "tpbed" };

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.GotoBed" };

        public void Execute(IRocketPlayer caller, string[] command)
        {

            if (command.Length >= 1)
            {
                CSteamID PlayerID;
                UnturnedPlayer TargetPlayer = UnturnedPlayer.FromName(command[0]);
                if (TargetPlayer == null)
                {
                    if (Helpers.IsSteamID(command[0]))
                    {
                        PlayerID = new CSteamID(Convert.ToUInt64(command[0]));
                    } else
                    {
                        UnturnedChat.Say(caller, "GotoBed_NoPlayer".Translate());
                        return;
                    }
                } else
                {
                    PlayerID = TargetPlayer.CSteamID;
                }
                UnturnedPlayer Player = (UnturnedPlayer)caller;
                if (BarricadeManager.tryGetBed(PlayerID, out Vector3 Location, out byte Angle))
                {
                    Player.Player.teleportToLocationUnsafe(Location, Angle);
                } else
                {
                    UnturnedChat.Say(caller, $"GotoBed_NoBed".Translate(Player.CSteamID.m_SteamID));
                }
            } else
            {
                UnturnedChat.Say(caller, Syntax);
            }
        }
    }
}
