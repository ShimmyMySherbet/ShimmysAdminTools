using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Models;
using ShimmysAdminTools.Modules;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShimmysAdminTools.Components
{
    public static class Extensions
    {
        public static IEnumerable<R> CastEnumeration<T, R>(this IEnumerable<T> Inp, Func<T, R> Castingmethod)
        {
            List<R> Res = new List<R>();
            foreach (T InpE in Inp)
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
            if (AdminToolsPlugin.Instance.Translations.Instance[Translation] != null)
            {
                return AdminToolsPlugin.Instance.Translate(Translation, placeholder: Args);
            } else // Handle new translation key not in existing translations file
            {
                return AdminToolsPlugin.Instance.DefaultTranslations.Translate(Translation, placeholder: Args);
            }
        }

        public static T getOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
        {
            var obj = go.GetComponent<T>();

            if (obj != null)
            {
                return obj;
            }

            obj = go.AddComponent<T>();

            return obj;
        }

        public static void DestroyComponentIfExists<T>(this GameObject go) where T : UnityEngine.Component
        {
            var obj = go.GetComponent<T>();
            if (obj != null)
            {
                GameObject.Destroy(obj);
            }
        }


    }
}