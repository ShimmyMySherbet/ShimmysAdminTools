using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Models;
using UnityEngine;

namespace ShimmysAdminTools.Modules
{
    public static class PointToolManager
    {
        public static void ManageGestureUpdate(UnturnedPlayer player, UnturnedPlayerEvents.PlayerGesture gesture)
        {
            PlayerSession Session = PlayerSessionStore.GetPlayerData(player);
            if (gesture == UnturnedPlayerEvents.PlayerGesture.Salute)
            {
                if (Session.FlySessionActive)
                {
                    Session.FlySession.ResetSpeed();
                }
            }
            if (gesture == UnturnedPlayerEvents.PlayerGesture.Point || gesture == UnturnedPlayerEvents.PlayerGesture.PunchLeft || gesture == UnturnedPlayerEvents.PlayerGesture.PunchRight)
            {
                if (Session.PointToolEnabled)
                {
                    RunPointTool(player, Session, gesture);
                }
            }
        }

        public static void RunPointTool(UnturnedPlayer Player, PlayerSession Session, UnturnedPlayerEvents.PlayerGesture gesture)
        {
            if (Session.PointTool == PointToolMode.Destroy)
            {
                RaycastResult Raycast = RaycastUtility.RayCastPlayer(Player, RayMasks.BARRICADE | RayMasks.STRUCTURE | RayMasks.VEHICLE);
                if (Raycast.RaycastHit)
                {
                    RunDestroyTool(Player, Raycast);
                }
            }
            else if (Session.PointTool == PointToolMode.Utility)
            {
                RaycastResult Raycast = RaycastUtility.RayCastPlayer(Player, RayMasks.BARRICADE | RayMasks.STRUCTURE | RayMasks.VEHICLE);
                if (Raycast.RaycastHit)
                {
                    RunUtilityTool(Player, Raycast);
                }
            }
            else if (Session.PointTool == PointToolMode.Repair)
            {
                RaycastResult Raycast = RaycastUtility.RayCastPlayer(Player, RayMasks.BARRICADE | RayMasks.STRUCTURE | RayMasks.VEHICLE);
                if (Raycast.RaycastHit)
                {
                    RunRepairTool(Player, Raycast);
                }
            }
            else if (Session.PointTool == PointToolMode.Kill)
            {
                RaycastResult CloseEnemyCheck = RaycastUtility.RayCastPlayer(Player, RayMasks.AGENT | RayMasks.ENEMY, 7);
                RaycastResult ClosePlayerCheck = RaycastUtility.RayCastPlayer(Player, RayMasks.PLAYER, 10);
                if (ClosePlayerCheck.RaycastHit && ClosePlayerCheck.ParentHasComponent<Player>() && ClosePlayerCheck.TryGetEntity<Player>().channel.owner.playerID.steamID.m_SteamID != Player.CSteamID.m_SteamID)
                {
                    RunKillTool(Player, ClosePlayerCheck);
                }
                else if (CloseEnemyCheck.RaycastHit)
                {
                    RunKillTool(Player, CloseEnemyCheck);
                }
                else
                {
                    Vector3 RaycastSpot = Player.Player.look.aim.position + (Player.Player.look.aim.forward.normalized * 0.5f);
                    RaycastResult Raycast = RaycastUtility.Raycast(RaycastSpot, Player.Player.look.aim.forward, RayMasks.ENEMY | RayMasks.PLAYER | RayMasks.AGENT);
                    if (Raycast.RaycastHit)
                    {
                        RunKillTool(Player, Raycast);
                    }
                }
            }
            else if (Session.PointTool == PointToolMode.Jump)
            {
                RaycastResult Raycast = RaycastUtility.RayCastPlayerAll(Player, 5000);
                if (Raycast.RaycastHit)
                {
                    RunJumpTool(Player, Raycast, gesture);
                }
            }
        }

