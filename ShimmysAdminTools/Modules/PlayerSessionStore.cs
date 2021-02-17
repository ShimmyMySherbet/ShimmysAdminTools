using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimmysAdminTools.Modules
{
    // TODO: Migrate from player session store to mono behaviors 
    public static class PlayerSessionStore
    {
        public static Dictionary<ulong, PlayerSession> Store;
        public static void Init()
        {
            Store = new Dictionary<ulong, PlayerSession>();
        }
        public static void TryRegisterPlayer(UnturnedPlayer Player)
        {
            if (Store != null && !Store.ContainsKey(Player.CSteamID.m_SteamID)) Store.Add(Player.CSteamID.m_SteamID, new PlayerSession(Player));
        }
        public static void TryDeregisterPlayer(UnturnedPlayer Player)
        {
            if (Store != null && Store.ContainsKey(Player.CSteamID.m_SteamID))
            {
                PlayerSession session = GetPlayerData(Player);
                if (session.FlySessionActive) session.StopFlightSession();
                if (session.MapJumpingSession != null) session.StopMapJumpingSession();
                if (session.NoClipSessionActive) session.NoClip.Stop();
                session.MapJumpingSession = null;
                session.NoClipSessionActive = false;
                session.PointToolEnabled = false;
                Store.Remove(Player.CSteamID.m_SteamID);
            }
        }
        public static PlayerSession GetPlayerData(UnturnedPlayer Player)
        {
            if (Store == null) return null;
            if (Store.ContainsKey(Player.CSteamID.m_SteamID))
            {
                return Store[Player.CSteamID.m_SteamID];
            }
            else
            {
                return null;
            }
        }


        public static PlayerSession GetPlayerData(Player Player)
        {
            if (Store == null) return null;
            if (Store.ContainsKey(Player.channel.owner.playerID.steamID.m_SteamID))
            {
                return Store[Player.channel.owner.playerID.steamID.m_SteamID];
            }
            else
            {
                return null;
            }
        }

        public static bool RunPlayerCommandSpy()
        {
            foreach (var Session in Store)
            {
                if (Session.Value.IsSpyingCommands) return true;
            }
            return false;
        }

    }
}
