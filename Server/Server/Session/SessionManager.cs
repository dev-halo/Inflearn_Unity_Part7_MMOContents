using System;
using System.Collections.Generic;

namespace Server
{
    class SessionManager
    {
        static readonly SessionManager session = new();
        public static SessionManager Instance { get { return session; } }

        int sessionId = 0;
        readonly Dictionary<int, ClientSession> sessions = new();
        readonly object _lock = new();

        public ClientSession Generate()
        {
            lock (_lock)
            {
                int sessionId = ++this.sessionId;

                ClientSession session = new()
                {
                    SessionId = sessionId
                };
                sessions.Add(sessionId, session);

                Console.WriteLine($"Connected : {sessionId}");

                return session;
            }
        }

        public ClientSession Find(int id)
        {
            lock (_lock)
            {
                sessions.TryGetValue(id, out ClientSession session);
                return session;
            }
        }

        public void Remove(ClientSession session)
        {
            lock (_lock)
            {
                sessions.Remove(session.SessionId);
            }
        }
    }
}
