
namespace Server
{
    public interface IJobQueue
    {
        void Push(Action job);
    }
    public class JobQueue : IJobQueue
    {
        Queue<Action> m_JobQueue = new Queue<Action>();
        object m_Lock = new object();
        bool m_Flush = false;

        public void Push(Action action)
        {
            bool flush = false;

            lock (m_Lock)
            {
                m_JobQueue.Enqueue(action);

                if (m_Flush == false)
                    m_Flush = flush = true;
            }

            if (flush)
                Flush();
        }

        public Action Pop()
        {
            lock (m_Lock)
            {
                if (m_JobQueue.Count == 0)
                {
                    m_Flush = false;
                    return null;
                }
                return m_JobQueue.Dequeue();
            }
        }

        public void Flush()
        {
            while (true)
            {
                Action action = Pop();
                if (action == null) return;

                action.Invoke();
            }
        }
    }
}
