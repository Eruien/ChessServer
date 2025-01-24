using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class Listener
    {
        Func<Session> m_SessionFactory;
        Socket m_Socket;
        SocketAsyncEventArgs m_ListenArgs = new SocketAsyncEventArgs();

        // 리스닝 소켓 바인딩 리스닝, 이벤트 등록
        public void Init(Func<Session> sessionFunc, int listenCount)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = IPAddress.Parse("192.168.0.5");
            IPEndPoint endPoint = new IPEndPoint(ipAddr, Global.g_PortNumber);
            m_Socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            m_Socket.Bind(endPoint);
            m_Socket.Listen(listenCount);

            m_ListenArgs.Completed += OnAcceptCompleted;
            m_SessionFactory += sessionFunc;
        }

        public void Start()
        {
            RgisterAccept();
        }

        void RgisterAccept()
        {
            m_ListenArgs.AcceptSocket = null;

            bool pending = m_Socket.AcceptAsync(m_ListenArgs);
            if (pending == false)
                OnAcceptCompleted(null, m_ListenArgs);
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                Console.WriteLine("클라이언트와 연결 성공");

                try
                {
                    Session session = m_SessionFactory.Invoke();
                    session.Init(args.AcceptSocket);
                    session.Start();
                    session.OnConnect();
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Listener -> OnAcceptCompleted : {e}");
                }
            }
            else
            {
                Console.WriteLine($"Listener -> OnAcceptCompleted : {args.SocketError}");
            }
            RgisterAccept();
        }
    }
}
