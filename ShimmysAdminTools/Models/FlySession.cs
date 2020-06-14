using Rocket.Core.Utils;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Modules;
using System.Threading;

namespace ShimmysAdminTools.Models
{
    public class FlySession
    {
        public UnturnedKeyWatcher Watcher;
        public UnturnedPlayer Player;

        public float Gravity = 0;
        public float Speed = 1;

        public float AscendRate = 1;
        public float DescendRate = 1;

        private bool Active = false;

        public int BackUpdateDelay = 1000;

        public void Start()
        {
            Gravity = 0;
            Speed = 1;
            Watcher = new UnturnedKeyWatcher(Player);
            Watcher.KeyDown += Watcher_KeyDown;
            Watcher.KeyUp += Watcher_KeyUp;
            Active = true;
            UpdateGravity();
            UpdateSpeed();
            new Thread(BackUpdateTick).Start();
        }

        public void ResetSpeed()
        {
            Speed = 1;
            UpdateSpeed();
        }

        private void Watcher_KeyUp(UnturnedPlayer Player, UnturnedKey Key)
        {
            if (Key == UnturnedKey.Jump || Key == UnturnedKey.Sprint)
            {
                Gravity = 0;
                UpdateGravity();
            }
        }

        public void Stop()
        {
            Active = false;
            Gravity = 1;
            Speed = 1;
            Watcher.KeyDown -= Watcher_KeyDown;
            Watcher.Stop();
            UpdateSpeed();
            UpdateGravity();
        }

        private void Watcher_KeyDown(UnturnedPlayer Player, UnturnedKey Key)
        {
            if (Key == UnturnedKey.CodeHotkey1)
            {
                if (Helpers.PlayerCanFlyAtSpeed(Player, Speed - 1))
                {
                    Speed -= 1;
                    UpdateSpeed();
                }
                else
                {
                    TaskDispatcher.QueueOnMainThread(delegate () { UnturnedChat.Say(Player, "Flight_Speed_Denied_Hotkey".Translate()); });
                }
            }
            else if (Key == UnturnedKey.CodeHotkey2)
            {
                if (Helpers.PlayerCanFlyAtSpeed(Player, Speed + 1))
                {
                    Speed += 1;
                    UpdateSpeed();
                }
                else
                {
                    TaskDispatcher.QueueOnMainThread(delegate () { UnturnedChat.Say(Player, "Flight_Speed_Denied_Hotkey".Translate()); });
                }
            }
            else if (Key == UnturnedKey.Jump)
            {
                Gravity = AscendRate * -1;
                UpdateGravity();
            }
            else if (Key == UnturnedKey.Sprint)
            {
                if (Player.Player.look.pitch > 160)
                {
                    Gravity = DescendRate;
                    UpdateGravity();
                }
            }
        }

        private void BackUpdateTick()
        {
            while (Active)
            {
                Thread.Sleep(BackUpdateDelay);
                UpdateGravity();
                UpdateSpeed();
            }
        }

        public void UpdateGravity()
        {
            if (Player.Player.movement.pluginGravityMultiplier != Gravity)
            {
                TaskDispatcher.QueueOnMainThread(delegate { Player.Player.movement.sendPluginGravityMultiplier(Gravity); });
            }
        }

        public void UpdateSpeed()
        {
            if (Player.Player.movement.pluginSpeedMultiplier != Speed)
            {
                TaskDispatcher.QueueOnMainThread(delegate { Player.Player.movement.sendPluginSpeedMultiplier(Speed); });
            }
        }
    }
}