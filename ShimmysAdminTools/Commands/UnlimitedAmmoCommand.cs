using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Behaviors;
using ShimmysAdminTools.Components;
using UnityEngine;

namespace ShimmysAdminTools.Commands
{
    public class UnlimitedAmmoCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "UnlimitedAmmo";

        public string Help => "Toggles unlimited ammo.";

        public string Syntax => "UnlimitedAmmo <Bypass Max: true/false>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.UnlimitedAmmo" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (AdminToolsPlugin.Config.DisableAbusableCommands)
            {
                UnturnedChat.Say(caller, "Fail_Command_Disabled".Translate());
                return;
            }
            UnturnedPlayer Player = (UnturnedPlayer)caller;
            UnlimitedAmmoBehaviour Existing = Player.Player.gameObject.GetComponent<UnlimitedAmmoBehaviour>();
            if (Existing == null)
            {
                bool Override = false;
                byte Amount = 255;
                if (command.Length > 0)
                {
                    if (byte.TryParse(command[0], out byte Amt))
                    {
                        Override = true;
                        Amount = Amt;
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "UnlimitedAmmo_Fail_Override".Translate());
                        return;
                    }
                }
                UnlimitedAmmoBehaviour AB = Player.Player.gameObject.getOrAddComponent<UnlimitedAmmoBehaviour>();
                if (Override)
                {
                    AB.AmountOverride = Amount;
                    AB.AmountOverrideEnabled = Override;
                    UnturnedChat.Say(caller, "UnlimitedAmmo_Enabled_OverrideAmount".Translate(Amount));
                }
                else
                {
                    UnturnedChat.Say(caller, "UnlimitedAmmo_Enabled".Translate());
                }
            }
            else
            {
                Object.Destroy(Existing);
                UnturnedChat.Say(caller, "UnlimitedAmmo_Disabled".Translate());
            }
        }
    }
}