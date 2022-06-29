using System.Collections.Generic;
using Rocket.API;
using SDG.Unturned;

namespace ShimmysAdminTools.Commands
{
    public class KickAllCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "KickAll";
        public string Help => "Kicks all players on the server.";
        public string Syntax => "KickAll [reason]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.Kickall" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var reason = string.Join(", ", command);
            for (int i = 0; i < Provider.clients.Count; i++)
            {
                Provider.kick(Provider.clients[i].playerID.steamID, reason);
            }
        }
    }
}