using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Components;

namespace ShimmysAdminTools.Commands
{
    public class SetServerSpeed : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "ServerSpeed";

        public string Help => "Changes the server speed multiplier.";

        public string Syntax => "ServerSpeed [Value], Default value is 1.";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.SetSpeed.Global" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length >= 1)
            {
                if (float.TryParse(command[0], out float Val))
                {
                    AdminToolsPlugin.Instance.ServerSpeedMult = Val;
                    AdminToolsPlugin.Instance.ForEachNonFlyingPlayer(x => x.movement.sendPluginSpeedMultiplier(Val));
                    UnturnedChat.Say(caller, "SetSpeed_Global_Pass_Changed".Translate(Val));
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
