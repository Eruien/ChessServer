using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class Connector
    {
        Func<Session> m_SessionFactory;
      
        // 이벤트 등록 소켓 임시 생성 후 세션에게 넘겨줌
        public void Connect(EndPoint endPoint, Func<Session> SessionFunc)
        {
            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            m_SessionFactory += SessionFunc;

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnConnectCompleted;
            args.RemoteEndPoint = endPoint;
            args.UserToken = socket;
            
            RegisterConnect(args);
        }

        public void RegisterConnect(SocketAsyncEventArgs args)
        {
            Socket socket = args.UserToken as Socket;

            if (socket == null) return;
 
            bool pending = socket.ConnectAsync(args);
            if (pending == false)
                OnConnectCompleted(null, args);
        }

        public void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                Console.WriteLine("서버와 연결에 성공");
                Session m_Session = m_SessionFactory.Invoke();
                m_Session.Init(args.UserToken as Socket);
                m_Session.Start();
                m_Session.OnConnect();
            }
            else
            {
                Console.WriteLine($"Connector : {args.SocketError}");
            }
           
        }
    }
}
