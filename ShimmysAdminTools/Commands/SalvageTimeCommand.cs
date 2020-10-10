using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;

namespace ShimmysAdminTools.Commands
{
    public class SalvageTimeCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "SalvageTime";

        public string Help => "Changes your salvage speed";

        public string Syntax => "SalvageTime";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.SalvageTime" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length >= 1 && float.TryParse(command[0], out float speed))
            {
                UnturnedPlayer pl = (UnturnedPlayer)caller;
                pl.Player.interact.sendSalvageTimeOverride(speed);
                UnturnedChat.Say(caller, $"Changed salvage speed to {speed}.");
            } else
            {
                UnturnedChat.Say(caller, Syntax);
            }
        }
    }
}
