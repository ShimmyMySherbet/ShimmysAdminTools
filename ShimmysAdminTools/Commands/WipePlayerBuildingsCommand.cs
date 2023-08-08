using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Modules;
using Steamworks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
            if (command.Length == 0)
            {
                UnturnedChat.Say(caller, Syntax);
            }

            if (!PlayerSelector.GetPlayerID(command[0], out var PlayerID))
            {
                UnturnedChat.Say(caller, "Failed to find player");
                return;
            }


            ushort BID = 0;
            if (command.Length >= 2 && ushort.TryParse(command[1], out ushort ResBID))
            {
                BID = ResBID;
            }

            var wipeList = new List<IWipeable>();


            for (byte x = 0; x < BarricadeManager.regions.GetLength(0); x++)
            {
                for (byte y = 0; y < BarricadeManager.regions.GetLength(1); y++)
                {
                    var region = BarricadeManager.regions[x, y];

                    for (int i = 0; i < region.barricades.Count; i++)
                    {
                        var barricade = region.barricades[i];
                        if (barricade.owner == PlayerID && (BID == 0 || barricade.barricade.asset.id == BID))
                        {
                            var drop = region.drops[i];

                            wipeList.Add(new WipeBarricade(drop, x, y));
                        }
                    }
                }
            }


            for (byte x = 0; x < StructureManager.regions.GetLength(0); x++)
            {
                for (byte y = 0; y < StructureManager.regions.GetLength(1); y++)
                {
                    var region = StructureManager.regions[x, y];

                    for(int i = 0; i < region.structures.Count; i++)
                    {
                        var structure = region.structures[i];
                        if (structure.owner == PlayerID && (BID == 0 || structure.structure.asset.id == BID))
                        {
                            var drop = region.drops[i];

                            wipeList.Add(new WipeStructure(drop, x, y));
                        }
                    }
                }
            }

            UnturnedChat.Say(caller, $"Found {wipeList.Count} buildables; wiping...");
            AdminToolsPlugin.Instance.StartCoroutine(RunWipe(wipeList, 30, () =>
            {
                UnturnedChat.Say(caller, $"Finished wiping {wipeList.Count} buildables");
            }));
        }


        private IEnumerator RunWipe(IEnumerable<IWipeable> wipeables, int perTick, System.Action callback)
        {
            using(var wipes = wipeables.GetEnumerator())
            {
                for(int i = 0; i < perTick; i++)
                {
                    if (!wipes.MoveNext())
                    {
                        yield break;
                    }

                    try
                    {
                        wipes.Current.Delete();
                    }
                    catch (Exception ex)
                    {
                        Rocket.Core.Logging.Logger.LogError($"Error running wipe: {ex.Message}");
                    }
                }
                yield return new WaitForFixedUpdate();
            }
            callback();
        }



        private interface IWipeable
        {
            void Delete();
        }


        private class WipeBarricade : IWipeable
        {
            public BarricadeDrop Drop { get; }

            public byte X { get; }
            public byte Y { get; }

            public WipeBarricade(BarricadeDrop drop, byte x, byte y)
            {
                Drop = drop;
                X = x;
                Y = y;
            }

            public void Delete()
            {
                BarricadeManager.destroyBarricade(Drop, X, Y, ushort.MaxValue);
            }
        }


        private class WipeStructure : IWipeable
        {
            public StructureDrop Drop { get; }

            public byte X { get; }
            public byte Y { get; }

            public WipeStructure(StructureDrop drop, byte x, byte y)
            {
                Drop = drop;
                X = x;
                Y = y;
            }

            public void Delete()
            {
                StructureManager.destroyStructure(Drop, X, Y, Vector3.zero);
            }
        }
    }
}