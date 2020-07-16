﻿using Rocket.Unturned.Player;
using ShimmysAdminTools.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShimmysAdminTools.Models
{
    public class PlayerSession
    {
        public ulong Player;
        public bool PointToolEnabled = false;
        public PointToolMode PointTool = PointToolMode.None;
        public FlySession FlySession;
        public bool FlySessionActive = false;
        public NoClippingTool NoClip;
        public bool NoClipSessionActive = false;
        public Dictionary<string, Vector3> Markers = new Dictionary<string, Vector3>();
        public GameObject MapJumpingSession = null;

        public void StartMapJumpingSession()
        {
            if (MapJumpingSession == null)
            {
                Console.WriteLine("Create Jump Session");
                MapJumpingSession = new GameObject("MapJumpingSession");
                UnityEngine.Object.DontDestroyOnLoad(MapJumpingSession);
                MapJumpingSession.AddComponent<MapJumpingSession>();
                UnturnedPlayer pl = UnturnedPlayer.FromCSteamID(new Steamworks.CSteamID(Player));
                Console.WriteLine($"create {pl.DisplayName}");
                GameObjectExtension.getOrAddComponent<MapJumpingSession>(MapJumpingSession).SetPlayer(pl);
                //MapJumpingSession = new GameObject("MapJumpingSession");
                //UnityEngine.Object.DontDestroyOnLoad(MapJumpingSession);
                //MapJumpingSession.AddComponent<MapJumpingSession>().SetPlayer(UnturnedPlayer.FromCSteamID(new Steamworks.CSteamID(Player)));
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


    }
}
