using Rocket.API;
using Rocket.Unturned.Chat;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Models;
using System;
using System.Collections.Generic;

namespace ShimmysAdminTools.Commands
{
    public class AFlyVSpeedCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "AFlyVSpeed";

        public string Help => "Changes/Resets your vertical fly speed.";

        public string Syntax => "AFlyVSpeed [Speed]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.Flight" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            PlayerSession Session = caller.GetSession();
            if (Session.FlySessionActive)
            {
                float Speed = 1;
                if (command.Length >= 1)
                {
                    Speed = (float)Convert.ToDouble(command[0]);
                }
                if (Helpers.PlayerCanFlyAtVSpeed(caller.UPlayer(), Speed))
                {
                    Session.FlySession.VerticalSpeed = Speed;
                    if (Speed == 1)
                    {
                        UnturnedChat.Say(caller, "Flight_Speed_Vertical_Reset".Translate());
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "Flight_Speed_Vertical_Changed".Translate(Speed));
                    }
                } else
                {
                    UnturnedChat.Say(caller, "Flight_Speed_Denied".Translate());
                }
            } else
            {
                UnturnedChat.Say(caller, "Flight_Speed_NotFlying".Translate());
            }
        }
    }

    public class AFlySpeedCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "AFlySpeed";

        public string Help => "Changes/Resets your fly speed.";

        public string Syntax => "AFlySpeed [Speed]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.Flight" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            PlayerSession Session = caller.GetSession();
            if (Session.FlySessionActive)
            {
                float Speed = 1;
                if (command.Length >= 1)
                {
                    Speed = (float)Convert.ToDouble(command[0]);
                }
                if (Speed == 1 || Helpers.PlayerCanFlyAtSpeed(caller.UPlayer(), Speed))
                {
                    Session.FlySession.Speed = Speed;
                    Session.FlySession.SendUpdateSpeed();
                    if (Speed == 1)
                    {
                        UnturnedChat.Say(caller, "Flight_Speed_Reset".Translate());
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "Flight_Speed_Changed".Translate(Speed));
                    }
                }
                else
                {
                    UnturnedChat.Say(caller, "Flight_Speed_Denied".Translate());
                }
            }
            else
            {
                UnturnedChat.Say(caller, "Flight_Speed_NotFlying".Translate());
            }
        }
    }
}