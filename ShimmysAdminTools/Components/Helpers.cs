using Rocket.API;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Models;
using ShimmysAdminTools.Modules;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;

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
            if (Player.IsAdmin || Player.HasPermission("shimmysadmintools.flight.maxspeed.bypass")) return Limit(99999, AdminToolsPlugin.Config.MaxGlobalFlySpeed);
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
            return Limit(MaxSpeed, AdminToolsPlugin.Config.MaxGlobalFlySpeed);
        }

        public static bool PlayerCanFlyAtSpeed(UnturnedPlayer Player, float Speed)
        {
            float MaxSpeed = GetPlayerMaxFlySpeed(Player);
            PlayerData data = PlayerDataStore.GetPlayerData(Player);
            if (data != null && data.FlightSpeedPermitOverride != 0) MaxSpeed = data.FlightSpeedPermitOverride;
            return Math.Abs(Speed) <= MaxSpeed;
        }

        public static float GetPlayerMaxVFlySpeed(UnturnedPlayer Player)
        {
            if (Player.IsAdmin || Player.HasPermission("shimmysadmintools.flight.maxvspeed.bypass")) return Limit(99999, AdminToolsPlugin.Config.MaxGlobalFlySpeed);
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
            return Limit(MaxSpeed, AdminToolsPlugin.Config.MaxGlobalFlySpeed);
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


        public static TimeSpan GetTimespanFromString(string dur)
        {
            double numst = GetNumericStart(dur);
            string tmod = GetTimeModifier(dur);
            return GetTimespanFromDetails(numst, tmod);
        }
        public static string GetTimeFromTimespan(TimeSpan ts)
        {
            if (ts.TotalDays >= 365)
            {
                double yrs = Math.Round(ts.TotalDays / 365, 1);
                return $"{yrs} year{Plurify(yrs)}";
            } else if (ts.TotalDays >= 7) {
                int weeks = (int)Math.Round(ts.TotalDays / 7);
                int days = ts.Days;
                return $"{weeks} week{Plurify(weeks)}{Switchstring($" {days} day{Plurify(days)}", days > 0)}";
            } else if (ts.TotalDays >= 1) {
                int days = ts.Days;
                int hours = ts.Hours;
                return $"{days} day{Plurify(days)}{Switchstring($" {hours} hour{Plurify(hours)}", hours > 0)}";
            } else if (ts.TotalHours >= 1)
            {
                int hours = ts.Hours;
                int minutes = ts.Minutes;
                return $"{hours} hour{Plurify(hours)}{Switchstring($" {minutes} minute{Plurify(minutes)}", minutes > 0)}";
            } else if (ts.TotalMinutes >= 1)
            {
                int minutes = ts.Minutes;
                int seconds = ts.Seconds;
                return $"{minutes} minute{Plurify(minutes)}{Switchstring($" {seconds} second{Plurify(seconds)}", seconds > 0 && minutes < 10)}";
            } else
            {
                int sec = ts.Seconds;
                return $"{sec} second{Plurify(sec)}";
            }
        }
        private static string Switchstring(string basestr, bool condition)
        {
            if (condition) return basestr;
            return "";
        }
        private static string Plurify(double ct)
        {
            if (ct != 1) return "s";
            return "";
        }

        //private static TimeModifier GetModifierFromSeconds(double sec)
        //{
        //    foreach(var modi in TimeModifiers)
        //    {
        //        if ((sec / (double)modi.Time) >= 1)
        //        {
        //            return modi;
        //        }
        //    }
        //    return TimeModifiers.Last();
        //}


        private static double GetNumericStart(string dur)
        {
            string Allowed = "1234567890";
            bool HitFloat = false;
            string retstr = "";

            foreach(char cha in dur)
            {
                if (Allowed.Contains(cha.ToString()))
                {
                    retstr += cha;
                } else if (cha == '.')
                {
                    if (!HitFloat)
                    {
                        retstr += cha;
                        HitFloat = true;
                    } else
                    {
                        break;
                    }
                } else
                {
                    break;
                }
            }
            if (string.IsNullOrEmpty(retstr))
            {
                return 0;
            } else
            {
                return Convert.ToDouble(retstr);
            }
        }


        private static string GetTimeModifier(string dur)
        {
            string NumericStart = GetNumericStart(dur).ToString();
            string Modifier = dur.Remove(0, NumericStart.Length).Trim(' ');
            return Modifier;
        }

        private static readonly List<TimeModifier> TimeModifiers = new List<TimeModifier>()
        {
            new TimeModifier() { DisplayName = "Year", DisplayNamePlural = "Years", Time = 60 * 60 * 24 * 365, Names = new List<string>() {"y", "year", "years", "yr", "yrs"} },
            new TimeModifier() { DisplayName = "Week", DisplayNamePlural = "Weeks", Time = 60 * 60 * 24 * 7, Names = new List<string>() {"w", "week", "weeks", "wk", "wks"} },
            new TimeModifier() { DisplayName = "Day", DisplayNamePlural = "Days", Time = 60 * 60 * 24, Names = new List<string>() {"d", "day", "days", "dy", "dys", "ds"} },
            new TimeModifier() { DisplayName = "Hour", DisplayNamePlural = "Hours", Time = 60 * 60, Names = new List<string>() {"h", "hour", "hours", "hs", "hr", "hrs"} },
            new TimeModifier() { DisplayName = "Minute", DisplayNamePlural = "Minutes", Time = 60, Names = new List<string>() {"m", "minute", "minutes", "ms"} },
            new TimeModifier() { DisplayName = "Second", DisplayNamePlural = "Seconds", Time = 1, Names = new List<string>() {"s", "seconds", "second", ""} }
        };

        private static TimeModifier GetTimeModifierObject(string modifier)
        {
            foreach(var mod in TimeModifiers)
            {
                if (mod.Names.Contains(modifier.ToLower().Trim(' ')))
                {
                    return mod;
                }
            }
            return TimeModifiers.Last();
        }
        public static TimeSpan GetTimespanFromDetails(double dur, string modifier)
        {
            return TimeSpan.FromSeconds(GetTimeModifierObject(modifier).GetTime(dur));
        }
        private class TimeModifier
        {
            public List<string> Names = new List<string>();
            public int Time = 0;
            public string DisplayName;
            public string DisplayNamePlural;
            public int GetTime(double dur)
            {
                return (int)(dur * Time);
            }
        }
    }
}