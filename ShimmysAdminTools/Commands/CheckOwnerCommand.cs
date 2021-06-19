using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Modules;
using Steamworks;

namespace ShimmysAdminTools.Commands
{
    public class CheckOwnerCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "CheckOwner";

        public string Help => "Checks the owner of a barricade/vehicle";

        public string Syntax => "CheckOwner";

        public List<string> Aliases => new List<string>() { "ck" };

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.Checkowner" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var raycast = RaycastUtility.RayCastPlayer((UnturnedPlayer)caller, RayMasks.BARRICADE | RayMasks.STRUCTURE | RayMasks.VEHICLE, 500);

            if (raycast.RaycastHit)
            {
                ulong ID = 0;
                string name;

                if (raycast.Barricade != null)
                {
                    ID = raycast.Barricade.owner;
                }
                else if (raycast.Structure != null)
                {
                    ID = raycast.Structure.owner;
                }
                else if (raycast.Vehicle != null)
                {
                    ID = raycast.Vehicle.lockedOwner.m_SteamID;
                }

                name = AdminToolsPlugin.Instance.GetPlayerName(ID, "Unknown Player");

                InteractableBed b = raycast.TryGetEntity<InteractableBed>();

                if (b != null && b.owner != CSteamID.Nil)
                {
                    ulong bedID = b.owner.m_SteamID;
                    string bedName = AdminToolsPlugin.Instance.GetPlayerName(bedID, "Unknown Player");
                    UnturnedChat.Say(caller, "CheckOwner_Pass_Found_Bed".Translate(name, ID, bedName, bedID));
                }
                else
                {
                    UnturnedChat.Say(caller, "CheckOwner_Pass_Found".Translate(name, ID));
                }
                return;
            }
            UnturnedChat.Say(caller, "CheckOwner_Fail_NotFound".Translate());
        }
    }
}