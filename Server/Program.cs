using ServerCore;
using ServerContent;

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
    
        static void RegisterRabo()
        {
            Labo redTeamLabo = new Labo();
            Labo blueTeamLabo = new Labo();
            redTeamLabo.SelfTeam = Team.RedTeam;
            redTeamLabo.Init();
            redTeamLabo.SetPosition(0.0f, 2.904f, -22.0f);
            blueTeamLabo.SelfTeam = Team.BlueTeam;
            blueTeamLabo.Init();
            blueTeamLabo.SetPosition(0.0f, 2.904f, 22.0f);

            int redNumber = Managers.Object.Register(redTeamLabo);
            int blueNumber = Managers.Object.Register(blueTeamLabo);
            Managers.Labo.Register("Red", redTeamLabo, redNumber);
            Managers.Labo.Register("Blue", blueTeamLabo, blueNumber);
        }

        static void Main(string[] args)
        {
            LTimer timer = new LTimer();
            Listener listener = new Listener();

            timer.Init();
            Managers.Init();
            listener.Init(SessionManager.Instance.Create, 100);
            listener.Start();

            JobTimer.Instance.Push(FlushRoom);
            RegisterRabo();

            while (true)
            {
                timer.Frame();
                if (g_IsGameStart)
                {
                    Managers.Object.Frame();
                }
            
                JobTimer.Instance.Flush();
            }
        }
    }
}
