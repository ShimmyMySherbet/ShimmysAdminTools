using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Components;

namespace ShimmysAdminTools.Commands
{
    public class SetFiremodeCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "SetFiremode";

        public string Help => "Force changes the firemode of your weapon.";

        public string Syntax => "SetFiremode [Mode]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.SetFireMode" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (AdminToolsPlugin.Config.DisableAbusableCommands)
            {
                UnturnedChat.Say(caller, "Fail_Command_Disabled".Translate());
                return;
            }
            UnturnedPlayer Player = (UnturnedPlayer)caller;

            if (command.Length > 0)
            {
                string StrMode = command[0].ToUpper(); ;

                if (Player.Player.equipment.state == null || Player.Player.equipment.state.Length < 10)
                {
                    UnturnedChat.Say(caller, "Firemode_Unsupported".Translate());
                    return;
                }

                if (Enum.TryParse(StrMode, out EFiremode result))
                {
                    Player.Player.equipment.state[11] = (byte)result;
                    Player.Player.equipment.sendUpdateState();
                    Player.TriggerEffect(8);
                    UnturnedChat.Say(caller, "Firemode_Changed".Translate(Enum.GetName(typeof(EFiremode), result)));
                }
                else
                {
                    UnturnedChat.Say(caller, $"Firemode_Modes".Translate());
                }

            }
            else
            {
                UnturnedChat.Say(caller, Syntax);
            }
        }
    }
}