using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using ShimmysAdminTools.Behaviors;
using ShimmysAdminTools.Modules;

namespace ShimmysAdminTools.Commands
{
    public class FireworkCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "Firework";
        public string Help => "Launches a player upwards and detonates them";
        public string Syntax => "Firework [Player] {Fuse Time} {Trail Rate}";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.Firework" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                UnturnedChat.Say(caller, Syntax + "\n" + Help);
                return;
            }

            var targetPlayer = PlayerUtil.GetPlayer(command[0]);

            if (targetPlayer == null)
            {
                UnturnedChat.Say(caller, "Failed to find player");
                return;
            }

            var fireworkObject = targetPlayer.Player.gameObject.GetOrAddComponent<FireworkBehaviour>();

            if (fireworkObject.IsLaunched)
            {
                fireworkObject.Abort();
                UnturnedChat.Say(caller, "Aborted Fireworks.");
                return;
            }

            if (command.Length >= 2)
            {
                if (float.TryParse(command[1], out var fuse) && fuse > 0 && fuse < 100)
                {
                    fireworkObject.Fuse = fuse;
                }
                else
                {
                    UnturnedChat.Say(caller, "Invalid fuse time");
                    return;
                }
            }

            if (command.Length >= 3)
            {
                if (float.TryParse(command[2], out var rate) && rate >= 0)
                {
                    fireworkObject.TrailRate = rate;
                }
                else
                {
                    UnturnedChat.Say(caller, "Invalid trail rate");
                    return;
                }
            }

            fireworkObject.Launch();
            UnturnedChat.Say(caller, "Fireworks launched.");
        }
    }
}