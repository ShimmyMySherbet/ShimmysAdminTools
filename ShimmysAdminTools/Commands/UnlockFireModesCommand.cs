using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Behaviors;
using ShimmysAdminTools.Components;
using UnityEngine;

namespace ShimmysAdminTools.Commands
{
    public class UnlockFireModesCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "UnlockFireModes";

        public string Help => "Unlocks all firemodes for your weapons";

        public string Syntax => Name;

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.UnlockFiremodes" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (AdminToolsPlugin.Config.DisableAbusableCommands)
            {
                UnturnedChat.Say(caller, "Fail_Command_Disabled".Translate());
                return;
            }
            UnturnedPlayer Player = (UnturnedPlayer)caller;
            FiremodeUnlocker Existing = Player.Player.gameObject.GetComponent<FiremodeUnlocker>();
            if (Existing == null)
            {
                Player.Player.gameObject.getOrAddComponent<FiremodeUnlocker>();
                UnturnedChat.Say(caller, "Firemode_Unlocker_Enabled".Translate());
            }
            else
            {
                Object.Destroy(Existing);
                UnturnedChat.Say(caller, "Firemode_Unlocker_Disabled".Translate());
            }
        }
    }
}