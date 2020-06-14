using Rocket.Unturned.Player;
using ShimmysAdminTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimmysAdminTools.Modules
{
    public static class PlayerSessionStore
    {
        public static Dictionary<ulong, PlayerSession> Store;
        public static void Init()
        {
            Store = new Dictionary<ulong, PlayerSession>();
        }
        public static void TryRegisterPlayer(UnturnedPlayer Player)
        {
            if (Store != null && !Store.ContainsKey(Player.CSteamID.m_SteamID)) Store.Add(Player.CSteamID.m_SteamID, new PlayerSession() { Player = Player.CSteamID.m_SteamID });
        }
        public static void TryDeregisterPlayer(UnturnedPlayer Player)
        {
            if (Store != null && Store.ContainsKey(Player.CSteamID.m_SteamID)) Store.Remove(Player.CSteamID.m_SteamID);
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

    }
}
