using System;
using System.Collections;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using ShimmysAdminTools.Models;
using ShimmysAdminTools.Modules;
using UnityEngine;

namespace ShimmysAdminTools.Commands
{
    public class LaunchBushesCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "LaunchBushes";
        public string Help => "Launches all bushes on the map into the air";
        public string Syntax => "LaunchBushes {Target Player}";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.LaunchBushes" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var bushes = new List<Bush>();

            for (byte x = 0; x < LevelGround.trees.GetLength(0); x++)
            {
                for (byte y = 0; y < LevelGround.trees.GetLength(1); y++)
                {
                    for (ushort i = 0; i < LevelGround.trees[x, y].Count; i++)
                    {
                        // janky but fuck it
                        var bush = new Bush(x, y, i);
                        var name = bush.Spawn.asset.name;
                        if (!string.IsNullOrEmpty(name) && name.IndexOf("Bush", StringComparison.InvariantCultureIgnoreCase) != -1)
                        {
                            bushes.Add(bush);
                        }
                    }
                }
            }

            Player target = null;

            if (command.Length >= 1)
            {
                var upl = PlayerUtil.GetPlayer(command[0]);
                if (upl == null)
                {
                    UnturnedChat.Say(caller, "Failed to find player");
                    return;
                }
                target = upl.Player;
            }

            // Need a MonoBehaviour to start the coroutine through, took the lazy route
            VehicleManager.instance.StartCoroutine(LaunchBushes(bushes, target));
        }

        private IEnumerator LaunchBushes(List<Bush> bushes, Player targetPlayer = null)
        {
            Action<byte, byte, ushort> revive;
            Action<byte, byte, ushort> kill;

            var sendResourceAlive = typeof(ResourceManager).DynGet<ClientStaticMethod<byte, byte, ushort>>("SendResourceAlive");
            var sendResourceDead = typeof(ResourceManager).DynGet<ClientStaticMethod<byte, byte, ushort, Vector3>>("SendResourceDead");

            if (targetPlayer != null)
            {
                revive = (x, y, i) =>
                {
                    sendResourceAlive.Invoke(SDG.NetTransport.ENetReliability.Unreliable, targetPlayer.channel.GetOwnerTransportConnection(), x, y, i);
                };

                kill = (x, y, i) =>
                {
                    sendResourceDead.Invoke(SDG.NetTransport.ENetReliability.Unreliable, targetPlayer.channel.GetOwnerTransportConnection(), x, y, i,
                        Vector3.up * 100000);
                };
            }
            else
            {
                revive = (x, y, i) => ResourceManager.ServerSetResourceAlive(x, y, i);
                kill = (x, y, i) => ResourceManager.ServerSetResourceDead(x, y, i, Vector3.up * 100000);
            }

            foreach (var b in bushes)
            {
                if (b.Spawn.isDead || targetPlayer == null)
                {
                    revive(b.X, b.Y, b.I);
                }
            }
            yield return null;

            foreach (var b in bushes)
            {
                kill(b.X, b.Y, b.I);
            }

            yield return null;

            foreach (var b in bushes)
            {
                revive(b.X, b.Y, b.I);
            }
        }

        private class Bush
        {
            public ResourceSpawnpoint Spawn;
            public byte X;
            public byte Y;
            public ushort I;

            public Bush(byte x, byte y, ushort i)
            {
                X = x;
                Y = y;
                I = i;
                Spawn = LevelGround.trees[x, y][i];
            }
        }
    }
}