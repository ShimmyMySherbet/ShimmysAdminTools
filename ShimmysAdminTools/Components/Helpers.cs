using Rocket.API;
using Rocket.Unturned.Player;
using System;

namespace ShimmysAdminTools.Components
{
    public static class Helpers
    {
        public static bool IsSteamID(string inp)
        {
            string Numeric = "0123456789";
            foreach (char cha in inp)
            {
                if (!Numeric.Contains(cha.ToString()))
                {
                    return false;
                }
            }
            return (inp.Length > 16 && inp.Length <= 18);
        }

        public static bool IsNumeric(string inp)
        {
            string Numeric = "0123456789.";
            bool DecimalHit = false;
            foreach (char cha in inp)
            {
                if (!Numeric.Contains(cha.ToString()))
                {
                    return false;
                }
                else
                {
                    if (cha == '.')
                    {
                        if (DecimalHit) return false;
                        DecimalHit = true;
                    }
                }
            }
            return true;
        }

        public static float GetPlayerMaxFlySpeed(UnturnedPlayer Player)
        {
            if (Player.IsAdmin || Player.HasPermission("shimmysadmintools.flight.maxspeed.bypass")) return Limit(99999, main.Config.MaxGlobalFlySpeed);
            float MaxSpeed = 1;
            string permbase = "shimmysadmintools.flight.maxspeed.";
            foreach (var Perm in Player.GetPermissions())
            {
                if (Perm.Name.ToLower().StartsWith(permbase))
                {
                    string sp = Perm.Name.Remove(0, permbase.Length);
                    if (IsNumeric(sp))
                    {
                        float speed = (float)Convert.ToDouble(Perm.Name.Remove(0, permbase.Length));
                        if (speed > MaxSpeed) MaxSpeed = speed;
                    }
                }
            }
            return Limit(MaxSpeed, main.Config.MaxGlobalFlySpeed);
        }

        public static bool PlayerCanFlyAtSpeed(UnturnedPlayer Player, float Speed)
        {
            float MaxSpeed = GetPlayerMaxFlySpeed(Player);
            return Math.Abs(Speed) <= MaxSpeed;
        }

        public static float GetPlayerMaxVFlySpeed(UnturnedPlayer Player)
        {
            if (Player.IsAdmin || Player.HasPermission("shimmysadmintools.flight.maxvspeed.bypass")) return Limit(99999, main.Config.MaxGlobalFlySpeed);
            float MaxSpeed = 1;
            string permbase = "shimmysadmintools.flight.maxvspeed.";
            foreach (var Perm in Player.GetPermissions())
            {
                if (Perm.Name.ToLower().StartsWith(permbase))
                {
                    string sp = Perm.Name.Remove(0, permbase.Length);
                    if (IsNumeric(sp))
                    {
                        float speed = (float)Convert.ToDouble(Perm.Name.Remove(0, permbase.Length));
                        if (speed > MaxSpeed) MaxSpeed = speed;
                    }
                }
            }
            return Limit(MaxSpeed, main.Config.MaxGlobalFlySpeed);
        }

        private static float Limit(float inp, float max)
        {
            if (inp > max) return max;
            return inp;
        }

        public static bool PlayerCanFlyAtVSpeed(UnturnedPlayer Player, float Speed)
        {
            float MaxSpeed = GetPlayerMaxVFlySpeed(Player);
            return Math.Abs(Speed) <= MaxSpeed;
        }
    }
}