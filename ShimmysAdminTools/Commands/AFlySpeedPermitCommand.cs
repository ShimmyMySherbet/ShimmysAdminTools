using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Models;
using ShimmysAdminTools.Modules;

namespace ShimmysAdminTools.Commands
{
    public class AFlySpeedPermitCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "AFlySpeedPermit";

        public string Help => "Overrides a player's max fly speed.";

        public string Syntax => "AFlySpeedPermit [Player] [Speed]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.Grant.AflySpeed" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                UnturnedChat.Say(caller, Syntax);
            }
            else
            {
                UnturnedPlayer Player = UnturnedPlayer.FromName(command[0]);
                if (Player == null)
                {
                    UnturnedChat.Say(caller, "Error_PlayerNotFound".Translate());
                    return;
                }
                int NewOverride = 0;
                if (command.Length > 1)
                {
                    bool failed = false;
                    try
                    {
                        NewOverride = Convert.ToInt32(command[1]);
                    }
                    catch (FormatException)
                    {
                        failed = true;
                    }
                    catch (OverflowException)
                    {
                        failed = true;
                    }
                    if (failed)
                    {
                        UnturnedChat.Say(caller, "Flight_PermitSpeed_InvalidNumber".Translate());
                        return;
                    }
                }
                PlayerData data = PlayerDataStore.GetPlayerData(Player);
                if (data == null) return;
                data.FlightSpeedPermitOverride = NewOverride;
                if (NewOverride == 0)
                {
                    UnturnedChat.Say(caller, "Flight_PermitSpeed_Reset".Translate());
                }
                else
                {
                    UnturnedChat.Say(caller, "Flight_PermitSpeed_Updated".Translate());
                }
            }
        }
    }
}