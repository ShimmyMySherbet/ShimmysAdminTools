using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimmysAdminTools.Models
{
    public class PlayerData
    {
        public ulong Player;
        public bool CanEnterVehicle = true;
        public bool IsMuted = false;
        public DateTime MuteExpires;
        public bool MuteIsTemp = false;
        public int FlightSpeedPermitOverride = 0;
    }
}