        public static void RunKillTool(UnturnedPlayer Player, RaycastResult Raycast)
        {
            if (Raycast.ParentHasComponent<Player>())
            {
                Player player = Raycast.TryGetEntity<Player>();
                if (player.channel.owner.playerID.steamID.m_SteamID == Player.CSteamID.m_SteamID) return;
                player.life.askDamage(100, player.look.aim.forward, EDeathCause.KILL, ELimb.SKULL, Player.CSteamID, out _, true, ERagdollEffect.GOLD, true, true);
            }
            else if (Raycast.ParentHasComponent<Zombie>())
            {
                Zombie zombie = Raycast.TryGetEntity<Zombie>();
                zombie.killWithFireExplosion();
            }
            else if (Raycast.ParentHasComponent<Animal>())
            {
                Raycast.TryGetEntity<Animal>().askDamage(ushort.MaxValue, Vector3.one, out _, out _, false, true, ERagdollEffect.NONE);
            }
        }

        public static void RunDestroyTool(UnturnedPlayer Player, RaycastResult Raycast)
        {
            if (Raycast.Vehicle != null)
            {
                bool IsPlayersVehicle = Raycast.Vehicle.lockedOwner == Player.CSteamID || Raycast.Vehicle.lockedGroup == Player.SteamGroupID;
                bool Allow = IsPlayersVehicle;
                if (!IsPlayersVehicle) Allow = PlayerCanDestroyOtherPlayersStuff(Player);
                if (Allow)
                {
                    VehicleManager.askVehicleDestroy(Raycast.Vehicle);
                }
                else
                {
                    UnturnedChat.Say(Player, "PointTool_Destroy_Denied".Translate());
                }
            }
            if (Raycast.Barricade != null)
            {
                bool IsPlayersBarricade = Raycast.Barricade.owner == Player.CSteamID.m_SteamID || Raycast.Barricade.group == Player.SteamGroupID.m_SteamID;
                bool Allow = IsPlayersBarricade;
                if (!IsPlayersBarricade) Allow = PlayerCanDestroyOtherPlayersStuff(Player);
                if (Allow)
                {
                    BarricadeManager.destroyBarricade(Raycast.BarricadeRegion, Raycast.BarricadeX, Raycast.BarricadeY, Raycast.BarricadePlant, Raycast.BarricadeIndex);
                }
                else
                {
                    UnturnedChat.Say(Player, "PointTool_Destroy_Denied".Translate());
                }
            }
            if (Raycast.Structure != null)
            {
                bool IsPlayersStructure = Raycast.Structure.owner == Player.CSteamID.m_SteamID || Raycast.Structure.group == Player.SteamGroupID.m_SteamID;
                bool Allow = IsPlayersStructure;
                if (!IsPlayersStructure) Allow = PlayerCanDestroyOtherPlayersStuff(Player);
                if (Allow)
                {
                    StructureManager.destroyStructure(Raycast.StructureRegion, Raycast.StructureX, Raycast.StructureY, Raycast.StructureIndex, new Vector3(0, 0, 0));
                }
                else
                {
                    UnturnedChat.Say(Player, "PointTool_Destroy_Denied".Translate());
                }
            }
        }

        public static bool PlayerCanDestroyOtherPlayersStuff(UnturnedPlayer Player)
        {
            return Player.HasPermission("ShimmysAdminTools.PointTool.DestroyOtherPlayersStuff") ||
                Player.HasPermission("ShimmysAdminTools.PointTool.all") ||
                Player.HasPermission("ShimmysAdminTools.PointTool.*");
        }

