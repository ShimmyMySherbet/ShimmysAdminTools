using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Models;
using ShimmysAdminTools.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimmysAdminTools.Commands
{
    public class CheckBedOwner : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "CheckBedOwner";

        public string Help => "Gets the owner of a bed";

        public string Syntax => Name;

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.CheckBedOwner" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            RaycastResult Raycast = RaycastUtility.RayCastPlayer((UnturnedPlayer)caller, RayMasks.BARRICADE | RayMasks.STRUCTURE);
            if (Raycast.RaycastHit)
            {
                if (Raycast.ParentHasComponent<InteractableBed>())
                {
                    InteractableBed Bed = Raycast.TryGetEntity<InteractableBed>();
                    if (Bed.owner.m_SteamID == 0)
                    {
                        UnturnedChat.Say(caller, "CheckBedOwner_NotClaimed".Translate());
                    } else
                    {
                        UnturnedChat.Say(caller, "CheckBedOwner_ShowOwner".Translate($"{main.Instance.GetPlayerName(Bed.owner.m_SteamID, "UnknownPlayer")} ({Bed.owner.m_SteamID})"));

                    }
                }
            } else
            {
                UnturnedChat.Say(caller, "CheckBedOwner_NoBedFound".Translate());
            }
        }
    }
}
