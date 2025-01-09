
namespace Server
{
    public class Room
    {
        List<ClientSession> m_SessionList = new List<ClientSession>();
        JobQueue m_JobQueue = new JobQueue();
        List<ArraySegment<byte>> m_PendingList = new List<ArraySegment<byte>>();

        // 서버쪽에도 등록, 클라쪽에도 등록

        public void Push(Action job)
        {
            m_JobQueue.Push(job);
        }

        public void Flush()
        {
            foreach (var session in m_SessionList)
            {
                session.Send(m_PendingList);
            }
            m_PendingList.Clear();
        }

        public void BroadCast(ArraySegment<byte> segment)
        {
            m_PendingList.Add(segment);
        }

        public void Enter(ClientSession session)
        {
            session.m_GameRoom = this;
            // 새로운 세션이 들어오면 기존애 있던 애들한테 알림
            m_SessionList.Add(session);
        }

        public void Leave()
        {

        }
    }
}
