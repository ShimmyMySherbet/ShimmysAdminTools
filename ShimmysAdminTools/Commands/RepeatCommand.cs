using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Core;
using Rocket.Core.Commands;
using Rocket.Unturned.Chat;
using ShimmysAdminTools.Behaviors;
using Action = System.Action;

namespace ShimmysAdminTools.Commands
{
    public class RepeatCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "Repeat";

        public string Help => "Repeats a command";

        public string Syntax => "Repeat [Times] [Command]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.Repeat" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length <= 1 || !int.TryParse(command[0], out int Times))
            {
                UnturnedChat.Say(caller, Syntax);
                return;
            }
            string commandStr = string.Join(" ", command.ToList().GetRange(1, command.Length - 1));
            Action exec = () =>
            {
                for (int i = 0; i < Times; i++)
                {
                    R.Commands.Execute(caller, commandStr);
                }
            };
            RepeatCommandQueue.EnqueueAction(exec);
        }
    }
}