using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    public class ClientSession : PacketSession
    {
        public ushort SessionId { get; set; }
        public Room GameRoom { get; set; }

        public override void OnConnect()
        {
            Program.g_GameRoom.Enter(this);
        }

        public override void OnDisconnect()
        {
            throw new NotImplementedException();
        }

        public override int OnRecvPacket(ArraySegment<byte> buffer)
        {
            return PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnSend(int numOfBytes)
        {
            throw new NotImplementedException();
        }
    }
}
