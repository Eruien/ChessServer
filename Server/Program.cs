using ServerCore;
using System.Net.Sockets;
using System.Net;
using ServerContent;
using System.Reflection;
using System.Diagnostics;

namespace Server
{
    class Program
    {
        public static bool g_IsGameStart = false;
        public static Room g_GameRoom = new Room();

        static void FlushRoom()
        {
            g_GameRoom.Push(() => g_GameRoom.Flush());
            JobTimer.Instance.Push(FlushRoom, 250);
        }

        
        static void Main(string[] args)
        {
            LTimer timer = new LTimer();
            timer.Init();
            Managers.Init();
            Listener listener = new Listener();

            listener.Init(SessionManager.Instance.Create, 100);
            listener.Start();

            JobTimer.Instance.Push(FlushRoom);
   
            while (true)
            {
                timer.Frame();
                if (g_IsGameStart)
                {
                    Managers.Monster.Frame();
                }
            
                JobTimer.Instance.Flush();
            }
        }
    }
}
