using System;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Models;
using ShimmysAdminTools.Modules;

namespace ShimmysAdminTools
{
    public class main : RocketPlugin<PluginConfig>
    {
        public static main Instance;
        public static PluginConfig Config;
        public CheckPermissions BaseCheckPermissions;

        public override void LoadPlugin()
        {
            base.LoadPlugin();
            Instance = this;
            Config = Configuration.Instance;
            PlayerDataStore.Init();
            PlayerSessionStore.Init();
            U.Events.OnBeforePlayerConnected += Events_OnBeforePlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
            if (Config.EnableVehicleAccessManagement) VehicleManager.onEnterVehicleRequested += VehicleManager_onEnterVehicleRequested;
            Rocket.Unturned.Events.UnturnedPlayerEvents.OnPlayerUpdateGesture += UnturnedPlayerEvents_OnPlayerUpdateGesture;
            BaseCheckPermissions = ChatManager.onCheckPermissions;
            ChatManager.onCheckPermissions = Chat_OnCheckPermissions;
            LoadCurrentPlayers();
        }

        private void Chat_OnCheckPermissions(SteamPlayer player, string text, ref bool shouldExecuteCommand, ref bool shouldList)
        {
            PlayerData playerData = PlayerDataStore.GetPlayerData(player.playerID.steamID.m_SteamID);
            if (playerData != null)
            {
                if (playerData.IsMuted && !text.StartsWith(@"/"))
                {
                    if (PlayerMuteExpired(playerData))
                    {
                        playerData.IsMuted = false;
                        playerData.MuteIsTemp = false;
                        BaseCheckPermissions(player, text, ref shouldExecuteCommand, ref shouldList);
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
                                Console.WriteLine($"Expires: {playerData.MuteExpires.ToLongTimeString()}");
                                Console.WriteLine($"Current: {DateTime.Now.ToLongTimeString()}");
                                Console.WriteLine($"time: {TimeLeft.TotalSeconds}");
                                UnturnedChat.Say(player.playerID.steamID, "Mute_ChatBlocked_TimeLeft".Translate(Helpers.GetTimeFromTimespan(TimeLeft)));
                            }
                            else
                            {
                                UnturnedChat.Say(player.playerID.steamID, "Mute_ChatBlocked".Translate());
                            }
                        }
                    }
                }
                else
                {
                    BaseCheckPermissions(player, text, ref shouldExecuteCommand, ref shouldList);
                }
            }
            else
            {
                BaseCheckPermissions(player, text, ref shouldExecuteCommand, ref shouldList);
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
            { "Flight_Speed_Reset", "Fly Speed Reset." },
            { "Flight_Speed_NotFlying", "You are not flying." },
            { "Flight_Speed_Changed", "Fly speed set to {0}." },
            { "Flight_Speed_Denied", "You don't have permission to fly that fast." },
            { "Flight_Speed_Denied_Hotkey", "You can't fly any faster." },
            { "Flight_Speed_Vertical_Reset", "Vertical Fly Speed reset." },
            { "Flight_Speed_Vertical_Changed", "Vertical Fly Speed set to {0}." },
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
        };

        private void UnturnedPlayerEvents_OnPlayerUpdateGesture(UnturnedPlayer player, Rocket.Unturned.Events.UnturnedPlayerEvents.PlayerGesture gesture)
        {
            PointToolManager.ManageGestureUpdate(player, gesture);
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
            U.Events.OnBeforePlayerConnected -= Events_OnBeforePlayerConnected;
            U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnected;
            VehicleManager.onEnterVehicleRequested -= VehicleManager_onEnterVehicleRequested;
            Rocket.Unturned.Events.UnturnedPlayerEvents.OnPlayerUpdateGesture -= UnturnedPlayerEvents_OnPlayerUpdateGesture;
            foreach (var Session in PlayerSessionStore.Store)
            {
                if (Session.Value.FlySessionActive) Session.Value.FlySession.Stop();
                if (Session.Value.NoClipSessionActive) Session.Value.NoClip.Stop();
            }
            ChatManager.onCheckPermissions = BaseCheckPermissions;
            base.UnloadPlugin(state);
        }
    }
}