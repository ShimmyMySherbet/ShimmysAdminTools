using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimmysAdminTools.Models
{
    public enum UnturnedKey : int
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
