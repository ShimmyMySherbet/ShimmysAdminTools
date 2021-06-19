using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using ShimmysAdminTools.Components;

namespace ShimmysAdminTools.Commands
{
    public class SetServerJump : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "ServerJump";

        public string Help => "Changes the server jump multiplier.";

        public string Syntax => "ServerJump [Value], Default value is 1.";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.SetJump.Global" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length >= 1)
            {
                if (float.TryParse(command[0], out float Val))
                {
                    AdminToolsPlugin.Instance.ServerJumpMult = Val;
                    AdminToolsPlugin.Instance.ForEachNonFlyingPlayer(x => x.movement.sendPluginJumpMultiplier(Val));
                    UnturnedChat.Say(caller, "SetJump_Global_Pass_Changed".Translate(Val));
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