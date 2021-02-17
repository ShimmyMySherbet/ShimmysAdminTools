using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Behaviors;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Models;
using ShimmysAdminTools.Modules;
using UnityEngine;

namespace ShimmysAdminTools
{
    public partial class main : RocketPlugin<PluginConfig>
    {
        public static main Instance;
        public static PluginConfig Config;

        public float ServerSpeedMult = 1;
        public float ServerJumpMult = 1;
        public float ServerGravityMult = 1;

        public override void LoadPlugin()
        {

            base.LoadPlugin();
            Instance = this;
            Config = Configuration.Instance;
            PlayerDataStore.Init();
            PlayerSessionStore.Init();
            U.Events.OnBeforePlayerConnected += Events_OnBeforePlayerConnected;
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
            VehicleManager.onEnterVehicleRequested += VehicleManager_onEnterVehicleRequested;
            Rocket.Unturned.Events.UnturnedPlayerEvents.OnPlayerUpdateGesture += UnturnedPlayerEvents_OnPlayerUpdateGesture;

            ChatManager.onCheckPermissions += Chat_OnCheckPermissions;

            LoadCurrentPlayers();

            Level.onLevelLoaded += OnLevelloaded;

            gameObject.AddComponent<RepeatCommandQueue>();

            if (!Config.DelayStartEXECUtility)
            {
                ExecManager.Activate();
            }
        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            new Thread(async () =>
            {
                await Task.Delay(1500);

                Rocket.Core.Utils.TaskDispatcher.QueueOnMainThread(() =>
                {
                    if (ServerJumpMult != 1)
                    {

                        player.Player.movement.sendPluginJumpMultiplier(ServerJumpMult);
                    }

                    if (ServerGravityMult != 1)
                    {

                        player.Player.movement.sendPluginGravityMultiplier(ServerGravityMult);
                    }

                    if (ServerSpeedMult != 1)
                    {
                        player.Player.movement.sendPluginSpeedMultiplier(ServerSpeedMult);
                    }
                });
            }).Start();
        }

        private void OnLevelloaded(int level)
        {
            if (State == PluginState.Loaded && Config.DelayStartEXECUtility && level >= 2)
            {
                ExecManager.Activate();
            }
        }

        private void Chat_CheckCommand(SteamPlayer Player, string Command)
        {
            Command = Command.TrimStart('/', ' ');
            List<string> array = (from Match m in Regex.Matches(Command, "[\\\"](.+?)[\\\"]|([^ ]+)", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled)
                                  select m.Value.Trim('"').Trim()).ToList();
            if (array.Count == 0) return;
            string cmdbase = array[0];
            IRocketCommand RCmd = R.Commands.GetCommand(array[0]);
            if (RCmd == null) return;
            UnturnedPlayer UPlayer = UnturnedPlayer.FromSteamPlayer(Player);
            if (!(R.Permissions.HasPermission(UPlayer, RCmd.Permissions) || R.Permissions.HasPermission(UPlayer, cmdbase))) return;

            array.RemoveAt(0);
            List<string> Modified = new List<string>();
            foreach (string prt in array)
            {
                if (prt.Contains(' ')) Modified.Add($@"""{prt}""");
                else Modified.Add(prt);
            }

            foreach (var ses in PlayerSessionStore.Store.Where(x => x.Value.IsSpyingCommands))
            {
                if ((ses.Value.CommandSpyGlobalEnabled || ses.Value.CommandSpyPlayers.Contains(Player.playerID.steamID.m_SteamID)) && Player.playerID.steamID.m_SteamID != ses.Value.UPlayer.CSteamID.m_SteamID)
                {
                    UnturnedChat.Say(ses.Value.UPlayer, $"[Command Spy] {Player.playerID.characterName} -> /{cmdbase} {string.Join(" ", Modified)}", Color.grey);
                }
            }
        }

