using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Behaviors;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Models;
using ShimmysAdminTools.Modules;

namespace ShimmysAdminTools.Commands
{
    public class PointToolCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "PointTool";

        public string Help => "Manages the point tool";

        public string Syntax => "PointTool [Mode]";

        public List<string> Aliases => new List<string>() { "pt" };

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.PointTool" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer Player = (UnturnedPlayer)caller;
            PlayerSession Session = PlayerSessionStore.GetPlayerData(Player);
            if (command.Length == 0)
            {
                Session.PointToolEnabled = !Session.PointToolEnabled;
                if (Session.PointToolEnabled)
                {
                    UnturnedChat.Say(caller, "PointTool_EnabledActive".Translate(Session.PointTool));
                    Player.Player.gameObject.AddComponent<PointToolInputListener>();
                }
                else
                {
                    UnturnedChat.Say(caller, "PointTool_Disabled".Translate());
                    Player.Player.gameObject.DestroyComponentIfExists<PointToolInputListener>();
                }
            }
            else
            {
                PointToolMode PrevTool = Session.PointTool;
                bool SendMsg = false;
                string Send = "";
                string arg = command[0].ToLower();
                if (arg == "d" || arg == "destroy")
                {
                    Session.PointTool = PointToolMode.Destroy;
                    SendMsg = true;
                    Send = "PointTool_Selected_Destroy".Translate();
                }
                else if (arg == "r" || arg == "repair")
                {
                    Session.PointTool = PointToolMode.Repair;
                    SendMsg = true;
                    Send = "PointTool_Selected_Repair".Translate();
                }
                else if (arg == "u" || arg == "utility")
                {
                    Session.PointTool = PointToolMode.Utility;
                    SendMsg = true;
                    Send = "PointTool_Selected_Utility".Translate();
                }
                else if (arg == "k" || arg == "kill")
                {
                    Session.PointTool = PointToolMode.Kill;
                    SendMsg = true;
                    Send = "PointTool_Selected_Kill".Translate();
                }
                else if (arg == "j" || arg == "jump")
                {
                    Session.PointTool = PointToolMode.Jump;
                    SendMsg = true;
                    Send = "PointTool_Selected_Jump".Translate();
                }
                else if (arg == "ck" || arg == "c" || arg == "checkowner")
                {
                    Session.PointTool = PointToolMode.CheckOwner;
                    SendMsg = true;
                    Send = "PointTool_Selected_CheckOwner".Translate();
                }
                if (PlayerCanUseMode(Player, Session.PointTool))
                {
                    if (SendMsg)
                    {
                        if (Session.PointToolEnabled)
                        {
                            UnturnedChat.Say(caller, Send);
                        }
                        else
                        {
                            Session.PointToolEnabled = true;
                            UnturnedChat.Say(caller, Send + "PointTool_Extension_Enabled".Translate());
                            Player.Player.gameObject.AddComponent<PointToolInputListener>();
                        }
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "Tools: Destroy, Utility, Jump, Kill, CheckOwner, Repair");
                    }
                }
                else
                {
                    Session.PointTool = PrevTool;
                    UnturnedChat.Say(caller, "PointTool_Denied".Translate());
                }
            }
        }

        public static bool PlayerCanUseMode(UnturnedPlayer Player, PointToolMode Mode)
        {
            if (Player.HasPermission("ShimmysAdminTools.PointTool.all") || Player.HasPermission("ShimmysAdminTools.PointTool.*"))
            {
                return true;
            }
            if (Mode == PointToolMode.Destroy) return Player.HasPermission("ShimmysAdminTools.PointTool.Destroy") && AdminToolsPlugin.Config.PointToolSettings.DestroyToolEnabled;
            if (Mode == PointToolMode.Jump) return Player.HasPermission("ShimmysAdminTools.PointTool.Jump") && AdminToolsPlugin.Config.PointToolSettings.JumpToolEnabled;
            if (Mode == PointToolMode.Utility) return Player.HasPermission("ShimmysAdminTools.PointTool.Utility") && AdminToolsPlugin.Config.PointToolSettings.UtilityToolEnabled;
            if (Mode == PointToolMode.Kill) return Player.HasPermission("ShimmysAdminTools.PointTool.Kill") && AdminToolsPlugin.Config.PointToolSettings.KillToolEnabled;
            if (Mode == PointToolMode.CheckOwner) return Player.HasPermission("ShimmysAdminTools.PointTool.CheckOwner") && AdminToolsPlugin.Config.PointToolSettings.KillToolEnabled;
            if (Mode == PointToolMode.Repair) return Player.HasPermission("ShimmysAdminTools.PointTool.Repair") && AdminToolsPlugin.Config.PointToolSettings.KillToolEnabled;
            if (Mode == PointToolMode.None) return true;
            return false;
        }
    }
}