        public static void RunUtilityTool(UnturnedPlayer Player, RaycastResult Raycast)
        {
            if (Raycast.ParentHasComponent<InteractableCharge>())
            {
                Raycast.TryGetEntity<InteractableCharge>().detonate(Player.CSteamID);
            }

            if (Raycast.ParentHasComponent<InteractableVehicle>())
            {
                var f = Raycast.TryGetEntity<InteractableVehicle>();
                f.tellHorn();
            }
            if (Raycast.ParentHasComponent<InteractableBed>())
            {
                var f = Raycast.TryGetEntity<InteractableBed>();
                if (f.owner.m_SteamID != 0)
                {
                    UnturnedChat.Say(Player, "PointTool_Utility_Bed_ShowOwner".Translate(f.owner));
                }
                else
                {
                    UnturnedChat.Say(Player, "PointTool_Utility_Bed_NotClaimed".Translate()); ;
                }
            }
            if (Raycast.ParentHasComponent<InteractableDoor>())
            {
                var f = Raycast.TryGetEntity<InteractableDoor>();
                SendOpenDoor(Raycast.BarricadePlant, Raycast.BarricadeX, Raycast.BarricadeY, Raycast.BarricadeIndex, f, Raycast.BarricadeRegion);
            }
            if (Raycast.ParentHasComponent<InteractableStorage>())
            {
                InteractableStorage Storage = Raycast.TryGetEntity<InteractableStorage>();
                Player.Player.inventory.updateItems(7, Storage.items);
                Player.Player.inventory.sendStorage();
            }
        }

        public static void RunJumpTool(UnturnedPlayer Player, RaycastResult Raycast, UnturnedPlayerEvents.PlayerGesture gesture)
        {
            if (gesture == UnturnedPlayerEvents.PlayerGesture.PunchRight)
            {
                int RunMode = 0;
                float Pitch = Player.Player.look.pitch;

                if (Player.Stance == EPlayerStance.STAND)
                {
                    if (Pitch <= 5)
                    {
                        RunMode = 1;
                    }
                    else if (Pitch >= 175)
                    {
                        RunMode = 2;
                    }
                    else
                    {
                        RunMode = 0;
                    }
                }
                else if (Player.Stance == EPlayerStance.CROUCH)
                {
                    if (Pitch <= 22)
                    {
                        RunMode = 1;
                    }
                    else if (Pitch >= 155)
                    {
                        RunMode = 2;
                    }
                    else
                    {
                        RunMode = 0;
                    }
                }

                if (RunMode == 0)
                {
                    Player.Player.teleportToLocationUnsafe(Raycast.Raycast.point, Player.Rotation);
                }
                else if (RunMode == 1)
                {
                    if (Raycast.Raycast.distance < 300)
                    {
                        Vector3 Target = new Vector3(Raycast.Raycast.point.x, Raycast.Raycast.point.y + (float)1.75, Raycast.Raycast.point.z);
                        Player.Player.teleportToLocationUnsafe(Target, Player.Rotation);
                    }
                    else
                    {
                        Player.Player.teleportToLocationUnsafe(Raycast.Raycast.point, Player.Rotation);
                    }
                }
                else if (RunMode == 2)
                {
                    if (Raycast.Raycast.distance > 300)
                    {
                        Player.Player.teleportToLocationUnsafe(Raycast.Raycast.point, Player.Rotation);
                    }
                    else
                    {
                        Vector3 ViewDir = Vector3.down;

                        RaycastResult FloorCast = RaycastUtility.Raycast(Player.Position, Vector3.down, RayMasks.GROUND, 1500);
                        float DistanceToGround = 9999;
                        if (FloorCast.RaycastHit)
                        {
                            DistanceToGround = FloorCast.Raycast.distance - (float)0.5;
                        }

                        RaycastResult DownCast = RaycastUtility.RaycastAll(new Vector3(Raycast.Raycast.point.x, Raycast.Raycast.point.y - 3, Raycast.Raycast.point.z), ViewDir, 300);
                        bool Cont = !DownCast.RaycastHit;
                        int Displacement = 3;
                        while (Cont)
                        {
                            DownCast = RaycastUtility.RaycastAll(new Vector3(Raycast.Raycast.point.x, Raycast.Raycast.point.y - Displacement, Raycast.Raycast.point.z), ViewDir, 300);
                            Displacement += 3;
                            if (Displacement > 15 || DownCast.RaycastHit)
                            {
                                Cont = false;
                            }
                        }

                        if (DownCast.RaycastHit && DownCast.Raycast.distance != 0 && DownCast.Raycast.distance < DistanceToGround)
                        {
                            Player.Player.teleportToLocationUnsafe(new Vector3(DownCast.Raycast.point.x, DownCast.Raycast.point.y + 0.2f, DownCast.Raycast.point.z), Player.Rotation);
                        }
                        else
                        {
                            UnturnedChat.Say(Player, "PointTool_Jump_NoPlatformBelow".Translate());
                        }
                    }
                }
            }
            else if (gesture == UnturnedPlayerEvents.PlayerGesture.PunchLeft)
            {
                Vector3 TargetPos = Raycast.Raycast.point;
                float Height = 15;
                RaycastResult[] Placings = {
                    GetDropPlacement(new Vector3(TargetPos.x, TargetPos.y + Height, TargetPos.z)),
                    GetDropPlacement(new Vector3(TargetPos.x + 1, TargetPos.y + Height, TargetPos.z)),
                    GetDropPlacement(new Vector3(TargetPos.x - 1, TargetPos.y + Height, TargetPos.z)),
                    GetDropPlacement(new Vector3(TargetPos.x, TargetPos.y + Height, TargetPos.z + 1)),
                    GetDropPlacement(new Vector3(TargetPos.x, TargetPos.y + Height, TargetPos.z - 1))
                };
                Vector3[] Placing = Placings.Where(X => X.RaycastHit && X.Raycast.distance != 0).OrderByDescending(V => V.Raycast.point.y).CastEnumeration(E => E.Raycast.point).ToArray();

                if (Placing.Count() != 0)
                {
                    Player.Player.teleportToLocationUnsafe(new Vector3(Placing[0].x, Placing[0].y + 0.5f, Placing[0].z), Player.Rotation);
                }
                else
                {
                    Player.Player.teleportToLocationUnsafe(new Vector3(TargetPos.x, TargetPos.y + 0.5f, TargetPos.z), Player.Rotation);
                }
            }
            else if (gesture == UnturnedPlayerEvents.PlayerGesture.Point)
            {
                Vector3 TargetPos = Raycast.Raycast.point;
                Vector3 CurrentPos = Player.Position;
                Vector3 ResultPos = Vector3.MoveTowards(TargetPos, CurrentPos, 1);
                Player.Player.teleportToLocationUnsafe(new Vector3(ResultPos.x, ResultPos.y + 0.5f, ResultPos.z), Player.Rotation);
            }
        }

