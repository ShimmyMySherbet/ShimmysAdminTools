using System.Collections.Generic;
using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Behaviors;

namespace ShimmysAdminTools.Commands
{
    public class StructureESPCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "StructureESP";

        public string Help => "Toggles Structure ESP";

        public string Syntax => "StructureESP <Setting> <Value>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.StructureESP" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer Player = (UnturnedPlayer)caller;

            StructureESPBehaviour StructESP = Player.Player.gameObject.GetComponent<StructureESPBehaviour>();

            if (command.Length >= 2 && StructESP != null)
            {
                string b = command[0];
                string z = command[1];
                if (b.ToLower() == "effect")
                {
                    StructESP.Effect = ushort.Parse(z);
                }
                else if (b.ToLower() == "range")
                {
                    StructESP.Range = int.Parse(z);
                }
                else if (b.ToLower() == "rate")
                {
                    StructESP.Rate = int.Parse(z);
                } else
                {
                    UnturnedChat.Say(caller, "Settings: Effect, Range, Rate");
                }
                return;
            }
            if (StructESP == null)
            {
                Player.Player.gameObject.AddComponent<StructureESPBehaviour>();
                UnturnedChat.Say(caller, "Structure ESP enabled");
            }
            else
            {
                Player.Player.gameObject.TryRemoveComponent<StructureESPBehaviour>();
                UnturnedChat.Say(caller, "Structure ESP disabled");
            }
        }
    }
}