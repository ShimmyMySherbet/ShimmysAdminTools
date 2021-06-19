using System.Collections.Generic;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Models;
using UnityEngine;

namespace ShimmysAdminTools.Behaviors
{
    public class FlightSession : MonoBehaviour
    {
        private readonly Dictionary<int, bool> KeyIndex = new Dictionary<int, bool>();
        public bool awake = false;
        private bool Ready = false;

        public Player Player;
        public UnturnedPlayer UPlayer;

        public float VerticalSpeed = 1;
        public float Gravity = 0;
        public float Speed = 1;

        private bool IsDescending = false;

        public void SendUpdateSpeed() => NeedUpdateSpeed = true;

        private bool NeedUpdateSpeed = false;

        public void SendUpdateGravity() => NeedUpdateGravity = true;

        private bool NeedUpdateGravity = false;

        public void SetReady(UnturnedPlayer Player)
        {
            this.Player = Player.Player;
            UPlayer = Player;
            Ready = true;
            Player.Player.movement.sendPluginGravityMultiplier(Gravity);
            Player.Player.movement.sendPluginSpeedMultiplier(Speed);
        }

        public void Awake()
        {
            awake = true;
        }

        private void OnKeyStateChanged(UnturnedKey Key, bool State)
        {
            if (Key == UnturnedKey.Jump)
            {
                if (State)
                {
                    Gravity = VerticalSpeed * -1;
                    Player.movement.sendPluginGravityMultiplier(Gravity);
                }
                else
                {
                    Gravity = 0;
                    Player.movement.sendPluginGravityMultiplier(Gravity);
                }
            }
            else if (Key == UnturnedKey.Sprint)
            {
                if (State)
                {
                    if (Player.look.pitch > 160)
                    {
                        Gravity = VerticalSpeed;
                        IsDescending = true;
                        Player.movement.sendPluginGravityMultiplier(Gravity);
                    }
                }
                else
                {
                    if (IsDescending)
                    {
                        IsDescending = false;
                        Gravity = 0;
                        Player.movement.sendPluginGravityMultiplier(Gravity);
                    }
                }
            }
            else if (Key == UnturnedKey.CodeHotkey1)
            {
                if (State)
                {
                    if (Helpers.PlayerCanFlyAtSpeed(UPlayer, Speed - 1))
                    {
                        Speed -= 1;
                        Player.movement.sendPluginSpeedMultiplier(Speed);
                    }
                    else
                    {
                        UnturnedChat.Say(UPlayer, "Flight_Speed_Denied_Hotkey".Translate());
                    }
                }
            }
            else if (Key == UnturnedKey.CodeHotkey2)
            {
                if (State)
                {
                    if (Helpers.PlayerCanFlyAtSpeed(UPlayer, Speed + 1))
                    {
                        Speed += 1;
                        Player.movement.sendPluginSpeedMultiplier(Speed);
                    }
                    else
                    {
                        UnturnedChat.Say(UPlayer, "Flight_Speed_Denied_Hotkey".Translate());
                    }
                }
            }
            else if (Key == UnturnedKey.CodeHotkey3)
            {
                if (State)
                {
                    Player.movement.sendPluginSpeedMultiplier(Speed);
                    Player.movement.sendPluginGravityMultiplier(Gravity);
                }
            }
        }

        private void CheckState(UnturnedKey Key, bool[] Inputs)
        {
            bool State = Inputs[(int)Key];
            if (CheckChanged((int)Key, State))
            {
                OnKeyStateChanged(Key, State);
            }
        }

        private bool CheckChanged(int Index, bool State)
        {
            if (KeyIndex.ContainsKey(Index))
            {
                bool LastState = KeyIndex[Index];
                if (LastState != State)
                {
                    KeyIndex[Index] = State;
                    return true;
                }
            }
            else
            {
                KeyIndex.Add(Index, State);
            }
            return false;
        }

        public void FixedUpdate()
        {
            if (awake && Ready)
            {
                bool[] Inputs = Player.input.keys;
                if (Inputs.Length >= 12)
                {
                    CheckState(UnturnedKey.Jump, Inputs);
                    CheckState(UnturnedKey.Sprint, Inputs);
                    CheckState(UnturnedKey.CodeHotkey1, Inputs);
                    CheckState(UnturnedKey.CodeHotkey2, Inputs);
                    CheckState(UnturnedKey.CodeHotkey3, Inputs);
                }
                CheckNeeds();
            }
        }

        private void CheckNeeds()
        {
            if (NeedUpdateSpeed)
            {
                NeedUpdateSpeed = false;
                Player.movement.sendPluginSpeedMultiplier(Speed);
            }

            if (NeedUpdateGravity)
            {
                NeedUpdateGravity = false;
                Player.movement.sendPluginGravityMultiplier(Gravity);
            }
        }

        public void Stop()
        {
            awake = false;
            Gravity = AdminToolsPlugin.Instance.ServerGravityMult;
            Speed = AdminToolsPlugin.Instance.ServerSpeedMult;
            Player.movement.sendPluginGravityMultiplier(Gravity);
            Player.movement.sendPluginSpeedMultiplier(Speed);
        }

        public void OnDestroy()
        {
            Stop();
        }
    }
}