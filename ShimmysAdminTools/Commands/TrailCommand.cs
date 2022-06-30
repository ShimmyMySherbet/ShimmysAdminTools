using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Behaviors;
using ShimmysAdminTools.Modules;
using UnityEngine;

namespace ShimmysAdminTools.Commands
{
    public class TrailCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "Trail";
        public string Help => "Manages a player's trail";
        public string Syntax => "Trail [None/Trail ID] {Player} {Rate}";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.Trail" };

        private Dictionary<string, ushort> TrailNames = new Dictionary<string, ushort>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "None", 0 },
            { "Fire", 139 },
            { "Water", 140 },
            { "WaterSheet", 140 },
            { "WaterFoam", 141 },
            { "WaterSpray", 142 },
            { "Waterfall", 146 },
        };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 1)
            {
                UnturnedChat.Say(caller, Syntax);
                return;
            }

            var trailHandle = command[0];
            ushort trailID;

            if (!ushort.TryParse(trailHandle, out trailID))
            {
                if (!TrailNames.TryGetValue(trailHandle, out trailID))
                {
                    UnturnedChat.Say(caller, $"Invalid Trail.\nOptions: {string.Join(", ", TrailNames.Keys)}");
                    return;
                }
            }

            UnturnedPlayer target;

            if (command.Length > 1)
            {
                target = PlayerUtil.GetPlayer(command[1]);
            }
            else if (caller is UnturnedPlayer pl)
            {
                target = pl;
            }
            else
            {
                UnturnedChat.Say(caller, "Console Usage: Trail [None/Trail ID] [Player]");
                return;
            }

            if (target == null)
            {
                UnturnedChat.Say(caller, "Failed to find player");
                return;
            }

            float rate = 0.05f;
            if (command.Length > 2)
            {
                if (!float.TryParse(command[2], out rate) || rate <= 0.01f)
                {
                    UnturnedChat.Say(caller, "Invalid effect rate");
                    return;
                }
            }

            var obj = target.Player.gameObject;

            if (trailID == 0)
            {
                var inst = obj.GetComponent<EffectTrailer>();
                if (inst == null)
                {
                    UnturnedChat.Say(caller, "Trail is not active.");
                    return;
                }

                inst.enabled = false;
                UnityEngine.Object.Destroy(inst);
            }
            else
            {
                var inst = obj.GetComponent<EffectTrailer>() ?? obj.AddComponent<EffectTrailer>();
                inst.Radius = EffectManager.MEDIUM;
                inst.EffectID = trailID;
                inst.Rate = rate;
                inst.enabled = true;
            }
        }
    }
}