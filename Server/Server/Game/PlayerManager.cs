using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class PlayerManager
    {
        public static PlayerManager Instance { get; } = new PlayerManager();

        object _lock = new object();
        Dictionary<int, Player> players = new Dictionary<int, Player>();
        int playerId = 1;

        public Player Add()
        {
            Player player = new Player();

            lock (_lock)
            {
                player.Info.PlayerId = playerId;
                players.Add(playerId, player);
                playerId++;
            }

            return player;
        }

        public bool Remove(int playerId)
        {
            lock (_lock)
            {
                return players.Remove(playerId);
            }
        }

        public Player Find(int playerId)
        {
            lock (_lock)
            {
                if (players.TryGetValue(playerId, out Player player))
                    return player;
                return null;
            }
        }
    }
}
