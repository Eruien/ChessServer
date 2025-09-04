using ServerCore;
using ServerContent;

namespace Server
{
    class Program
    {
        public static bool g_RedTeamGameStart = false;
        public static bool g_BlueTeamGameStart = false;
        public static bool g_IsGameStart = false;
        public static Room g_GameRoom = new Room();

        static void FlushRoom()
        {
            g_GameRoom.Push(() => g_GameRoom.Flush());
            JobTimer.Instance.Push(FlushRoom, 250);
        }
    
        static void RegisterRab()
        {
            Lab redTeamLab = new Lab();
            Lab blueTeamLab = new Lab();
            redTeamLab.m_SelfTeam = Team.RedTeam;
            redTeamLab.Init();
            redTeamLab.SetPosition(1.11f, 1.0f, -17.5096f);
            blueTeamLab.m_SelfTeam = Team.BlueTeam;
            blueTeamLab.Init();
            blueTeamLab.SetPosition(1.11f, 1.0f, 17.5096f);

            int redNumber = Managers.Object.Register(redTeamLab);
            redTeamLab.m_ObjectId = redNumber;
            int blueNumber = Managers.Object.Register(blueTeamLab);
            blueTeamLab.m_ObjectId = blueNumber;
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
            RegisterRab();

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
