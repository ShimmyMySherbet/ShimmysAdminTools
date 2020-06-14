using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Models;
using ShimmysAdminTools.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering;

namespace ShimmysAdminTools.Components
{
    public static class Extensions
    {
        public static IEnumerable<R> CastEnumeration<T, R>(this IEnumerable<T> Inp, Func<T, R> Castingmethod)
        {
            List<R> Res = new List<R>();
            foreach(T InpE in Inp)
            {
                Res.Add(Castingmethod(InpE));
            }
            return Res;
        }

        public static PlayerSession GetSession(this IRocketPlayer Player)
        {
            return ((UnturnedPlayer)Player).GetSession();
        }
        public static PlayerSession GetSession(this UnturnedPlayer Player)
        {
            return PlayerSessionStore.GetPlayerData(Player);
        }

        public static UnturnedPlayer UPlayer(this IRocketPlayer caller)
        {
            return (UnturnedPlayer)caller;
        }
        public static string Translate(this string Translation, params object[] Args)
        {
            return main.Instance.Translate(Translation, Args);
        }
    }
}
