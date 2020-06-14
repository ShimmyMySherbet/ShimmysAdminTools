using ShimmysAdminTools.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShimmysAdminTools.Models
{
    public class PlayerSession
    {
        public ulong Player;
        public bool PointToolEnabled = false;
        public PointToolMode PointTool = PointToolMode.None;
        public FlySession FlySession;
        public bool FlySessionActive = false;
        public NoClippingTool NoClip;
        public bool NoClipSessionActive = false;
        public Dictionary<string, Vector3> Markers = new Dictionary<string, Vector3>();
    }
}
