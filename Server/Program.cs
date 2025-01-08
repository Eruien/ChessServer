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
            Labo redTeamLab = new Labo();
            Labo blueTeamLab = new Labo();
            redTeamLab.SelfTeam = Team.RedTeam;
            redTeamLab.Init();
            redTeamLab.SetPosition(0.0f, 2.904f, -22.0f);
            blueTeamLab.SelfTeam = Team.BlueTeam;
            blueTeamLab.Init();
            blueTeamLab.SetPosition(0.0f, 2.904f, 22.0f);

            int redNumber = Managers.Object.Register(redTeamLab);
            int blueNumber = Managers.Object.Register(blueTeamLab);
            Managers.Lab.Register(Team.RedTeam, redTeamLab, redNumber);
            Managers.Lab.Register(Team.BlueTeam, blueTeamLab, blueNumber);
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