        public static RaycastResult GetDropPlacement(Vector3 Origin)
        {
            return RaycastUtility.RaycastAll(Origin, Vector3.down, 20);
        }

        public static void SendOpenDoor(ushort plant, byte x, byte y, ushort index, InteractableDoor interactableDoor, BarricadeRegion barricadeRegion)
        {
            if (plant == 65535)
            {
                BarricadeManager.instance.channel.send("tellToggleDoor", ESteamCall.ALL, x, y, BarricadeManager.BARRICADE_REGIONS, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[]
                {
                    x,
                    y,
                    plant,
                    index,
                    !interactableDoor.isOpen
                });
            }
            else
            {
                BarricadeManager.instance.channel.send("tellToggleDoor", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[]
                {
                    x,
                    y,
                    plant,
                    index,
                    !interactableDoor.isOpen
                });
            }
            barricadeRegion.barricades[(int)index].barricade.state[16] = (byte)(interactableDoor.isOpen ? 1 : 0);
            var d = (interactableDoor.isOpen ? (int)1 : (int)0);
        }

        // WIP
        public static void RunRepairTool(UnturnedPlayer Player, RaycastResult Raycast)
        {
            if (Raycast.Vehicle != null)
            {
                VehicleManager.repair(Raycast.Vehicle, 9999, 2);
            }

            if (Raycast.Barricade != null)
            {
                Raycast.Barricade.barricade.askRepair(9999);
                BarricadeManager.repair(Raycast.BarricadeRootTransform, 9999, 2);
            }
            if (Raycast.Structure != null)
            {
                Raycast.Structure.structure.askRepair(9999);
            }
        }
    }
}