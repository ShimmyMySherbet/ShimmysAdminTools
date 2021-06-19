using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Components;

namespace ShimmysAdminTools.Commands
{
    public class SetServerGravity : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "ServerGravity";

        public string Help => "Changes the server gravity multiplier.";

        public string Syntax => "ServerGravity [Value], Default value is 1.";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.SetGravity.Global" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length >= 1)
            {
                if (float.TryParse(command[0], out float Val))
                {
                    AdminToolsPlugin.Instance.ServerGravityMult = Val;
                    AdminToolsPlugin.Instance.ForEachNonFlyingPlayer(x => x.movement.sendPluginGravityMultiplier(Val));
                    UnturnedChat.Say(caller, "SetGravity_Global_Pass_Changed".Translate(Val));
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