        private void Chat_OnCheckPermissions(SteamPlayer player, string text, ref bool shouldExecuteCommand, ref bool shouldList)
        {
            if (text.StartsWith("/") && PlayerSessionStore.RunPlayerCommandSpy())
            {
                try
                {
                    Chat_CheckCommand(player, text);
                }
                catch (Exception)
                {
                }
            }
            PlayerData playerData = PlayerDataStore.GetPlayerData(player.playerID.steamID.m_SteamID);
            if (playerData != null)
            {
                if (playerData.IsMuted && !text.StartsWith(@"/"))
                {
                    if (PlayerMuteExpired(playerData))
                    {
                        playerData.IsMuted = false;
                        playerData.MuteIsTemp = false;
                        return;
                    }
                    else
                    {
                        if (!text.StartsWith(@"/"))
                        {
                            shouldList = false;
                            if (playerData.MuteIsTemp)
                            {
                                TimeSpan TimeLeft = playerData.MuteExpires.Subtract(DateTime.Now);
                                UnturnedChat.Say(player.playerID.steamID, "Mute_ChatBlocked_TimeLeft".Translate(Helpers.GetTimeFromTimespan(TimeLeft)));
                            }
                            else
                            {
                                UnturnedChat.Say(player.playerID.steamID, "Mute_ChatBlocked".Translate());
                            }
                        }
                    }
                }
            }
        }

