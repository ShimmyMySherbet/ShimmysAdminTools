using System.Linq;
using System.Reflection;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Models;
using Steamworks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace ShimmysAdminTools.Modules
{
	/// <summary>
	/// Delegate representing a PointTool interaction event
	/// </summary>
	/// <param name="tool">The name of the tool used</param>
	/// <param name="actor">The actor of the tool</param>
	/// <param name="point">The location the tool is triggered</param>
	/// <param name="target">The target object type</param>
	/// <param name="targetHandle">A handle of the target, or more info on the target</param>
	/// <param name="denied">When true, the interaction was denied due to missing permissions</param>
	public delegate void PointToolInteractionEvent(string tool, UnturnedPlayer actor, Vector3 point, string target, string targetHandle, bool denied);

	public static class PointToolManager
	{
		public static event PointToolInteractionEvent OnPointToolInteraction;

		public static void ManageGestureUpdate(UnturnedPlayer actor, UnturnedPlayerEvents.PlayerGesture gesture)
		{
			PlayerSession Session = PlayerSessionStore.GetPlayerData(actor);
			if (gesture == UnturnedPlayerEvents.PlayerGesture.Salute)
			{
				if (Session.FlySessionActive)
				{
					Session.FlySession.Speed = 1;
					Session.FlySession.SendUpdateSpeed();
				}
			}
			if (gesture == UnturnedPlayerEvents.PlayerGesture.Point || gesture == UnturnedPlayerEvents.PlayerGesture.PunchLeft || gesture == UnturnedPlayerEvents.PlayerGesture.PunchRight)
			{
				if (Session.PointToolEnabled)
				{
					RunPointTool(actor, Session, gesture);
				}
			}
		}

		/// <summary>
		/// Raises the inetraction event, and logs the event to console.
		/// </summary>
		public static void LogInteraction(string tool, UnturnedPlayer actor, Vector3 point, string targetType, object targetHandle, bool denied = false)
		{
			Logger.Log($"[PointTool:{tool}] [{(denied ? "Denied" : "Granted")}] [{actor?.DisplayName}:{actor?.CSteamID.m_SteamID}] {{{point.x}, {point.y}, {point.z}}} Target: {targetType} {targetHandle}");
			OnPointToolInteraction?.Invoke(tool, actor, point, targetType, targetHandle?.ToString(), denied);
		}

		public static void RunPointTool(UnturnedPlayer actor, PlayerSession session, UnturnedPlayerEvents.PlayerGesture gesture)
		{
			var isUsingBinoculars = actor.Player.equipment.isSelected && actor.Player.equipment.asset.id == 333;

			if (session.PointTool == PointToolMode.Destroy)
			{
				RaycastResult Raycast = RaycastUtility.RayCastPlayer(actor, RayMasks.BARRICADE | RayMasks.STRUCTURE | RayMasks.VEHICLE | RayMasks.RESOURCE, isUsingBinoculars ? 10000 : 100);
				if (Raycast.RaycastHit)
				{
					RunDestroyTool(actor, Raycast);
				}
			}
			else if (session.PointTool == PointToolMode.Utility)
			{
				RaycastResult Raycast = RaycastUtility.RayCastPlayer(actor, RayMasks.BARRICADE | RayMasks.STRUCTURE | RayMasks.VEHICLE | RayMasks.BARRICADE_INTERACT, isUsingBinoculars ? 10000 : 100);
				if (Raycast.RaycastHit)
				{
					RunUtilityTool(actor, Raycast);
				}
			}
			else if (session.PointTool == PointToolMode.Repair)
			{
				RaycastResult Raycast = RaycastUtility.RayCastPlayer(actor, RayMasks.BARRICADE | RayMasks.STRUCTURE | RayMasks.VEHICLE, isUsingBinoculars ? 10000 : 100);
				if (Raycast.RaycastHit)
				{
					RunRepairTool(actor, Raycast, gesture == UnturnedPlayerEvents.PlayerGesture.PunchLeft);
				}
			}
			else if (session.PointTool == PointToolMode.Kill)
			{
				RaycastResult CloseEnemyCheck = RaycastUtility.RayCastPlayer(actor, RayMasks.AGENT | RayMasks.ENEMY, isUsingBinoculars ? 500 : 7);
				RaycastResult ClosePlayerCheck = RaycastUtility.RayCastPlayer(actor, RayMasks.PLAYER, isUsingBinoculars ? 700 : 10);
				if (ClosePlayerCheck.RaycastHit && ClosePlayerCheck.ParentHasComponent<Player>() && ClosePlayerCheck.TryGetEntity<Player>().channel.owner.playerID.steamID.m_SteamID != actor.CSteamID.m_SteamID)
				{
					RunKillTool(actor, ClosePlayerCheck);
				}
				else if (CloseEnemyCheck.RaycastHit)
				{
					RunKillTool(actor, CloseEnemyCheck);
				}
				else
				{
					Vector3 RaycastSpot = actor.Player.look.aim.position + (actor.Player.look.aim.forward.normalized * 0.5f);
					RaycastResult Raycast = RaycastUtility.Raycast(RaycastSpot, actor.Player.look.aim.forward, RayMasks.ENEMY | RayMasks.PLAYER | RayMasks.AGENT);
					if (Raycast.RaycastHit)
					{
						RunKillTool(actor, Raycast);
					}
				}
			}
			else if (session.PointTool == PointToolMode.Jump)
			{
				RaycastResult Raycast = RaycastUtility.RayCastPlayerAll(actor, 5000);
				if (Raycast.RaycastHit)
				{
					RunJumpTool(actor, Raycast, gesture);
				}
			}
			else if (session.PointTool == PointToolMode.CheckOwner)
			{
				var raycast = RaycastUtility.RayCastPlayer(actor, RayMasks.VEHICLE | RayMasks.STRUCTURE | RayMasks.BARRICADE, isUsingBinoculars ? 10000 : 100);

				RunCheckownerTool(actor, raycast);
			}
		}

		public static void RunCheckownerTool(UnturnedPlayer actor, RaycastResult raycast)
		{
			if (raycast.Barricade != null)
			{
				var bOwner = raycast.Barricade.owner;
				var bGroup = raycast.Barricade.group;

				AdminToolsPlugin.Instance.TryGetPlayerName(bOwner, out var playerName);
				if (playerName == null)
					playerName = "Unknown Player";

				//Logger.Log($"[PointTool:CheckOwner] ({player.DisplayName}:{player.CSteamID.m_SteamID}) Barricade");
				LogInteraction("CheckOwner", actor, raycast.Raycast.point, "Barricade", raycast.Barricade.barricade.asset.id);
				if (bGroup != 0)
				{
					var inGameGroup = GroupManager.getGroupInfo(new CSteamID(bGroup));

					if (inGameGroup != null)
					{
						UnturnedChat.Say(actor, "PointTool_CheckOwner_Barricade_Group".Translate(playerName, bOwner, inGameGroup.name, raycast.Barricade.barricade.asset.id));

						return;
					}
				}

				UnturnedChat.Say(actor, "PointTool_CheckOwner_Barricade".Translate(playerName, bOwner, raycast.Barricade.barricade.asset.id));
				return;
			}
			else if (raycast.Structure != null)
			{
				var sOwner = raycast.Structure.owner;
				var sGroup = raycast.Structure.group;

				AdminToolsPlugin.Instance.TryGetPlayerName(sOwner, out var playerName);
				if (playerName == null)
					playerName = "Unknown Player";
				//Logger.Log($"[PointTool:CheckOwner] ({player.DisplayName}:{player.CSteamID.m_SteamID}) Structure");
				LogInteraction("CheckOwner", actor, raycast.Raycast.point, "Structure", raycast.Structure.structure.asset.id);

				if (sGroup != 0)
				{
					var inGameGroup = GroupManager.getGroupInfo(new CSteamID(sGroup));

					if (inGameGroup != null)
					{
						UnturnedChat.Say(actor, "PointTool_CheckOwner_Structure_Group".Translate(playerName, sOwner, inGameGroup.name, raycast.Structure.structure.asset.id));
						return;
					}
				}

				UnturnedChat.Say(actor, "PointTool_CheckOwner_Structure".Translate(playerName, sOwner, raycast.Structure.structure.asset.id));
				return;
			}
			else if (raycast.Vehicle != null)
			{
				if (!raycast.Vehicle.isLocked)
				{
					return;
				}

				var vOwner = raycast.Vehicle.lockedOwner.m_SteamID;
				var vGroup = raycast.Vehicle.lockedGroup;

				AdminToolsPlugin.Instance.TryGetPlayerName(vOwner, out var playerName);
				if (playerName == null)
					playerName = "Unknown Player";
				//Logger.Log($"[PointTool:CheckOwner] ({player.DisplayName}:{player.CSteamID.m_SteamID}) Vehicle");
				LogInteraction("CheckOwner", actor, raycast.Raycast.point, "Vehicle", raycast.Vehicle.id);

				if (vGroup.m_SteamID != 0)
				{
					var inGameGroup = GroupManager.getGroupInfo(vGroup);

					if (inGameGroup != null)
					{
						UnturnedChat.Say(actor, "PointTool_CheckOwner_Vehicle_Group".Translate(playerName, vOwner, inGameGroup.name, raycast.Vehicle.asset.id));
						return;
					}
				}

				UnturnedChat.Say(actor, "PointTool_CheckOwner_Vehicle".Translate(playerName, vOwner, raycast.Vehicle.asset.id));
				return;
			}
		}

		public static void RunKillTool(UnturnedPlayer actor, RaycastResult raycast)
		{
			if (raycast.ParentHasComponent<Player>())
			{
				Player player = raycast.TryGetEntity<Player>();
				if (player.channel.owner.playerID.steamID.m_SteamID == actor.CSteamID.m_SteamID) return;
				player.life.askDamage(100, player.look.aim.forward, EDeathCause.KILL, ELimb.SKULL, actor.CSteamID, out _, true, ERagdollEffect.GOLD, true, true);
				//Logger.Log($"[PointTool:Kill] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) Player {player.name} ({player.channel.owner.playerID.steamID.m_SteamID})");
				LogInteraction("Kill", actor, raycast.Raycast.point, "Player", $"{player.name} [{player.channel.owner.playerID.steamID.m_SteamID}]");

			}
			else if (raycast.ParentHasComponent<Zombie>())
			{
				Zombie zombie = raycast.TryGetEntity<Zombie>();
				zombie.killWithFireExplosion();
				//Logger.Log($"[PointTool:Kill] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) Zombie");
				LogInteraction("Kill", actor, raycast.Raycast.point, "Zombie", null);

			}
			else if (raycast.ParentHasComponent<Animal>())
			{
				raycast.TryGetEntity<Animal>().askDamage(ushort.MaxValue, Vector3.one, out _, out _, false, true, ERagdollEffect.NONE);
				//Logger.Log($"[PointTool:Kill] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) Animal");
				LogInteraction("Kill", actor, raycast.Raycast.point, "Animal", null);
			}
		}

		public static void RunDestroyTool(UnturnedPlayer actor, RaycastResult raycast)
		{
			if (raycast.Vehicle != null)
			{
				bool IsPlayersVehicle = raycast.Vehicle.lockedOwner == actor.CSteamID || raycast.Vehicle.lockedGroup == actor.SteamGroupID;
				bool Allow = IsPlayersVehicle;
				if (!IsPlayersVehicle) Allow = PlayerCanDestroyOtherPlayersStuff(actor);
				if (Allow)
				{
					//Logger.Log($"[PointTool:Destroy] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) Destroyed vehicle {raycast.Vehicle.asset.name} ({raycast.Vehicle.id})");
				LogInteraction("Destroy", actor, raycast.Raycast.point, "Vehicle", raycast.Vehicle.id);
					VehicleManager.askVehicleDestroy(raycast.Vehicle);
				}
				else
				{
					//Logger.Log($"[PointTool:Destroy] [Denied] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) on vehicle {raycast.Vehicle.asset.name} ({raycast.Vehicle.id})");
					LogInteraction("Destroy", actor, raycast.Raycast.point, "Vehicle", raycast.Vehicle.id, denied: true);
					UnturnedChat.Say(actor, "PointTool_Destroy_Denied".Translate());
				}
			}
			if (raycast.Barricade != null)
			{
				bool IsPlayersBarricade = raycast.Barricade.owner == actor.CSteamID.m_SteamID || raycast.Barricade.group == actor.SteamGroupID.m_SteamID;
				bool Allow = IsPlayersBarricade;
				if (!IsPlayersBarricade) Allow = PlayerCanDestroyOtherPlayersStuff(actor);
				if (Allow)
				{
					//Logger.Log($"[PointTool:Destroy] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) Destroyed barricade {raycast.Barricade.barricade.asset.name} ({raycast.Barricade.barricade.asset.id})");

					LogInteraction("Destroy", actor, raycast.Raycast.point, "Barricade", $"{raycast.Barricade.barricade.asset.name} ({raycast.Barricade.barricade.asset.id})", denied: false);

					BarricadeManager.damage(raycast.BarricadeRootTransform.transform, raycast.Barricade.barricade.health, 1, false, actor.CSteamID);
                }
				else
				{
					//Logger.Log($"[PointTool:Destroy] [Denied] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) on barricade {raycast.Barricade.barricade.asset.name} ({raycast.Barricade.barricade.asset.id})");
					LogInteraction("Destroy", actor, raycast.Raycast.point, "Barricade", $"{raycast.Barricade.barricade.asset.name} ({raycast.Barricade.barricade.asset.id})", denied: true);
					UnturnedChat.Say(actor, "PointTool_Destroy_Denied".Translate());
				}
			}
			if (raycast.Structure != null)
			{
				bool IsPlayersStructure = raycast.Structure.owner == actor.CSteamID.m_SteamID || raycast.Structure.group == actor.SteamGroupID.m_SteamID;
				bool Allow = IsPlayersStructure;
				if (!IsPlayersStructure) Allow = PlayerCanDestroyOtherPlayersStuff(actor);
				if (Allow)
				{
					//Logger.Log($"[PointTool:Destroy] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) on structure {raycast.Structure.structure.asset.name} ({raycast.Barricade.barricade.asset.id})");
					LogInteraction("Destroy", actor, raycast.Raycast.point, "Structure", $"{raycast.Structure.structure.asset.name} ({raycast.Structure.structure.asset.id})", denied: false);
					StructureManager.damage(raycast.StructureRootTransform.transform, new Vector3(0, 0, 0), raycast.Structure.structure.health, 1, false, actor.CSteamID);
				}
				else
				{
					LogInteraction("Destroy", actor, raycast.Raycast.point, "Structure", $"{raycast.Structure.structure.asset.name} ({raycast.Structure.structure.asset.id})", denied: true);
					UnturnedChat.Say(actor, "PointTool_Destroy_Denied".Translate());
				}
			}
			if (raycast.Raycast.transform.CompareTag("Resource"))
			{
				//Logger.Log($"[PointTool:Destroy] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) on Resoruce Node");
				LogInteraction("Destroy", actor, raycast.Raycast.point, "Resource", null, denied: false);
				ResourceManager.damage(raycast.Raycast.transform, Vector3.up * 10, 10000, 10, 1, out var kill, out var isnt);
			}
		}

		public static bool PlayerCanDestroyOtherPlayersStuff(UnturnedPlayer Player)
		{
			return Player.HasPermission("ShimmysAdminTools.PointTool.DestroyOtherPlayersStuff") ||
				Player.HasPermission("ShimmysAdminTools.PointTool.all") ||
				Player.HasPermission("ShimmysAdminTools.PointTool.*");
		}

		public static void RunUtilityTool(UnturnedPlayer actor, RaycastResult raycast)
		{
			if (raycast.ParentHasComponent<InteractableCharge>())
			{
				//Logger.Log($"[PointTool:Utility] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) on Charge");
				LogInteraction("Utility", actor, raycast.Raycast.point, "Charge", null, denied: false);
				raycast.TryGetEntity<InteractableCharge>().detonate(actor.CSteamID);
				return;
			}

			if (raycast.ParentHasComponent<InteractableFire>())
			{
				//Logger.Log($"[PointTool:Utility] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) on Fireplace");
				LogInteraction("Utility", actor, raycast.Raycast.point, "Fireplace", null, denied: false);
				var f = raycast.TryGetEntity<InteractableFire>();
				BarricadeManager.ServerSetFireLit(f, !f.isLit);
				return;
			}

			if (raycast.ParentHasComponent<InteractableGenerator>())
			{
				//Logger.Log($"[PointTool:Utility] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) on Generator");
				LogInteraction("Utility", actor, raycast.Raycast.point, "Generator", null, denied: false);
				var f = raycast.TryGetEntity<InteractableGenerator>();
				BarricadeManager.ServerSetGeneratorPowered(f, !f.isPowered);
				return;
			}

			if (raycast.ParentHasComponent<InteractableOven>())
			{
				LogInteraction("Utility", actor, raycast.Raycast.point, "Oven", null, denied: false);
				var f = raycast.TryGetEntity<InteractableOven>();
				BarricadeManager.ServerSetOvenLit(f, !f.isLit);
				return;
			}

			if (raycast.ParentHasComponent<InteractableOxygenator>())
			{
				LogInteraction("Utility", actor, raycast.Raycast.point, "Oxygenator", null, denied: false);
				var f = raycast.TryGetEntity<InteractableOxygenator>();
				BarricadeManager.ServerSetOxygenatorPowered(f, !f.isPowered);
				return;
			}

			if (raycast.ParentHasComponent<InteractableSafezone>())
			{
				LogInteraction("Utility", actor, raycast.Raycast.point, "Safezone", null, denied: false);
				var f = raycast.TryGetEntity<InteractableSafezone>();
				BarricadeManager.ServerSetSafezonePowered(f, !f.isPowered);
				return;
			}

			if (raycast.ParentHasComponent<InteractableSpot>())
			{
				LogInteraction("Utility", actor, raycast.Raycast.point, "Light", null, denied: false);
				var f = raycast.TryGetEntity<InteractableSpot>();
				BarricadeManager.ServerSetSpotPowered(f, !f.isPowered);
				return;
			}

			if (raycast.ParentHasComponent<InteractableBed>())
			{
				var f = raycast.TryGetEntity<InteractableBed>();
				LogInteraction("Utility", actor, raycast.Raycast.point, "Bed", $"(owned by {f.owner.m_SteamID})", denied: false);

				if (f.owner.m_SteamID != 0)
				{
					UnturnedChat.Say(actor, "PointTool_Utility_Bed_ShowOwner".Translate($"{AdminToolsPlugin.Instance.GetPlayerName(f.owner.m_SteamID, "Unknown Player")} ({f.owner})"));
				}
				else
				{
					UnturnedChat.Say(actor, "PointTool_Utility_Bed_NotClaimed".Translate()); ;
				}
				return;
			}
			if (raycast.ParentHasComponent<InteractableDoor>())
			{
				var f = raycast.TryGetEntity<InteractableDoor>();
				LogInteraction("Utility", actor, raycast.Raycast.point, "Door", $"(owned by {f.owner.m_SteamID})", denied: false);

				SendOpenDoor(raycast.BarricadePlant, raycast.BarricadeX, raycast.BarricadeY, raycast.BarricadeIndex, f, raycast.BarricadeRegion);
				return;
			}


			if (raycast.ParentHasComponent<InteractableStorage>())
			{
				InteractableStorage Storage = raycast.TryGetEntity<InteractableStorage>();
				//Logger.Log($"[PointTool:Utility] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) on Storage (owned by {Storage.owner.m_SteamID})");
				LogInteraction("Utility", actor, raycast.Raycast.point, "Storage", $"(owned by {Storage.owner.m_SteamID})", denied: false);

				actor.Player.inventory.updateItems(7, Storage.items);
				actor.Player.inventory.sendStorage();
				return;
			}

			if (raycast.HasComponent<InteractableStorage>())
			{
				InteractableStorage Storage = raycast.GetComponent<InteractableStorage>();
				LogInteraction("Utility", actor, raycast.Raycast.point, "Storage", $"(owned by {Storage.owner.m_SteamID})", denied: false);
				actor.Player.inventory.updateItems(7, Storage.items);
				actor.Player.inventory.sendStorage();
				return;
			}

			if (raycast.ParentHasComponent<InteractableVehicle>())
			{
				var f = raycast.TryGetEntity<InteractableVehicle>();
				//Logger.Log($"[PointTool:Utility] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) on Vehicle (owned by {f.lockedOwner.m_SteamID})");
				LogInteraction("Utility", actor, raycast.Raycast.point, "Vehicle", $"(locked by {f.lockedOwner.m_SteamID})", denied: false);

				VehicleManager.ServerForcePassengerIntoVehicle(actor.Player, f);
			}
		}

		public static void RunJumpTool(UnturnedPlayer actor, RaycastResult raycast, UnturnedPlayerEvents.PlayerGesture gesture)
		{
			if (gesture == UnturnedPlayerEvents.PlayerGesture.PunchRight)
			{
				int RunMode = 0;
				float Pitch = actor.Player.look.pitch;
				LogInteraction("Jump", actor, raycast.Raycast.point, "Clip in", null, denied: false);

				if (actor.Stance == EPlayerStance.STAND)
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
				else if (actor.Stance == EPlayerStance.CROUCH)
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
					// in
					//Logger.Log($"[PointTool:Jump] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) to {raycast.Raycast.point}");

					actor.Player.teleportToLocationUnsafe(raycast.Raycast.point, actor.Rotation);
				}
				else if (RunMode == 1)
				{
					// to

					if (raycast.Raycast.distance < 300)
					{
						Vector3 Target = new Vector3(raycast.Raycast.point.x, raycast.Raycast.point.y + (float)1.75, raycast.Raycast.point.z);
						//Logger.Log($"[PointTool:Jump] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) to {Target}");
						actor.Player.teleportToLocationUnsafe(Target, actor.Rotation);
					}
					else
					{
						//Logger.Log($"[PointTool:Jump] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) to {raycast.Raycast.point}");
						actor.Player.teleportToLocationUnsafe(raycast.Raycast.point, actor.Rotation);
					}
				}
				else if (RunMode == 2)
				{
					// near jump

					if (raycast.Raycast.distance > 300)
					{
						//Logger.Log($"[PointTool:Jump] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) to {raycast.Raycast.point}");
						actor.Player.teleportToLocationUnsafe(raycast.Raycast.point, actor.Rotation);
					}
					else
					{
						Vector3 ViewDir = Vector3.down;

						RaycastResult FloorCast = RaycastUtility.Raycast(actor.Position, Vector3.down, RayMasks.GROUND, 1500);
						float DistanceToGround = 9999;
						if (FloorCast.RaycastHit)
						{
							DistanceToGround = FloorCast.Raycast.distance - (float)0.5;
						}

						RaycastResult DownCast = RaycastUtility.RaycastAll(new Vector3(raycast.Raycast.point.x, raycast.Raycast.point.y - 3, raycast.Raycast.point.z), ViewDir, 300);
						bool Cont = !DownCast.RaycastHit;
						int Displacement = 3;
						while (Cont)
						{
							DownCast = RaycastUtility.RaycastAll(new Vector3(raycast.Raycast.point.x, raycast.Raycast.point.y - Displacement, raycast.Raycast.point.z), ViewDir, 300);
							Displacement += 3;
							if (Displacement > 15 || DownCast.RaycastHit)
							{
								Cont = false;
							}
						}

						if (DownCast.RaycastHit && DownCast.Raycast.distance != 0 && DownCast.Raycast.distance < DistanceToGround)
						{
							var dest = new Vector3(DownCast.Raycast.point.x, DownCast.Raycast.point.y + 0.2f, DownCast.Raycast.point.z);
							//Logger.Log($"[PointTool:Jump] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) to {dest}");

							actor.Player.teleportToLocationUnsafe(dest, actor.Rotation);
						}
						else
						{
							UnturnedChat.Say(actor, "PointTool_Jump_NoPlatformBelow".Translate());
						}
					}
				}
			}
			else if (gesture == UnturnedPlayerEvents.PlayerGesture.PunchLeft)
			{
				LogInteraction("Jump", actor, raycast.Raycast.point, "Level", null, denied: false);

				Vector3 TargetPos = raycast.Raycast.point;
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
					var target = Placing[0] + new Vector3(0, 0.5f, 0);
					//Logger.Log($"[PointTool:Jump] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) to {target}");

					actor.Player.teleportToLocationUnsafe(target, actor.Rotation);
				}
				else
				{

					var target = TargetPos + new Vector3(0, 0.5f, 0);
					//Logger.Log($"[PointTool:Jump] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) to {target}");
					actor.Player.teleportToLocationUnsafe(target, actor.Rotation);
				}
			}
			else if (gesture == UnturnedPlayerEvents.PlayerGesture.Point)
			{
				LogInteraction("Jump", actor, raycast.Raycast.point, "To", null, denied: false);

				Vector3 TargetPos = raycast.Raycast.point;
				Vector3 CurrentPos = actor.Position;
				Vector3 ResultPos = Vector3.MoveTowards(TargetPos, CurrentPos, 1);
				var target = ResultPos + new Vector3(0, 0.5f, 0);
				//Logger.Log($"[PointTool:Jump] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) to {target}");
				actor.Player.teleportToLocationUnsafe(new Vector3(ResultPos.x, ResultPos.y + 0.5f, ResultPos.z), actor.Rotation);
			}
		}

		public static RaycastResult GetDropPlacement(Vector3 Origin)
		{
			return RaycastUtility.RaycastAll(Origin, Vector3.down, 20);
		}

		public static void SendOpenDoor(ushort plant, byte x, byte y, ushort index, InteractableDoor interactableDoor, BarricadeRegion barricadeRegion)
		{
			BarricadeManager.ServerSetDoorOpen(interactableDoor, !interactableDoor.isOpen);
		}

		private static readonly FieldInfo f_VehicleHealth = typeof(VehicleManager).GetField("health", BindingFlags.NonPublic | BindingFlags.Instance);
		private static readonly MethodInfo m_SendBarricadeHealth = typeof(BarricadeManager).GetMethod("sendHealthChanged", BindingFlags.NonPublic | BindingFlags.Static);
		private static readonly MethodInfo m_SendStructureHealth = typeof(StructureManager).GetMethod("sendHealthChanged", BindingFlags.NonPublic | BindingFlags.Static);

		public static void RunRepairTool(UnturnedPlayer actor, RaycastResult raycast, bool repairAllTires = false)
		{
			if (raycast.Vehicle != null)
			{
				raycast.Vehicle.askRepair(9999);

				if (raycast.Vehicle.tires != null)
				{
					for (int i = 0; i < raycast.Vehicle.tires.Length; i++)
					{
						var tire = raycast.Vehicle.tires[i];
						if (!tire.isAlive && (repairAllTires || Vector3.Distance(tire?.wheel?.transform?.position ?? Vector3.zero, raycast.Raycast.point) <= 1.5f || raycast.Raycast.transform == tire.wheel?.transform))
						{
							LogInteraction("Repair", actor, raycast.Raycast.point, "Vehicle", null, denied: false);
							//Logger.Log($"[PointTool:Repair] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) Tire");
							tire.askRepair();
						}
					}
				}
			}

			if (raycast.Barricade != null)
			{
				raycast.Barricade.barricade.askRepair(9999);
				var drop = BarricadeManager.regions[raycast.BarricadeX, raycast.BarricadeY].drops[raycast.BarricadeIndex];
				m_SendBarricadeHealth.Invoke(null, new object[] { raycast.BarricadeX, raycast.BarricadeY, raycast.BarricadePlant, drop });
				//Logger.Log($"[PointTool:Repair] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) Barricade {raycast.Barricade.barricade.asset.name}");
				LogInteraction("Repair", actor, raycast.Raycast.point, "Barricade", null, denied: false);

			}
			if (raycast.Structure != null)
			{
				raycast.Structure.structure.askRepair(9999);
				var drop = StructureManager.regions[raycast.StructureX, raycast.StructureY].drops[raycast.StructureIndex];
				m_SendStructureHealth.Invoke(null, new object[] { raycast.StructureX, raycast.StructureY, drop });
				//Logger.Log($"[PointTool:Repair] ({actor.DisplayName}:{actor.CSteamID.m_SteamID}) Structure {raycast.Structure.structure.asset.name}");
				LogInteraction("Repair", actor, raycast.Raycast.point, "Structure", null, denied: false);
			}
		}
	}
}