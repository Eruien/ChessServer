using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    public class PacketHandler
    {
        static PacketHandler m_PacketHandler = new PacketHandler();
        public static PacketHandler Instance { get { return m_PacketHandler; } }

        public void MonsterPurchasePacketHandler(Session session, IPacket packet)
        {
            Console.WriteLine("MonsterPurchasePacketHandler 작동");
            MonsterPurchasePacket purchasePacket = packet as MonsterPurchasePacket;
            PurchaseAllowedPacket purchaseAllowed = new PurchaseAllowedPacket();

            if (purchasePacket.userGameMoney >= purchasePacket.monsterPrice)
            {
                purchaseAllowed.IsPurchase = true;
            }
            else
            {
                purchaseAllowed.IsPurchase = false;
            }

            ClientSession clientSession = session as ClientSession;

            if (clientSession.GameRoom == null) return;

            Room room = clientSession.GameRoom;
            room.BroadCast(purchaseAllowed.Write());
        }

        public void PurchaseAllowedPacketHandler(Session session, IPacket packet)
        {
            Console.WriteLine("PurchaseAllowedPacket 작동");
        }

        public void MovePacketHandler(Session session, IPacket packet)
        {
            Console.WriteLine("Move Pacekt Handler 작동");
        }
    }
}
