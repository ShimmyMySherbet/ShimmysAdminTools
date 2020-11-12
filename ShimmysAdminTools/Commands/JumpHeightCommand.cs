using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Components;

namespace ShimmysAdminTools.Commands
{
    public class JumpHeightCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "SetJump";

        public string Help => "Changes your jump multiplier.";

        public string Syntax => "SetJump [Value], Default value is 1.";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.SetJump.Self" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer UPlayer = (UnturnedPlayer)caller;

            if (command.Length >= 1)
            {
                if (float.TryParse(command[0], out float Val))
                {
                    UPlayer.Player.movement.sendPluginJumpMultiplier(Val);

                    UnturnedChat.Say(caller, "SetJump_Self_Pass_JumpChanged".Translate(Val));
                }
                else
                {
                    UnturnedChat.Say(caller, "SetJump_Self_Fail_InvalidParameter".Translate());
                }
            }
            else
            {
                UnturnedChat.Say(caller, Syntax);
            }
        }
    }
}