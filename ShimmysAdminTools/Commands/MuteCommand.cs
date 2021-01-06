//using System;
//using System.Collections.Generic;
//using Rocket.API;
//using Rocket.Unturned.Chat;
//using Rocket.Unturned.Player;
//using ShimmysAdminTools.Components;
//using ShimmysAdminTools.Models;
//using ShimmysAdminTools.Modules;









 /* 
  
  Command disabled because it conflicted with ChatMaster2
  
  
  */




//namespace ShimmysAdminTools.Commands
//{
//    public class MuteCommand : IRocketCommand
//    {
//        public AllowedCaller AllowedCaller => AllowedCaller.Both;

//        public string Name => "Mute";

//        public string Help => "Mutes a player from speaking in chat.";

//        public string Syntax => "Mute [Player] (Time)";

//        public List<string> Aliases => new List<string>() { "Unmute" };

//        public List<string> Permissions => new List<string>() { "ShimmysAdminTools.Mute" };

//        public void Execute(IRocketPlayer caller, string[] command)
//        {
//            if (command.Length > 0)
//            {
//                UnturnedPlayer Player = UnturnedPlayer.FromName(command[0]);
//                if (Player != null)
//                {
//                    PlayerData playerData = PlayerDataStore.GetPlayerData(Player);
//                    if (playerData != null)
//                    {
//                        playerData.IsMuted = !playerData.IsMuted;
//                        if (playerData.IsMuted)
//                        {
//                            if (command.Length > 1)
//                            {
//                                TimeSpan MuteTS = Helpers.GetTimespanFromString(command[1]);
//                                UnturnedChat.Say(caller, "Mute_PlayerMuted_Time".Translate(Helpers.GetTimeFromTimespan(MuteTS)));
//                                playerData.MuteExpires = DateTime.Now.Add(MuteTS);
//                                playerData.MuteIsTemp = true;
//                            }
//                            else
//                            {
//                                UnturnedChat.Say(caller, "Mute_PlayerMuted".Translate());
//                            }
//                        }
//                        else
//                        {
//                            playerData.MuteIsTemp = false;
//                            UnturnedChat.Say(caller, "Mute_PlayerUnmuted".Translate());
//                        }
//                    }
//                    else
//                    {
//                        UnturnedChat.Say(caller, "Plugin_Error".Translate());
//                    }
//                }
//                else
//                {
//                    UnturnedChat.Say(caller, "Mute_PlayerNotFound".Translate());
//                }
//            }
//            else
//            {
//                PlayerData playerData = PlayerDataStore.GetPlayerData((UnturnedPlayer)caller);
//                if (playerData != null && playerData.IsMuted)
//                {
//                    playerData.IsMuted = false;
//                    UnturnedChat.Say(caller, "Mute_PlayerUnmuted_Self".Translate());
//                }
//                else
//                {
//                    UnturnedChat.Say(caller, Syntax);
//                }
//            }
//        }
//    }
//}