using Rocket.Unturned.Player;

namespace ShimmysAdminTools.Modules
{
    public static class PlayerSelector
    {
        public static bool GetPlayer(string handle, out UnturnedPlayer unturnedPlayer)
        {
            if (ulong.TryParse(handle, out ulong UL))
            {
                UnturnedPlayer pl = UnturnedPlayer.FromCSteamID(new Steamworks.CSteamID(UL));
                if (pl != null)
                {
                    unturnedPlayer = pl;
                    return true;
                }
            }
            unturnedPlayer = UnturnedPlayer.FromName(handle);
            if (unturnedPlayer != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}