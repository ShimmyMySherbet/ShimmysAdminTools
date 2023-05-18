using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Modules;
using UnityEngine;

namespace ShimmysAdminTools.Commands
{
	public class WreckCommand : IRocketCommand
	{
		public AllowedCaller AllowedCaller => AllowedCaller.Player;
		public string Name => "wreck";
		public string Help => "Deletes objects you are looking at";
		public string Syntax => "Wreck";
		public List<string> Aliases { get; } = new List<string>() { "wr" };
		public List<string> Permissions { get; } = new List<string>() { "ShimmysAdminTools.Wreck" };

		public void Execute(IRocketPlayer caller, string[] command)
		{
			var player = caller as UnturnedPlayer;
			var result = RaycastUtility.RayCastPlayer(player, RayMasks.VEHICLE | RayMasks.BARRICADE | RayMasks.STRUCTURE);

			if (result.Vehicle != null)
			{
				if (!player.HasPermission("ShimmysAdminTools.Wreck.Vehicle"))
				{
					UnturnedChat.Say(caller, "You do not have permission to wreck vehicles.");
					return;
				}

				var vehicle = result.Vehicle;

				vehicle.forceRemoveAllPlayers();
				VehicleManager.askVehicleDestroy(result.Vehicle);
			}
			else if (result.Barricade != null)
			{
				if (!player.HasPermission("ShimmysAdminTools.Wreck.Barricade"))
				{
					UnturnedChat.Say(caller, "You do not have permission to wreck barricades.");
					return;
				}

				BarricadeManager.destroyBarricade(result.BarricadeRegion.drops[result.BarricadeIndex], result.BarricadeX, result.BarricadeY, result.BarricadePlant);
			}
			else if (result.Structure != null)
			{
				if (!player.HasPermission("ShimmysAdminTools.Wreck.Structure"))
				{
					UnturnedChat.Say(caller, "You do not have permission to wreck structures.");
					return;
				}
				StructureManager.destroyStructure(result.StructureRegion.drops[result.StructureIndex], result.StructureX, result.StructureY, Vector3.zero);
			}
			else
			{
				UnturnedChat.Say(caller, "Nothing found.");
			}
		}
	}
}
