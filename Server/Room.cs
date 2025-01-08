
namespace Server
{
    public class Room
    {
        List<ClientSession> SessionList = new List<ClientSession>();
        JobQueue JobQueue = new JobQueue();
        List<ArraySegment<byte>> PendingList = new List<ArraySegment<byte>>();

        // 서버쪽에도 등록, 클라쪽에도 등록

        public void Push(Action job)
        {
            JobQueue.Push(job);
        }

        public void Flush()
        {
            foreach (var session in SessionList)
            {
                session.Send(PendingList);
            }
            PendingList.Clear();
        }

        public void BroadCast(ArraySegment<byte> segment)
        {
            PendingList.Add(segment);
        }

        public void Enter(ClientSession session)
        {
            session.GameRoom = this;
            // 새로운 세션이 들어오면 기존애 있던 애들한테 알림
            SessionList.Add(session);
        }

        public void Leave()
        {

        }
    }
}
