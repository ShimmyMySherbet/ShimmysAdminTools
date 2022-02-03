using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Modules;

namespace ShimmysAdminTools.Commands
{
    public class ExecCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "exec";

        public string Help => "Excecutes a command or message as another player, bypassing permissions.";

        public string Syntax => "exec [Player] [Command]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.exec" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (AdminToolsPlugin.Instance.ExecCommandRedirect != null)
            {
                AdminToolsPlugin.Instance.ExecCommandRedirect.Execute(caller, command);
                return;
            }

            if (!AdminToolsPlugin.Config.ExecEnabled)
            {
                UnturnedChat.Say("The exec module is disabled. Get the new exec plugin from github.com/ShimmyMySherbet/ExecPlugin");
                return;
            }

            if (command.Length >= 2)
            {
                string player = command[0];
                List<string> CommandParts = command.ToList();
                CommandParts.RemoveAt(0);
                string Command = string.Join(" ", CommandParts);

                UnturnedPlayer TargetPlayer = UnturnedPlayer.FromName(player);
                if (TargetPlayer == null)
                {
                    UnturnedChat.Say(caller, "Exec_Fail_NoPlayer".Translate());
                    return;
                }
                if (ExecManager.IsActive)
                {
                    ExecManager.EnablePlayerEXEC(TargetPlayer.CSteamID.m_SteamID);
                    try
                    {
                        ChatManager.instance.askChat(TargetPlayer.CSteamID, (byte)EChatMode.GLOBAL, Command);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    finally
                    {
                        ExecManager.DisablePlayerEXEC(TargetPlayer.CSteamID.m_SteamID);
                    }
                } else
                {
                    UnturnedChat.Say(caller, "Exec_Fail_NotActive".Translate());
                }
            } else
            {
                UnturnedChat.Say(caller, Syntax);
            }
        }
    }
}