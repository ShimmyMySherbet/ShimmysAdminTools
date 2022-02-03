using Rocket.API;
using ShimmysAdminTools.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ShimmysAdminTools.Models
{
    public class PluginConfig : IRocketPluginConfiguration
    {
        public int MaxGlobalFlySpeed;
        public bool DisableAbusableCommands;
        public bool DelayStartEXECUtility = false;
        public PointToolSettings PointToolSettings;
        public bool EnableExperimentalCommands = false;
        public bool ExecEnabled = false;

        [XmlArrayItem(elementName: "ID")]
        public List<ushort> BlacklistedAttachments;
        public void LoadDefaults()
        {
            DelayStartEXECUtility = false;
            MaxGlobalFlySpeed = 500;
            PointToolSettings = new PointToolSettings()
            {
                DestroyToolEnabled = true,
                JumpToolEnabled = true,
                KillToolEnabled = true,
                UtilityToolEnabled = true
            };
            DisableAbusableCommands = false;
            BlacklistedAttachments = new List<ushort>()
            {
                354,
                350,
                117,
                1002,
                1167,
                1338,
                1444,
                1394,
                1300
            };

        }
    }
    public class PointToolSettings
    {
        public bool DestroyToolEnabled;
        public bool JumpToolEnabled;
        public bool UtilityToolEnabled;
        public bool KillToolEnabled;
    }
}
