# ShimmysAdminTools
An admin toolkit for Unturned including flight and Noclip.

# Demo Video
Heres a very out-dated demo video. I'll get around to making a proper one eventually.

<a href="https://www.youtube.com/watch?v=2yo6BurDUA4">
<img src="https://i.ibb.co/L1RJdz3/Flight.jpg"/>
</a>

https://www.youtube.com/watch?v=2yo6BurDUA4

# Commands
<table>
    <tr>
        <th>Command</th>
        <th>Description</th>
    </tr>
    <tr>
        <td>AFly</td>
        <td>Toggles flight</td>
    </tr>
    <tr>
        <td>AFlySpeed [Speed]</td>
        <td>Changes/resets fly speed</td>
    </tr>
    <tr>
        <td>AFlyVSpeed [Speed]</td>
        <td>Changes/resets vertical fly speed</td>
    </tr>
    <tr>
        <td>Noclip</td>
        <td>Toggles Noclip</td>
    </tr>
    <tr>
        <td>CheckBedOwner</td>
        <td>Checks who has claimed the bed your looking at</td>
    </tr>
    <tr>
        <td>DisableVehicleAccess [Player]</td>
        <td>Disables/Enables a player's ability to enter and use vehicles</td>
    </tr>
    <tr>
        <td>ForceDismount [Player]</td>
        <td>Forcibly removes a player from their vehicle</td>
    </tr>
    <tr>
        <td>GotoBed [Name/ID]</td>
        <td>Teleports you to another player's bed</td>
    </tr>
    </tr>
    <tr>
        <td>Marker [Marker Name]</td>
        <td>Places a marker at your position</td>
    </tr>
    </tr>
    <tr>
        <td>GotoMarker [Marker Name]</td>
        <td>Teleports you to a previously set marker</td>
    </tr>
    </tr>
    <tr>
        <td>PointTool [Tool]</td>
        <td>Toggles/Changes your active PointTool (See Below)</td>
    </tr>
    <tr>
        <td>MapJump</td>
        <td>Toggles Map/Marker jumping (teleport on new marker).</td>
    </tr>
     <tr>
        <td>SalvageTime [Sec]</td>
        <td>Changes the amount of time required for you to salvage an object</td>
    </tr>
     <tr>
        <td>CommandSpy</td>
        <td>Toggles global command spying.</td>
    </tr>
     <tr>
        <td>CommandSpy [Players]</td>
        <td>Toggles command spying on a player.</td>
    </tr>
     <tr>
        <td>SetFireMode [Mode]</td>
        <td>Force changes the fire mode of your gun (e.g., full auto grizzly).</td>
    </tr>
     <tr>
        <td>UnlockFireModes</td>
        <td>Allows you to cycle through all fire modes using your change fire hotkey.</td>
    </tr>
     <tr>
        <td>Attachment [Attachment Name/ID]</td>
        <td>Forcibly adds an attachment to your weapon.</td>
    </tr>
     <tr>
        <td>Attachment [Magazine Name/ID]</td>
        <td>Forcibly gives your weapon any Magazine/Ammo Clip.</td>
    </tr>
     <tr>
        <td>UnlimitedAmmo</td>
        <td>Enables/Disables unlimited ammo.</td>
    </tr>
     <tr>
        <td>UnlimitedAmmo [Max Ammo override]</td>
        <td>Enables Unlimited Ammo, overriding your gun's max ammo.</td>
    </tr>
     <tr>
        <td>Exec [Player] [Command/Message]</td>
        <td>Similar to /sudo, however the target player does not need permission to run the command.</td>
    </tr>
    <tr>
        <td>ExecAll [Command/Message]</td>
        <td>Same as /Exec, but targets all players.</td>
    </tr>
    <tr>
        <td>WipePlayerBuildings [Player/PlayerID]</td>
        <td>Wipes all structures/barricades placed by the target player.</td>
    </tr>
    <tr>
        <td>WipePlayerBuildings [Player/PlayerID] [Structure/Barricade ID]</td>
        <td>Wipes all instances of the target structures/barricades placed by the target player.</td>
    </tr>
    <tr>
        <td>Ascend [Distance]</td>
        <td>Teleports you up the specified distance.</td>
    </tr>
    <tr>
        <td>Descend [Distance]</td>
        <td>Teleports you down the specified distance.</td>
    </tr>
    <tr>
        <td>PlaceObject [Barricade/Structure Name/ID]</td>
        <td>Creates and places the specified barricade/structure where your standing.</td>
    </tr>
    <tr>
        <td>ClearInventory</td>
        <td>Clears all items in your inventory</td>
    </tr>
</table>

## PointTools
PointTools is a small toolkit includes tools that are triggered by punching/gestures. These tools will target where/what you are activly looking at.

