using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;

namespace ShimmysAdminTools.Commands
{
    public class ForceSaveCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "ForceSave";

        public string Help => "Forces a server save.";

        public string Syntax => "ForceSave";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.ForceSave" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            SaveManager.save();
            UnturnedChat.Say(caller, "Server Saved.");
        }
    }
}