        public bool PlayerMuteExpired(PlayerData Data)
        {
            if (Data.MuteIsTemp)
            {
                return DateTime.Compare(Data.MuteExpires, DateTime.Now) <= 0;
            }
            else
            {
                return false;
            }
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "Flight_Enabled", "Flight Enabled." },
            { "Flight_Disabled", "Flight Disabled." },
            { "Flight_Enabled_Other", "Enabled {0}'s Flight." },
            { "Flight_Disabled_Other", "Disabled {0}'s Flight." },
            { "Flight_Speed_Reset", "Fly Speed Reset." },
            { "Flight_Speed_NotFlying", "You are not flying." },
            { "Flight_Speed_Changed", "Fly speed set to {0}." },
            { "Flight_Speed_Denied", "You don't have permission to fly that fast." },
            { "Flight_Speed_Denied_Hotkey", "You can't fly any faster." },
            { "Flight_Speed_Vertical_Reset", "Vertical Fly Speed reset." },
            { "Flight_Speed_Vertical_Changed", "Vertical Fly Speed set to {0}." },
            { "Flight_PermitSpeed_InvalidNumber", "Speed needs to be a valid whole number." },
            { "Flight_PermitSpeed_Reset", "Player max fly speed reset." },
            { "Flight_PermitSpeed_Updated", "Player max fly speed updated." },
            { "CheckBedOwner_NotClaimed", "Bed is not claimed." },
            { "CheckBedOwner_ShowOwner", "Bed Owner: {0}" },
            { "CheckBedOwner_NoBedFound", "Nothing found." },
            { "DisableVehicleAccess_NoPlayer", "Failed to find player." },
            { "DisableVehicleAccess_AccessEnabled", "Player may now use vehicles." },
            { "DisableVehicleAccess_AccessDisabled", "Player no longer has access to vehicles." },
            { "ForceDismount_Dismounted", "Player Dismounted." },
            { "ForceDismount_NoVehicle", "Player is not in Vehicle." },
            { "ForceDismount_NoPlayer", "Failed to find player." },
            { "GotoBed_NoPlayer", "Failed to find player." },
            { "GotoBed_NoBed", "Failed to find bed for player with ID of {0}." },
            { "GotoMarker_Teleported", "Teleported to marker {0}." },
            { "GotoMarker_NoMarker", "Failed to find a marker called {0}." },
            { "Marker_Placed", "Placed marker {0}." },
            { "Noclip_Enabled", "Noclip enabled." },
            { "Noclip_Disabled", "Noclip disabled." },
            { "PointTool_EnabledActive", "Point tool enabled, Active Tool: {0}." },
            { "PointTool_Disabled", "Point tool disabled." },
            { "PointTool_Denied", "You don't have access to that tool." },
            { "PointTool_Selected_Destroy", "Destroy tool selected, be careful." },
            { "PointTool_Selected_Repair", "Repair tool selected." },
            { "PointTool_Selected_Utility", "Utility tool selected." },
            { "PointTool_Selected_Kill", "Kill tool selected, be careful." },
            { "PointTool_Selected_Jump", "Jump tool selected." },
            { "PointTool_Extension_Enabled", " Point tool enabled." },
            { "PointTool_Utility_Bed_ShowOwner", "[PointTool] Bed Owner: {0}" },
            { "PointTool_Utility_Bed_NotClaimed", "[PointTool] Bed is not claimed." },
            { "PointTool_Jump_NoPlatformBelow", "Failed to find a platform below you." },
            { "PointTool_Destroy_Denied", "You don't have permission to destroy that." },
            { "Mute_PlayerNotFound", "Player not found." },
            { "Mute_PlayerMuted", "Player Muted." },
            { "Mute_PlayerMuted_Time", "Player Muted for {0}." },
            { "Mute_ChatBlocked", "You cannot speak, you are muted." },
            { "Mute_ChatBlocked_TimeLeft", "You cannot speak, you are muted. {0} left." },
            { "Mute_PlayerUnmuted", "Player Unmuted." },
            { "Mute_PlayerUnmuted_Self", "You have been unmuted." },
            { "Plugin_Error", "An error occurred." },
            { "MapJump_Enabled", "Waypoint jumping enabled." },
            { "MapJump_Disabled", "Waypoint jumping disabled." },
            { "Error_PlayerNotFound", "Failed to find player." },
            { "CommandSpy_Disabled", "Command Spying Disabled." },
            { "CommandSpy_Disabled_Player", "You are no longer spying {0}'s commands." },
            { "CommandSpy_Enabled_Global", "Global Command Spying Enabled." },
            { "CommandSpy_Enabled_Player", "You are now spying on {0}'s commands." },
            { "Firemode_Changed", "Changed firemode to {0}." },
            { "Firemode_Modes", "Modes: Safety, Semi, Burst, Auto." },
            { "Firemode_Unsupported", "You can't change the firemode of this item!" },
            { "Firemode_Unlocker_Enabled", "Weapon firemodes unlocked." },
            { "Firemode_Unlocker_Disabled", "Weapon firemodes reverted." },
            { "UnlimitedAmmo_Enabled", "Unlimited ammo enabled." },
            { "UnlimitedAmmo_Enabled_OverrideAmount", "Unlimited ammo enabled. Overriding max ammo to {0}." },
            { "UnlimitedAmmo_Disabled", "Unlimited ammo disabled." },
            { "UnlimitedAmmo_Fail_Override", "Amount override must be between 0 and 255." },
            { "SetAttachment_Fail_Gun", "You cannot put an attachment on this item." },
            { "SetAttachment_Fail_Item", "Failed to find item." },
            { "SetAttachment_Fail_Blacklist", "This attachment is blacklisted." },
            { "SetAttachment_GaveAttachment", "Gave your gun {0}." },
            { "Fail_Command_Disabled", "This command is disabled." },
            { "Exec_Fail_NoPlayer", "Failed to find player." },
            { "Exec_Fail_NotActive", "ERROR: The EXEC Permissions utility is not active. Try restarting the server." },
            { "ForceTP_Fail_NoPlayer", "Failed to find player" },
            { "ForceTP_Fail_IsConsole", "Can't run this command variation from console." },
            { "ForceTP_Pass_TeleportSelfTo", "Teleported to {0}." },
            { "ForceTP_Pass_TeleportOtherTo", "Teleported {1} to {0}." },
            { "SetGravity_Self_Fail_IsFlying", "Can't change your gravity while flying." },
            { "SetGravity_Self_Fail_InvalidParameter", "Invalid multiplier." },
            { "SetGravity_Self_Pass_GravityChanged", "Set your gravity multiplier to {0}x." },
            { "SetSpeed_Self_Fail_IsFlying", "Can't change your run speed while flying." },
            { "SetSpeed_Self_Fail_InvalidParameter", "Invalid multiplier." },
            { "SetSpeed_Self_Pass_SpeedChanged", "Set your run speed multiplier to {0}x." },
            { "SetJump_Self_Fail_InvalidParameter", "Invalid multiplier." },
            { "SetJump_Self_Pass_JumpChanged", "Set your jump multiplier to {0}x." },