<table>
    <tr>
        <td>Tool</td>
        <td>Description</td>
    </tr>
    <tr>
        <td>Destroy</td>
        <td>Destroys what your currently looking at. Works on barricades, Structurs, and Vehicles.</td>
    </tr>
    <tr>
        <td>Kill</td>
        <td>Kills the Player/Animal/Zombie you are looking at. Turns Players to gold.</td>
    </tr>
    <tr>
        <td>Jump</td>
        <td>Teleports you to where your looking.<br>
            <ul>
                <li>Left Punch - Finds the highest point near where you punched. e.g. punch at a flag pole, get TPed ontop of it.</li>
                <li>Right Punch - Teleports you just inside of what your looking at, to allow you to clip through walls.</v>
                <li>Point - Teleports you directly where your looking, but not inside of the object.</li>
                </ul>
            You can also punch up into the sky to teleport to the skybox, then punch down anywhere on the map. Allowing you to teleport anywhere, quickly, and with high accuracy.
        </td>
    </tr>
    <tr>
        <td>Utility</td>
        <td>A kit of smaller tools<br>
            <ul>
                <li>Explodes charges</li>
                <li>Opens/Closes doors (Even if you don't own them)</li>
                <li>Checks the owner of a bed (Same as /CheckBedOwner)</li>
                <li>Opens storage (from any distance, even if you don't own them)</li>
            </ul>
        Tools are still to be added.
    </tr>
    <tr>
        <td>Repair</td>
        <td>Repairs Barricades, Structures, Vehicles, and Vehicle Tires.</td>
    </tr>
    <tr>
        <td>CheckOwner</td>
        <td>Shows you the owner of Structures, barricades, and Locked Vehicles. Along with the player's group.</td>
    </tr>
</table>

There are still more 'PointTools' to be added. This is not the final version.

# Permissions


<table>
    <tr>
        <td>Permission</td>
        <td>Description</td>
    </tr>
    <tr>
        <td>ShimmysAdminTools.Flight</td>
        <td>Provides access to all the flight commands, incl. /Afly, /AflySpeed, and /AflyVSpeed</td>
    </tr>
    <tr>
        <td>ShimmysAdminTools.Flight.MaxSpeed.X</td>
        <td>Sets the max fly speed, where X is the speed. Default is 1 (Walk/Run speed)</td>
    </tr>
    <tr>
        <td>ShimmysAdminTools.Flight.MaxSpeed.Bypass</td>
        <td>Disables fly speed restriction (global limit still applies)</td>
    </tr>
    <tr>
        <td>ShimmysAdminTools.Flight.MaxVSpeed.X</td>
        <td>Sets the max vertical fly speedd. Default is 1</td>
    </tr>
    <tr>
        <td>ShimmysAdminTools.Flight.MaxVSpeed.Bypass</td>
        <td>Disables vertical fly speed restriction (global limit still applies)</td>
    </tr>
    <tr>
        <td>ShimmysAdminTools.PointTool</td>
        <td>Provides access to /PointTool</td>
    </tr>
    <tr>
        <td>ShimmysAdminTools.PointTool.all</td>
        <td>Grants all PointTool perms</td>
    </tr>
    <tr>
        <td>ShimmysAdminTools.PointTool.Destroy</td>
        <td>Provides access to the Destroy tool</td>
    </tr>
    <tr>
        <td>ShimmysAdminTools.PointTool.Jump</td>
        <td>Provides access to the Jump tool</td>
    </tr>
    <tr>
        <td>ShimmysAdminTools.PointTool.Utility</td>
        <td>Provides access to the Utility tool</td>
    </tr>
    <tr>
        <td>ShimmysAdminTools.PointTool.Kill</td>
        <td>Provides access to the Kill tool</td>
    </tr>
    <tr>
        <td>ShimmysAdminTools.PointTool.CheckOwner</td>
        <td>Provides access to the CheckOwner tool</td>
    </tr>
    <tr>
        <td>ShimmysAdminTools.PointTool.Repair</td>
        <td>Provides access to the Repair tool</td>
    </tr>
    <tr>
        <td>ShimmysAdminTools.PointTool.DestroyOtherPlayersStuff</td>
        <td>Grants permission to destroy other player's vehicles/buildables with the Destoy Tool</td>
    </tr>
</table>
The rest of the perms are to be granted via their respective command.

## Screenshots

### Command Spy
Command spy displays the commands of other players and admins. It's a useful tool to keep an eye on commands without needing console.
An example of command spy in action:

<img src="https://cdn.discordapp.com/attachments/658867220314587162/735449754862878770/unknown.png" width="500px">

# Downloads
You can download this plugin from the <a href="https://github.com/ShimmyMySherbet/ShimmysAdminTools/releases/">Releases</a> page.

# Donate
I maintain and develop this product for free and for everyone to use. If your feeling generous and want to support my open source work, you can donate/sponsor the project on my Ko-Fi <a href="https://ko-fi.com/shimmymysherbet">Here</a>. Even $3 goes a long way.
