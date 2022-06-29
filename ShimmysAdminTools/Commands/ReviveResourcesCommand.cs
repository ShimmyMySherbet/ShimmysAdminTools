using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace ShimmysAdminTools.Commands
{
    public class ReviveResourcesCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "ReviveResources";
        public string Help => "Respawns all resources in the area";
        public string Syntax => "ReviveResources [radius] [effect]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.ReviveResources" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var uPlayer = caller as UnturnedPlayer;

            if (command.Length == 0)
            {
                UnturnedChat.Say(caller, Syntax);
                return;
            }

            if (!float.TryParse(command[0], out var range))
            {
                UnturnedChat.Say(caller, "Invalid Radius.");
                return;
            }

            ushort effect = 0;

            if (command.Length >= 2 && !ushort.TryParse(command[1], out effect))
            {
                UnturnedChat.Say(caller, "Invalid effect ID");
                return;
            }

            var sqRange = range * range;

            var trees = new List<(float magitude, ResourceSpawnpoint spawn, (int x, int y, int index))>();

            for (int x = 0; x < LevelGround.trees.GetLength(0); x++)
            {
                for (int y = 0; y < LevelGround.trees.GetLength(1); y++)
                {
                    for (int i = 0; i < LevelGround.trees[x, y].Count; i++)
                    {
                        var tree = LevelGround.trees[x, y][i];
                        var magnitude = (tree.point - uPlayer.Position).sqrMagnitude;
                        if (magnitude < sqRange)
                        {
                            trees.Add((magnitude, tree, (x, y, i)));
                        }
                    }
                }
            }

            var queue = new Queue<(byte x, byte y, ushort i, Vector3 pos)>();

            foreach (var tree in trees.OrderBy(x => x.magitude))
            {
                if (tree.spawn.isDead)
                    queue.Enqueue(((byte)tree.Item3.x, (byte)tree.Item3.y, (ushort)tree.Item3.index, tree.spawn.point));
            }

            uPlayer.Player.StartCoroutine(ReviveResource(queue, effect, 15));
        }

        private IEnumerator ReviveResource(Queue<(byte x, byte y, ushort i, Vector3 pos)> trees, ushort effect, int treesPerUpdate)
        {
            while (true)
            {
                for (int i = 0; i < treesPerUpdate; i++)
                {
                    if (trees.Count == 0)
                    {
                        yield break;
                    }

                    var tree = trees.Dequeue();
                    //System.Console.WriteLine($"Revive {tree.x} {tree.y}, {tree.i}");
                    var spawn = LevelGround.trees[tree.x, tree.y][tree.i];
                    //System.Console.WriteLine($"dead: {spawn.isDead}");
                    //spawn.revive();
                    ResourceManager.ServerSetResourceAlive(tree.x, tree.y, tree.i);
                    if (effect != 0)
                    {
                        EffectManager.sendEffect(effect, 200, tree.pos);
                    }
                }
                yield return null;
            }
        }
    }
}