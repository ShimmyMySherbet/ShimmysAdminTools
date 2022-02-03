using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Core.Steam;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Behaviors;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Models;
using ShimmysAdminTools.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace ShimmysAdminTools
{
    public partial class AdminToolsPlugin : RocketPlugin<PluginConfig>
    {
        public static AdminToolsPlugin Instance;
        public static PluginConfig Config;

        public float ServerSpeedMult = 1;
        public float ServerJumpMult = 1;
        public float ServerGravityMult = 1;

        public Dictionary<ulong, string> NamesCache = new Dictionary<ulong, string>();

        public bool ExecPluginPresent => File.Exists(Path.Combine(Rocket.Core.Environment.PluginsDirectory, "ExecPlugin.dll"));

        public IRocketCommand ExecCommandRedirect { get; private set; } = null;
        public IRocketCommand ExecAllCommandRedirect { get; private set; } = null;

        public override void LoadPlugin()
        {
            Logger.Log($"Loading ShimmysAdminTools v{UpdaterCore.CurrentVersion} by ShimmyMySherbet");
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

            Logger.Log("Checking for updates...");
            UpdaterCore.Init();

            if (UpdaterCore.IsOutDated)
            {
                Logger.LogWarning("ShimmysAdminTools is out of date!");
                Logger.Log($"Latest Version: v{UpdaterCore.LatestVersion}");
                if (UpdaterCore.TryGetUpdateMessage(out string msg))
                {
                    Logger.Log($"Update Notes:");
                    Logger.Log(msg);
                }
                Logger.Log("Download the latest update at https://github.com/ShimmyMySherbet/ShimmysAdminTools");
            }

            gameObject.AddComponent<RepeatCommandQueue>();

            if (Config.ExecEnabled)
            {
                if (ExecPluginPresent)
                {
                    Logger.Log("Detected ExecPlugin. Skipping load of exec module.");
                }
                else if (!Config.DelayStartEXECUtility)
                {
                    ExecManager.Activate();
                }
            }

            Logger.Log("ShimmysAdminTools loaded.");
        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            lock (NamesCache)
            {
                NamesCache[player.CSteamID.m_SteamID] = player.DisplayName;
            }

            ThreadPool.QueueUserWorkItem(async (_) =>
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
             });
        }

        private void OnLevelloaded(int level)
        {
            if (ExecPluginPresent)
            {
                Logger.Log("Adding command redirects for ExecPlugin...");
                var asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name.Equals("ExecPlugin", StringComparison.InvariantCultureIgnoreCase));
                if (asm != null)
                {
                    var execCommand = asm.GetType("ShimmyMySherbet.ExecPlugin.Commands.ExecCommand");
                    var execAllCommand = asm.GetType("ShimmyMySherbet.ExecPlugin.Commands.ExecAllCommand");

                    if (execCommand != null)
                    {
                        ExecCommandRedirect = (IRocketCommand)Activator.CreateInstance(execCommand);
                        Logger.Log("Added redirection for /exec from ShimmysAdminTools to ExecPlugin");
                    }
                    else
                    {
                        Logger.Log("Failed to add redirection for /exec");
                    }

                    if (execAllCommand != null)
                    {
                        ExecAllCommandRedirect = (IRocketCommand)Activator.CreateInstance(execAllCommand);
                        Logger.Log("Added redirection for /execall from ShimmysAdminTools to ExecPlugin");
                    }
                    else
                    {
                        Logger.Log("Failed to add redirection for /execall");
                    }
                }
                else
                {
                    Logger.Log("Failed to locate ExecPlugin");
                }
            }

            if (Config.ExecEnabled && Config.DelayStartEXECUtility)
            {
                if (ExecPluginPresent)
                {
                    Logger.Log("Detected ExecPlugin. Skipping load of exec module.");
                }
                else if (State == PluginState.Loaded && level >= 2)
                {
                    ExecManager.Activate();
                }
            }
        }

        private void Chat_OnCheckPermissions(SteamPlayer player, string text, ref bool shouldExecuteCommand, ref bool shouldList)
        {
            if (text == null || !shouldExecuteCommand) return;

            if (text.Trim(' ').StartsWith("/"))
            {
                List<string> commandParts = (from Match m in Regex.Matches(text, "[\\\"](.+?)[\\\"]|([^ ]+)", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled)
                                             select m.Value.Trim('"').Trim()).ToList();
                //check if valid command
                IRocketCommand targetCommand = R.Commands.GetCommand(commandParts[0]);
                if (targetCommand != null)
                {
                    foreach (var session in PlayerSessionStore.Store.Where(x => x.Value.IsSpyingCommands))
                    {
                        if (session.Key != player.playerID.steamID.m_SteamID && (session.Value.CommandSpyGlobalEnabled || session.Value.CommandSpyPlayers.Contains(player.playerID.steamID.m_SteamID)))
                        {
                            UnturnedChat.Say(session.Value.UPlayer, $"[Command Spy] {player.playerID.characterName} -> /{text.Trim(' ')}", Color.grey);
                        }
                    }
                }
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
            { "PointTool_Selected_CheckOwner", "Check Owner tool selected." },
            { "PointTool_Extension_Enabled", " Point tool enabled." },
            { "PointTool_Utility_Bed_ShowOwner", "[PointTool] Bed Owner: {0}" },
            { "PointTool_Utility_Bed_NotClaimed", "[PointTool] Bed is not claimed." },
            { "PointTool_Jump_NoPlatformBelow", "Failed to find a platform below you." },
            { "PointTool_Destroy_Denied", "You don't have permission to destroy that." },
            { "PointTool_CheckOwner_Barricade_Group", "[{3}] Owner: {0} ({1}), Group: {2}" },
            { "PointTool_CheckOwner_Barricade", "[{2}] Owner: {0} ({1})" },
            { "PointTool_CheckOwner_Structure_Group", "[{3}] Owner: {0} ({1}), Group: {2}" },
            { "PointTool_CheckOwner_Structure", "[{2}] Owner: {0} ({1})" },
            { "PointTool_CheckOwner_Vehicle_Group", "[{3}] Owner: {0} ({1}), Group: {2}" },
            { "PointTool_CheckOwner_Vehicle", "[{2}] Owner: {0} ({1})" },
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
            { "NoDrain_OverKill_Enabled", "Overkill Disabled." },

            { "CheckOwner_Fail_NotFound", "Nothing Found." },
            { "CheckOwner_Pass_Found", "Owner: {0} ({1})." },
            { "CheckOwner_Pass_Found_Bed", "Owner: {0} ({1}). Bed claimed by {2} ({3})" },
            { "SeeInv_Fail_NoPlayer", "Failed to find a player with that name" },
            { "SeeInv_Fail_BadPage", "No such page." },
            { "SeeInv_Experimental", "This command is experimental, and can cause instability issues." },
            { "Experimental_Disabled", "This command is experimentl, and has been disabled in the config." },
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

        public string GetPlayerName(ulong player, string def)
        {
            if (TryGetPlayerName(player, out var m))
            {
                return m;
            }
            return def;
        }

        public bool TryGetPlayerName(ulong playerID, out string name)
        {
            lock (NamesCache)
            {
                if (NamesCache.ContainsKey(playerID))
                {
                    name = NamesCache[playerID];
                    return true;
                }

                int strl = playerID.ToString().Length;

                if (strl > 19 || strl < 16)
                {
                    name = null;
                    return false;
                }

                try
                {
                    Profile pr = new Profile(playerID);

                    NamesCache[playerID] = pr.SteamID;

                    name = pr.SteamID;
                    return true;
                }
                catch (Exception)
                {
                }
            }
            name = null;
            return false;
        }
    }
}