            { "SetGravity_Global_Pass_Changed", "Set server gravity multiplier to {0}x." },
            { "SetSpeed_Global_Pass_Changed", "Set server speed multiplier to {0}x." },
            { "SetJump_Global_Pass_Changed", "Set server jump multiplier to {0}x." },
            { "NoDrain_Enabled", "No Drain Enabled." },
            { "NoDrain_Disabled", "No Drain Disabled." },
            { "NoDrain_OverKill_Disabled", "Overkill Disabled." },
            { "NoDrain_OverKill_Enabled", "Overkill Disabled." }
        };

        private void UnturnedPlayerEvents_OnPlayerUpdateGesture(UnturnedPlayer player, Rocket.Unturned.Events.UnturnedPlayerEvents.PlayerGesture gesture)
        {
            if (gesture != UnturnedPlayerEvents.PlayerGesture.PunchLeft && gesture != UnturnedPlayerEvents.PlayerGesture.PunchRight)
            {
                PointToolManager.ManageGestureUpdate(player, gesture);
            }
        }

        private void Events_OnBeforePlayerConnected(UnturnedPlayer player)
        {
            PlayerSessionStore.TryRegisterPlayer(player);
            PlayerDataStore.TryRegisterPlayer(player);
        }

        private void VehicleManager_onEnterVehicleRequested(Player player, InteractableVehicle vehicle, ref bool shouldAllow)
        {
            var Data = PlayerDataStore.GetPlayerData(UnturnedPlayer.FromPlayer(player));
            if (Data != null)
            {
                if (!Data.CanEnterVehicle) shouldAllow = false;
            }
        }

        public void ForEachNonFlyingPlayer(Action<Player> action)
        {
            foreach (SteamPlayer client in Provider.clients)
            {
                PlayerSession session = PlayerSessionStore.GetPlayerData(client.player);
                if (session != null && !session.FlySessionActive)
                {
                    action(client.player);
                }
            }
        }

        private void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {
            PlayerSessionStore.TryDeregisterPlayer(player);
        }

        public void LoadCurrentPlayers()
        {
            foreach (var client in Provider.clients)
            {
                UnturnedPlayer Player = UnturnedPlayer.FromSteamPlayer(client);
                PlayerDataStore.TryRegisterPlayer(Player);
                PlayerSessionStore.TryRegisterPlayer(Player);
            }
        }

        public override void UnloadPlugin(PluginState state = PluginState.Unloaded)
        {
            ExecManager.Deactivate();
            Level.onLevelLoaded -= OnLevelloaded;
            U.Events.OnBeforePlayerConnected -= Events_OnBeforePlayerConnected;
            U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnected;
            VehicleManager.onEnterVehicleRequested -= VehicleManager_onEnterVehicleRequested;
            Rocket.Unturned.Events.UnturnedPlayerEvents.OnPlayerUpdateGesture -= UnturnedPlayerEvents_OnPlayerUpdateGesture;
            foreach (var Session in PlayerSessionStore.Store)
            {
                if (Session.Value.FlySessionActive) Session.Value.FlySession.Stop();
                if (Session.Value.NoClipSessionActive) Session.Value.NoClip.Stop();
            }
            base.UnloadPlugin(state);
        }
    }
}