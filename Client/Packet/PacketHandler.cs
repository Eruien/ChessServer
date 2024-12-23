using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ServerCore;

namespace Client
{
    public class PacketHandler
    {
        static PacketHandler m_PacketHandler = new PacketHandler();
        public static PacketHandler Instance { get { return m_PacketHandler; } }

        //private GameObject skeletonPrefab;

        public void PurchaseAllowedPacket(Session session, IPacket packet)
        {
            PurchaseAllowedPacket purchaseAllowed = packet as PurchaseAllowedPacket;
            if (purchaseAllowed.IsPurchase)
            {
                Console.WriteLine("샀어요");
                //Debug.Log("샀어요");
                // Managers.Resource.Instantiate("Skeleton", new Vector3(0.0f, 1.0f, 0.0f));
            }
            else
            {
                Console.WriteLine("돈이 없어서 못사요");
                //Debug.Log("돈이 없어서 못사요");
            }
        }

        public void MovePacketHandler(Session session, IPacket packet)
        {
            Console.WriteLine("Move Pacekt Handler 작동");
        }

        public void PlayerListHandler(Session session, IPacket packet)
        {
            Console.WriteLine("클라이언트에 플레이어 목록 등록");
            //Program.g_ClientPlayerList = packet as PlayerList;
        }

        public void S_BroadcastEnterGameHandler(Session session, IPacket packet)
        {
            S_BroadcastEnterGame enterPlayer = packet as S_BroadcastEnterGame;
            /* Program.g_ClientPlayerList.Add(new PlayerList.Player
             {
                 PlayerId = enterPlayer.playerId,
                 PosX = enterPlayer.posX,
                 PosY = enterPlayer.posY,
                 PosZ = enterPlayer.posZ,
             });*/
        }
    }
}
