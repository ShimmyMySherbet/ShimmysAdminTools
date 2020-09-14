using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace ShimmysAdminTools.Commands
{
    public class WipePlayerBuildingsCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "WipePlayerBuildings";

        public string Help => "Wipes the specified player's buildables.";

        public string Syntax => "WipePlayerBuildings [Player] <ID>";

        public List<string> Aliases => new List<string>() { "WPB" };

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.WipePlayerBuildables" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length >= 1)
            {
                ulong PlayerID;
                if (ulong.TryParse(command[0], out ulong ResID))
                {
                    PlayerID = ResID;
                }
                else
                {
                    UnturnedPlayer uPlayer = UnturnedPlayer.FromName(command[0]);
                    if (uPlayer == null)
                    {
                        UnturnedChat.Say(caller, "Failed to find player");
                        return;
                    }
                    else
                    {
                        PlayerID = uPlayer.CSteamID.m_SteamID;
                    }
                }
                ushort BID = 0;
                if (command.Length >= 2 && ushort.TryParse(command[1], out ushort ResBID))
                {
                    BID = ResBID;
                }

                List<KeyValuePair<BarricadeRegion, BarricadeData>> WipeList = new List<KeyValuePair<BarricadeRegion, BarricadeData>>();
                foreach (BarricadeRegion BRegion in BarricadeManager.regions)
                {
                    foreach (BarricadeData B in BRegion.barricades)
                    {
                        if (B.owner == PlayerID && ((BID == 0) || (B.barricade.id == BID)))
                        {
                            WipeList.Add(new KeyValuePair<BarricadeRegion, BarricadeData>(BRegion, B));
                        }
                    }
                }
                UnturnedChat.Say(caller, $"Found {WipeList.Count} buildables; wiping...");
                foreach (var ent in WipeList)
                {
                    if (Regions.tryGetCoordinate(ent.Value.point, out byte x, out byte y))
                    {
                        ushort plant = ushort.MaxValue;
                        ushort index = (ushort)ent.Key.barricades.IndexOf(ent.Value);
                        BarricadeManager.destroyBarricade(ent.Key, x, y, plant, index);
                    }
                }
                UnturnedChat.Say(caller, $"Destroyed {WipeList.Count} Buildables.");
            } else
            {
                UnturnedChat.Say(caller, Syntax);
            }
        }
    }
}