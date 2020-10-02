using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Models;
using UnityEngine;

namespace ShimmysAdminTools.Commands
{
    public class FindBaseCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "FindBase";

        public string Help => "Searches for a player's base/buildables.";

        public string Syntax => "FindBase [Player]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.FindBase" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            bool Teleport = false;
            int TeleportIndex = 0;
            if (command.Length >= 1)
            {
                if (command.Length >= 2)
                {
                    if (int.TryParse(command[1], out int ind))
                    {
                        TeleportIndex = ind - 1;
                        if (TeleportIndex < 0)
                        {
                            UnturnedChat.Say(caller, "Base Number must be greater than 0");
                            return;
                        }
                        else
                        {
                            Teleport = true;
                        }
                    }
                }

                string PlayerStr = command[0];
                ulong PlayerID;
                ulong GroupID = 0;
                if (ulong.TryParse(PlayerStr, out ulong Res))
                {
                    PlayerID = Res;
                    GroupInfo Info = GroupManager.getGroupInfo(new Steamworks.CSteamID(Res));
                    if (Info != null)
                    {
                        GroupID = Info.groupID.m_SteamID;
                    }
                }
                else
                {
                    UnturnedPlayer UPlayer = UnturnedPlayer.FromName(PlayerStr);
                    if (UPlayer == null)
                    {
                        UnturnedChat.Say(caller, "Failed to find player.");
                        return;
                    }
                    else
                    {
                        PlayerID = UPlayer.CSteamID.m_SteamID;
                        GroupID = UPlayer.Player.quests.groupID.m_SteamID;
                    }
                }
                List<StructureCluster> Clusters = new List<StructureCluster>();

                foreach (var structureRegion in StructureManager.regions)
                {
                    foreach (StructureData structure in structureRegion.structures)
                    {
                        if (structure.owner != PlayerID && (structure.group != GroupID || GroupID == 0))
                        {
                            continue;
                        }
                        bool NewCluster = true;
                        Vector3 Pos = structure.point;
                        StructureCluster ExistingCluster = null;
                        foreach (StructureCluster cluster in Clusters)
                        {
                            foreach (Vector3 ClusterObject in cluster.Objects)
                            {
                                if (Vector3.Distance(ClusterObject, Pos) < 10f)
                                {
                                    NewCluster = false;
                                    ExistingCluster = cluster;
                                    break;
                                }
                                if (ExistingCluster != null) break;
                            }
                            if (ExistingCluster != null) break;
                        }
                        if (NewCluster)
                        {
                            StructureCluster NCluster = new StructureCluster();
                            NCluster.Objects.Add(Pos);
                            NCluster.OriginPosition = Pos;
                            Clusters.Add(NCluster);
                        }
                        else
                        {
                            ExistingCluster.Objects.Add(Pos);
                        }
                    }
                }

                foreach (var barricadeRegion in BarricadeManager.regions)
                {
                    foreach (BarricadeData barricade in barricadeRegion.barricades)
                    {
                        if (barricade.owner != PlayerID && (barricade.group != GroupID || GroupID == 0))
                        {
                            continue;
                        }

                        if (barricade.barricade.asset is ItemTrapAsset)
                        {
                            continue;
                        }

                        bool NewCluster = true;
                        Vector3 Pos = barricade.point;
                        StructureCluster ExistingCluster = null;
                        int StorageSize = 0;
                        if (barricade.barricade.asset is ItemStorageAsset StorageAsset)
                        {
                            StorageSize = StorageAsset.storage_x * StorageAsset.storage_y;
                        }

                        foreach (StructureCluster cluster in Clusters)
                        {
                            foreach (Vector3 ClusterObject in cluster.Objects)
                            {
                                if (Vector3.Distance(ClusterObject, Pos) < 10f)
                                {
                                    NewCluster = false;
                                    ExistingCluster = cluster;
                                    break;
                                }
                                if (ExistingCluster != null) break;
                            }
                            if (ExistingCluster != null) break;
                        }
                        if (NewCluster)
                        {
                            StructureCluster NCluster = new StructureCluster();
                            NCluster.Objects.Add(Pos);
                            NCluster.OriginPosition = Pos;
                            NCluster.StorageSlots += StorageSize;
                            Clusters.Add(NCluster);
                        }
                        else
                        {
                            ExistingCluster.Objects.Add(Pos);
                            ExistingCluster.StorageSlots += StorageSize;
                        }
                    }
                }
                Clusters = Clusters.OrderByDescending(x => x.Objects.Count).ToList();

                if (Teleport)
                {
                    if (Clusters.Count > TeleportIndex)
                    {
                        StructureCluster SelectedCluster = Clusters[TeleportIndex];
                        UnturnedPlayer UPlayer = (UnturnedPlayer)caller;

                        Vector3 Max = SelectedCluster.OriginPosition;
                        foreach (Vector3 Obj in SelectedCluster.Objects)
                        {
                            if (Max.y < Obj.y) Max = Obj;
                        }
                        UPlayer.Player.teleportToLocationUnsafe(Max + new Vector3(0, 10, 0), UPlayer.Rotation);
                        UnturnedChat.Say(caller, $"Teleported to cluster {TeleportIndex + 1}.");
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "No base with that index.");
                    }
                    return;
                }

                UnturnedChat.Say(caller, $"Found {Clusters.Count} clusters.");

                int SelectAmt = 5;
                if (SelectAmt > Clusters.Count) SelectAmt = Clusters.Count;
                foreach (StructureCluster Cluster in Clusters.GetRange(0, SelectAmt))
                {
                    UnturnedChat.Say(caller, $"[Cluster {Clusters.IndexOf(Cluster) + 1}] Objects: {Cluster.Objects.Count}, Storage Slots: {Cluster.StorageSlots}");
                }
            }
        }
    }
}