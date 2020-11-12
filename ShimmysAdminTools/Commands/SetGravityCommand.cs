using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Models;
using ShimmysAdminTools.Modules;

namespace ShimmysAdminTools.Commands
{
    public class SetGravityCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "SetGravity";

        public string Help => "Changes your gravity multiplier.";

        public string Syntax => "SetGravity [Value], Default value is 1.";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.SetGravity.Self" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer UPlayer = (UnturnedPlayer)caller;

            PlayerSession session = PlayerSessionStore.GetPlayerData(UPlayer);

            if (command.Length >= 1)
            {
                if (session.FlySessionActive)
                {
                    UnturnedChat.Say(caller, "SetGravity_Self_Fail_IsFlying".Translate());
                }
                else
                {
                    if (float.TryParse(command[0], out float Val))
                    {
                        UPlayer.Player.movement.sendPluginGravityMultiplier(Val);

                        UnturnedChat.Say(caller, "SetGravity_Self_Pass_GravityChanged".Translate(Val));
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "SetGravity_Self_Fail_InvalidParameter".Translate());
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