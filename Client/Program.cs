using ServerCore;
using System.Net.Sockets;
using System.Net;

namespace Client
{
    class Program
    {
        public static PlayerList g_ClientPlayerList = new PlayerList();
        public static int g_UserGameMoney = 5000;
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();

            IPHostEntry ipHost = Dns.GetHostEntry(host);
            //IPAddress ipAddr = IPAddress.Parse("25.31.78.91");
            IPEndPoint endPoint = new IPEndPoint(ipHost.AddressList[0], Global.g_PortNumber);
            // 세션 생성한다음에 메시지 주고 받기 일단 주는것 부터
            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return SessionManager.Instance.Create(); });
            MonsterPurchasePacket monsterPurchase = new MonsterPurchasePacket();
            monsterPurchase.userGameMoney = g_UserGameMoney;
            monsterPurchase.monsterPrice = 500;

            while (true)
            {
                SessionManager.Instance.SendForEach();
                Thread.Sleep(10000);
            }
        }
    }
}
