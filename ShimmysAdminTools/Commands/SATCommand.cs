using System.Collections.Generic;
using System.Text;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmysAdminTools.Components;

namespace ShimmysAdminTools.Commands
{
    public class SATCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "ShimmysAdminTools";

        public string Help => "Displays the version of ShimmysAdminTools";

        public string Syntax => "SAT [Update/Version/Info]";

        public List<string> Aliases => new List<string>() { "SAT" };

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                UnturnedChat.Say(caller, Syntax);
                return;
            }
            string param = command[0].ToLower();

            switch (param)
            {
                case "version":
                    UnturnedChat.Say(caller, $"This server is running ShimmysAdminTools v{UpdaterCore.CurrentVersion}. Latest Version: v{UpdaterCore.LatestVersion}");
                    break;

                case "info":
                    UnturnedChat.Say(caller, "ShimmysAdminTools is a free admin toolkit plugin developed by ShimmyMySherbet.");
                    break;

                case "update":

                    if (caller.HasPermission("ShimmysAdminTools.Update"))
                    {
                        if (!UpdaterCore.IsOutDated)
                        {
                            UnturnedChat.Say(caller, "ShimmysAdminTools is up to date!");
                            return;
                        }

                        bool hasMSG = UpdaterCore.TryGetUpdateMessage(out string msg);
                        if (caller is ConsolePlayer)
                        {
                            UnturnedChat.Say(caller, $"ShimmysAdminTools is out of date!");
                            UnturnedChat.Say(caller, $"Latest Version: v{UpdaterCore.LatestVersion}");
                            if (hasMSG)
                            {
                                UnturnedChat.Say(caller, "Update Notes:");
                                UnturnedChat.Say(caller, msg);
                            }
                            UnturnedChat.Say(caller, "Download the latest update at https://github.com/ShimmyMySherbet/ShimmysAdminTools/releases");
                        }
                        else if (caller is UnturnedPlayer pl)
                        {
                            StringBuilder b = new StringBuilder();
                            b.AppendLine($"ShimmysAdminTools is out of date!");
                            b.AppendLine($"Latest Version: v{UpdaterCore.LatestVersion}");
                            b.AppendLine($"Download the latest update from the GitHub page.");
                            if (hasMSG)
                            {
                                b.AppendLine("Update Notes:");
                                b.AppendLine(msg);
                            }
                            pl.Player.sendBrowserRequest(b.ToString(), "https://github.com/ShimmyMySherbet/ShimmysAdminTools/releases");
                        }
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "You do not have permission to use this command.");
                    }
                    break;
                default:
                    UnturnedChat.Say(caller, Syntax);
                    break;
            }
        }
    }
}