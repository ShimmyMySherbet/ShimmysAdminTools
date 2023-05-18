using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmysAdminTools.Modules;

namespace ShimmysAdminTools.Commands
{
	public class UnlockCommand : IRocketCommand
	{
		public AllowedCaller AllowedCaller => AllowedCaller.Player;
		public string Name => "Unlock";
		public string Help => "Unlocks the vehicle you are looking at";
		public string Syntax => Name;
		public List<string> Aliases { get; } = new List<string>();
		public List<string> Permissions { get; } = new List<string>() { "ShimmysAdminTools.Unlock" };

		public void Execute(IRocketPlayer caller, string[] command)
		{
			var player = caller as UnturnedPlayer;

			var raycast = RaycastUtility.RayCastPlayer(player, RayMasks.VEHICLE);

			if (raycast.Vehicle == null)
			{
				UnturnedChat.Say(caller, "You are not looking at a vehicle");
				return;
			}

			var vehicle = raycast.Vehicle;

			if (!vehicle.isLocked)
			{
				UnturnedChat.Say(caller, "Vehicle is not locked.");
				return;
			}

			VehicleManager.ServerSetVehicleLock(vehicle, vehicle.lockedOwner, vehicle.lockedGroup, false);
			UnturnedChat.Say(caller, "Vehicle unlocked");
		}
	}
}
