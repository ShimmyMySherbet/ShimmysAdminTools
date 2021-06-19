using System.Collections.Generic;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Modules;
using ShimmysAdminTools.Behaviors;
using UnityEngine;
using Rocket.API.Extensions;
using ShimmysAdminTools.Components;

namespace ShimmysAdminTools.Models
{
    public class PlayerSession
    {
        public PlayerSession(UnturnedPlayer Player)
        {
            this.Player = Player.CSteamID.m_SteamID;
            UPlayer = Player;
        }

        public ulong Player;
        public UnturnedPlayer UPlayer;
        public bool PointToolEnabled = false;
        public PointToolMode PointTool = PointToolMode.None;

        public NoClippingTool NoClip;

        public bool NoClipSessionActive = false;
        public Dictionary<string, Vector3> Markers = new Dictionary<string, Vector3>();

        public GameObject MapJumpingSession = null;

        public bool FlySessionActive { get => FlySession != null; }

        public FlightSession FlySession = null;

        public bool CommandSpyGlobalEnabled = false;
        public List<ulong> CommandSpyPlayers = new List<ulong>();
        public bool IsSpyingCommands => CommandSpyGlobalEnabled || CommandSpyPlayers.Count != 0;

        public void StartMapJumpingSession()
        {
            if (MapJumpingSession == null)
            {
                MapJumpingSession = new GameObject("MapJumpingSession");
                UnityEngine.Object.DontDestroyOnLoad(MapJumpingSession);
                var c =MapJumpingSession.AddComponent<MapJumpingSession>();
                UnturnedPlayer pl = UnturnedPlayer.FromCSteamID(new Steamworks.CSteamID(Player));
                c.SetPlayer(pl);
            }
        }

        public void StartFlightSession()
        {
            if (!FlySessionActive)
            {
                FlightSession NewFly = UPlayer.Player.gameObject.AddComponent<FlightSession>();
                NewFly.SetReady(UPlayer);
                FlySession = NewFly;
            }
        }

        public void StopFlightSession()
        {

            if (FlySessionActive)
            {
                FlySession.Stop();
                UPlayer.Player.gameObject.DestroyComponentIfExists<FlightSession>();
                FlySession = null;
            }
        }

        public void StopMapJumpingSession()
        {
            if (MapJumpingSession != null)
            {
                UnityEngine.Object.Destroy(MapJumpingSession);
                MapJumpingSession = null;
            }
        }

        public void DisableCommandSpy()
        {
            CommandSpyGlobalEnabled = false;
            if (CommandSpyPlayers.Count != 0) CommandSpyPlayers.Clear();
        }
    }
}