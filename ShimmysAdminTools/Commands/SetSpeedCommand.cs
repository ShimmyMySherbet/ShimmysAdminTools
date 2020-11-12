using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Models;
using ShimmysAdminTools.Modules;

namespace ShimmysAdminTools.Commands
{
    public class SetSpeedCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "SetSpeed";

        public string Help => "Changes your speed multiplier.";

        public string Syntax => "SetSpeed [Value], Default value is 1.";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.SetSpeed.Self" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer UPlayer = (UnturnedPlayer)caller;
            PlayerSession session = PlayerSessionStore.GetPlayerData(UPlayer);

            if (command.Length >= 1)
            {
                if (session.FlySessionActive)
                {
                    UnturnedChat.Say(caller, "SetSpeed_Self_Fail_IsFlying".Translate());
                }
                else
                {
                    if (float.TryParse(command[0], out float Val))
                    {
                        UPlayer.Player.movement.sendPluginSpeedMultiplier(Val);

                        UnturnedChat.Say(caller, "SetSpeed_Self_Pass_SpeedChanged".Translate(Val));
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "SetSpeed_Self_Fail_InvalidParameter".Translate());
                    }
                }
            }
            else
            {
                UnturnedChat.Say(caller, Syntax);
            }
        }
    }
}
