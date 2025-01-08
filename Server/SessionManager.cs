
namespace Server
{
    class SessionManager
    {
        int m_SessionId = 0;
        static SessionManager m_SessionMgr = new SessionManager();
        public static SessionManager Instance { get { return m_SessionMgr; } }

        object m_Lock = new object();

        List<ClientSession> m_SessionList = new List<ClientSession>();
        
        public ClientSession Create()
        {
            lock (m_Lock)
            {
                ++m_SessionId;
                ClientSession session = new ClientSession();
                session.SessionId = (ushort)m_SessionId;
                m_SessionList.Add(session);
                return session;
            }
        }
    }
}
