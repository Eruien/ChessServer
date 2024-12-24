using ServerCore;
using System.Net.Sockets;
using System.Net;
using ServerContent;

namespace Server
{
    class Program
    {
        public static Room g_GameRoom = new Room();

        static void FlushRoom()
        {
            g_GameRoom.Push(() => g_GameRoom.Flush());
            JobTimer.Instance.Push(FlushRoom, 250);
        }
        static BaseMonster monsterSample = new BaseMonster();
        static void Main(string[] args)
        {
            Managers.Init();
            monsterSample.Init();
            Listener listener = new Listener();

            listener.Init(SessionManager.Instance.Create, 100);
            listener.Start();

            JobTimer.Instance.Push(FlushRoom);
            

            while (true)
            {
                JobTimer.Instance.Flush();
            }
        }
    }
}
