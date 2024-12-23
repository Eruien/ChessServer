using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class SessionManager
    {
        static SessionManager m_SessionMgr = new SessionManager();
        public static SessionManager Instance { get { return m_SessionMgr; } }

        object m_Lock = new object();

        List<ServerSession> m_SessionList = new List<ServerSession>();

        public void SendForEach()
        {
            lock (m_Lock)
            {
                foreach (ServerSession session in m_SessionList)
                {
                    MonsterPurchasePacket monsterPurchase = new MonsterPurchasePacket();
                    monsterPurchase.userGameMoney = 4000;
                    monsterPurchase.monsterPrice = 500;
                    session.Send(monsterPurchase.Write());
                }
            }
        }
        
        public ServerSession Create()
        {
            lock (m_Lock)
            {
                ServerSession session = new ServerSession();
                m_SessionList.Add(session);
                return session;
            }
        }
    }
}
