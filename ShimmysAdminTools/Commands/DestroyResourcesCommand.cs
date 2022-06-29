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
    public class DestroyResourcesCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "DestroyResources";
        public string Help => "Destroys all resources in the area";
        public string Syntax => "DestroyResources [radius] [explode] [drop items]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.DestroyResources" };

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

            var explode = false;
            var dropItems = false;

            if (command.Length >= 2 && !bool.TryParse(command[1], out explode))
            {
                UnturnedChat.Say(caller, "Invalid value for explode. Values True/False");
                return;
            }

            if (command.Length >= 3 && !bool.TryParse(command[2], out dropItems))
            {
                UnturnedChat.Say(caller, "Invalid value for drop items. Values True/False");
                return;
            }

            var sqRange = range * range;

            var trees = new List<(float magitude, ResourceSpawnpoint spawn)>();

            for (int x = 0; x < LevelGround.trees.GetLength(0); x++)
            {
                for (int y = 0; y < LevelGround.trees.GetLength(1); y++)
                {
                    foreach (var tree in LevelGround.trees[x, y])
                    {
                        var magnitude = (tree.point - uPlayer.Position).sqrMagnitude;
                        if (magnitude < sqRange)
                        {
                            trees.Add((magnitude, tree));
                        }
                    }
                }
            }

            var queue = new Queue<ResourceSpawnpoint>();

            foreach (var tree in trees.OrderBy(x => x.magitude))
            {
                if (!tree.spawn.isDead)
                    queue.Enqueue(tree.spawn);
            }

            uPlayer.Player.StartCoroutine(DestroyResources(queue, explode, dropItems, 15));
        }

        private IEnumerator DestroyResources(Queue<ResourceSpawnpoint> trees, bool explode, bool drop, int treesPerUpdate)
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
                    ResourceManager.damage(tree.model, Vector3.up * 100f, 1000f, 10f, drop ? 1f : 0f, out _, out _);
                    if (explode)
                    {
                        EffectManager.sendEffect(20, 200, tree.point);
                    }
                }
                yield return null;
            }
        }
    }
}