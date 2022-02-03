using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Components;
using ShimmysAdminTools.Modules;
using UnityEngine;

namespace ShimmysAdminTools.Commands
{
    public class ExecAllCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "execall";

        public string Help => "Same as exec, except it targets all players";

        public string Syntax => "execall [Command]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.execall" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (AdminToolsPlugin.Instance.ExecAllCommandRedirect != null)
            {
                AdminToolsPlugin.Instance.ExecAllCommandRedirect.Execute(caller, command);
                return;
            }

            if (!AdminToolsPlugin.Config.ExecEnabled)
            {
                UnturnedChat.Say("The exec module is disabled. Get the new exec plugin from github.com/ShimmyMySherbet/ExecPlugin");
                return;
            }

            if (command.Length >= 1)
            {
                string Command = string.Join(" ", command);
                foreach (SteamPlayer player in Provider.clients)
                {
                    try
                    {
                        ExecManager.EnablePlayerEXEC(player.playerID.steamID.m_SteamID);
                        ChatManager.instance.askChat(player.playerID.steamID, (byte)EChatMode.GLOBAL, Command);
                    }
                    finally
                    {
                        ExecManager.DisablePlayerEXEC(player.playerID.steamID.m_SteamID);
                    }
                }
                UnturnedChat.Say(caller, "Command excecuted through all players");

            } else
            {
                UnturnedChat.Say(caller, Syntax);
            }
        }
    }
}
