using System.Collections.Generic;
using Rocket.Core;
using ShimmysAdminTools.Models;

namespace ShimmysAdminTools.Modules
{
    public static class ExecManager
    {
        public static List<ulong> EXECPlayers = new List<ulong>();
        public static bool IsActive = false;

        private static bool IsImplemented = false;

        public static void Activate()
        {
            if (!IsImplemented)
            {
                IsImplemented = true;
                R.Permissions = new EXECPassthroughPermissionsManager(R.Permissions);
            }
            IsActive = true;
        }

        public static void EnablePlayerEXEC(ulong Player)
        {
            lock (EXECPlayers)
            {
                if (!EXECPlayers.Contains(Player))
                {
                    EXECPlayers.Add(Player);
                }
                IsActive = true;
            }
        }

        public static void DisablePlayerEXEC(ulong Player)
        {
            lock (EXECPlayers)
            {
                EXECPlayers.RemoveAll(x => x == Player);
            }
        }

        public static bool PlayerIsEXEC(ulong Player)
        {
            lock (EXECPlayers)
            {
                return EXECPlayers.Contains(Player);
            }
        }

        public static void Deactivate()
        {
            lock (EXECPlayers)
            {
                IsActive = false;
                EXECPlayers.Clear();
            }
        }
    }
}