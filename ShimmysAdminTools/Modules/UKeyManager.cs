using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Threading;

namespace ShimmysAdminTools.Modules
{
    /// <summary>
    /// Tool to watch for player key events. Remeber to call Stop() when your finished with it, or it will continue to run.
    /// </summary>
    public class UnturnedKeyWatcher
    {
        public UnturnedPlayer Player;

        /// <summary>
        /// How many ms between updates. Lower value means higher accuracy, but too low and it could cause issues.
        /// </summary>
        public int UpdateRate = 100;

        private bool IsRunning = false;
        private Dictionary<int, bool> LastMapping = new Dictionary<int, bool>();

        public delegate void UKeyDown(UnturnedPlayer Player, UnturnedKey Key);

        public event UKeyDown KeyChanged;

        public event UKeyDown KeyUp;

        public event UKeyDown KeyDown;

        public bool JumpKeyDown { get { return LastMapping[(int)UnturnedKey.Jump]; } }
        public bool SprintKeyDown { get { return LastMapping[(int)UnturnedKey.Sprint]; } }
        public bool LPunchKeyDown { get { return LastMapping[(int)UnturnedKey.LPunch]; } }
        public bool RPunchKeyDown { get { return LastMapping[(int)UnturnedKey.RPunch]; } }
        public bool ProneKeyDown { get { return LastMapping[(int)UnturnedKey.Prone]; } }
        public bool CrouchKeyDown { get { return LastMapping[(int)UnturnedKey.Crouch]; } }
        public bool LeanLeftKeyDown { get { return LastMapping[(int)UnturnedKey.Leanleft]; } }
        public bool LeanRightKeyDown { get { return LastMapping[(int)UnturnedKey.LeanRight]; } }
        public bool CodeHotkey1Down { get { return LastMapping[(int)UnturnedKey.CodeHotkey1]; } }
        public bool CodeHotkey2Down { get { return LastMapping[(int)UnturnedKey.CodeHotkey2]; } }
        public bool CodeHotkey3Down { get { return LastMapping[(int)UnturnedKey.CodeHotkey3]; } }
        public bool CodeHotkey4Down { get { return LastMapping[(int)UnturnedKey.CodeHotkey4]; } }

        public UnturnedKeyWatcher(UnturnedPlayer Player)
        {
            this.Player = Player;
            IsRunning = true;
            Thread RunThread = new Thread(UpdateLoop);
            RunThread.Start();
        }

        public void Stop()
        {
            IsRunning = false;
        }

        private void UpdateLoop()
        {
            while (IsRunning)
            {
                for (int i = 0; i < Player.Player.input.keys.Length - 1; i++)
                {
                    bool Current = Player.Player.input.keys[i];
                    if (LastMapping.ContainsKey(i))
                    {
                        bool Last = LastMapping[i];
                        if (Last != Current)
                        {
                            UnturnedKey Key = IntToUKey(i);
                            if (Key != UnturnedKey.Unknown)
                            {
                                Thread Changed = new Thread(x => KeyChanged?.Invoke(Player, Key));
                                Changed.Start();
                                if (Current)
                                {
                                    Thread KeyD = new Thread(x => KeyDown?.Invoke(Player, Key));
                                    KeyD.Start();
                                }
                                else
                                {
                                    Thread KeyU = new Thread(x => KeyUp?.Invoke(Player, Key));
                                    KeyU.Start();
                                }
                            }
                            LastMapping[i] = Current;
                        }
                    }
                    else
                    {
                        LastMapping[i] = Current;
                    }
                }
                Thread.Sleep(UpdateRate);
            }
        }

        private UnturnedKey IntToUKey(int i)
        {
            if (i == 0) return UnturnedKey.Jump;
            if (i == 1) return UnturnedKey.LPunch;
            if (i == 2) return UnturnedKey.RPunch;
            if (i == 3) return UnturnedKey.Crouch;
            if (i == 4) return UnturnedKey.Prone;
            if (i == 5) return UnturnedKey.Sprint;
            if (i == 6) return UnturnedKey.Leanleft;
            if (i == 7) return UnturnedKey.LeanRight;
            if (i == 9) return UnturnedKey.CodeHotkey1;
            if (i == 10) return UnturnedKey.CodeHotkey2;
            if (i == 11) return UnturnedKey.CodeHotkey3;
            if (i == 12) return UnturnedKey.CodeHotkey4;
            return UnturnedKey.Unknown;
        }
    }

    public enum UnturnedKey
    {
        Unknown = -1,
        Jump = 0,
        LPunch = 1,
        RPunch = 2,
        /// <summary>
        /// WARNING: Key status will be down while the player is crouched, even if they are not holding the key down.
        /// </summary>
        Crouch = 3,
        /// <summary>
        /// WARNING: Key status will be down while the player is prone, even if they are not holding the key down.
        /// </summary>
        Prone = 4,
        Sprint = 5,
        Leanleft = 6,
        LeanRight = 7,
        /// <summary>
        /// Defaults to comma
        /// </summary>
        CodeHotkey1 = 9,
        /// <summary>
        /// Defaults to period
        /// </summary>
        CodeHotkey2 = 10,
        /// <summary>
        /// Defaults to forward slash
        /// </summary>
        CodeHotkey3 = 11,
        /// <summary>
        /// Defaults to semicolon
        /// </summary>
        CodeHotkey4 = 12,
    }
}