using Rocket.Unturned.Player;
using ShimmysAdminTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimmysAdminTools.Modules
{
    public static class PlayerDataStore
    {
        public static Dictionary<ulong, PlayerData> Store;
        public static void Init()
        {
            Store = new Dictionary<ulong, PlayerData>();
        }
        public static void TryRegisterPlayer(UnturnedPlayer Player)
        {
            if (Store != null && !Store.ContainsKey(Player.CSteamID.m_SteamID)) Store.Add(Player.CSteamID.m_SteamID, new PlayerData() { Player = Player.CSteamID.m_SteamID });
        }
        public static PlayerData GetPlayerData(UnturnedPlayer Player)
        {
            if (Player == null) return null;
            return GetPlayerData(Player.CSteamID.m_SteamID);
        }
        public static PlayerData GetPlayerData(ulong Player)
        {
            if (Store == null) return null;
            if (Store.ContainsKey(Player))
            {
                return Store[Player];
            }
            else
            {
                return null;
            }
        }
    }
}
