using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{
    class SessionManager
    {
        static readonly SessionManager session = new();
        public static SessionManager Instance { get { return session; } }

        readonly List<ServerSession> sessions = new();
        readonly object _lock = new();
        Random rand = new Random();

        public void SendForEach()
        {
            lock (_lock)
            {
                foreach (ServerSession session in sessions)
                {
                    C_Move movePacket = new C_Move();
                    movePacket.posX = rand.Next(-50, 50);
                    movePacket.posY = 0;
                    movePacket.posZ = rand.Next(-50, 50);
                    session.Send(movePacket.Write());
                }
            }
        }

        public ServerSession Generate()
        {
            lock (_lock)
            {
                ServerSession session = new();
                sessions.Add(session);
                return session;
            }
        }
    }
}
