using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ServerContent;
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
                purchaseAllowed.PosX = purchasePacket.PosX;
                purchaseAllowed.PosY = purchasePacket.PosY;
                purchaseAllowed.PosZ = purchasePacket.PosZ;
            }
            else
            {
                purchaseAllowed.IsPurchase = false;
            }

            ClientSession clientSession = session as ClientSession;

            if (clientSession.GameRoom == null) return;

            Room room = clientSession.GameRoom;
            room.Push(() => clientSession.Send(purchaseAllowed.Write()));
        }

        public void PurchaseAllowedPacketHandler(Session session, IPacket packet)
        {
            Console.WriteLine("PurchaseAllowedPacket 작동");
        }

        public void MovePacketHandler(Session session, IPacket packet)
        {
            Console.WriteLine("Move Pacekt Handler 작동");
        }

        public void GameStartPacketHandler(Session session, IPacket packet)
        {
            GameStartPacket purchasePacket = packet as GameStartPacket;
            if (purchasePacket.IsGameStart)
            {
                Program.g_IsGameStart = true;
            }
        }

        public void C_MonsterCreatePacketHandler(Session session, IPacket packet)
        {
            C_MonsterCreatePacket monsterCreatePacket = packet as C_MonsterCreatePacket;
            Labo labo = new Labo();
            BaseMonster monster = new BaseMonster();
            labo.Init();
            monster.Init();
            monster.SetTarget(labo);
            monster.SetPosition(monsterCreatePacket.PosX, monsterCreatePacket.PosY, monsterCreatePacket.PosZ);
            monster.monsterId = Managers.Monster.Register(monster);

            S_BroadcastMonsterCreatePacket broadcastMonsterPacket = new S_BroadcastMonsterCreatePacket();
            broadcastMonsterPacket.monsterTeam = monsterCreatePacket.monsterTeam;
            broadcastMonsterPacket.monsterId = (ushort)monster.monsterId;
            broadcastMonsterPacket.PosX = monsterCreatePacket.PosX;
            broadcastMonsterPacket.PosY = monsterCreatePacket.PosY;
            broadcastMonsterPacket.PosZ = monsterCreatePacket.PosZ;
           
            ClientSession clientSession = session as ClientSession;

            if (clientSession.GameRoom == null) return;

            Room room = clientSession.GameRoom;
            room.BroadCast(broadcastMonsterPacket.Write());
        }
    }
}
