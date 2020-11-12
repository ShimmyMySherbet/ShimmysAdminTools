using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace ShimmysAdminTools.Commands
{
    public class HideBuldablesCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "HideBuildables";

        public string Help => "Hides all buildables, increasing FPS.";

        public string Syntax => "HideBuildables";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.HideBuildables" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer UPlayer = (UnturnedPlayer)caller;
            int Purged = 0;
            foreach (BarricadeRegion BRegion in BarricadeManager.regions)
            {
                foreach (BarricadeData BData in BRegion.barricades)
                {
                    //if (BData.owner == UPlayer.CSteamID.m_SteamID) continue;
                    if (Regions.tryGetCoordinate(BData.point, out byte x, out byte y))
                    {
                        ushort index = (ushort)BRegion.barricades.IndexOf(BData);
                        BarricadeManager.instance.channel.send("tellTakeBarricade", UPlayer.CSteamID, ESteamPacket.UPDATE_RELIABLE_BUFFER, x, y, ushort.MaxValue, index);
                        Purged += 1;
                    }
                }
            }
        }

    }
}
