using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class ServerSession : PacketSession
    {
        public override void OnConnect()
        {
            IsOnConnected = true;
          /*  Console.WriteLine("MonsterPurchase 패킷 보냄");
            MonsterPurchasePacket monsterPurchase = new MonsterPurchasePacket();
            monsterPurchase.userGameMoney = 5000;
            monsterPurchase.monsterPrice = 500;
            this.Send(monsterPurchase.Write());*/
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
