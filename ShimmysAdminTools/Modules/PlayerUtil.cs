using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;

namespace ShimmysAdminTools.Modules
{
    public static class PlayerUtil
    {
        public static UnturnedPlayer GetPlayer(string handle)
        {
            if (ulong.TryParse(handle, out var id))
            {
                var pl = PlayerTool.getPlayer(new CSteamID(id));
                if (pl != null)
                {
                    return UnturnedPlayer.FromPlayer(pl);
                }
            }

            return UnturnedPlayer.FromName(handle);
        }
    }
}