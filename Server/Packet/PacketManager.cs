using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ServerCore;
using static System.Collections.Specialized.BitVector32;

namespace Server
{
    public class PacketManager
    {
        static PacketManager m_PacketMgr = new PacketManager();
        public static PacketManager Instance { get { return m_PacketMgr; } }

        Dictionary<ushort, Func<ArraySegment<byte>, IPacket>> m_MakePacketDict = new Dictionary<ushort, Func<ArraySegment<byte>, IPacket>>();
        Dictionary<ushort, Action<Session, IPacket>> m_RunFunctionDict = new Dictionary<ushort, Action<Session, IPacket>>();

        public PacketManager()
        {
            Init();
        }

        void Init()
        {
            m_MakePacketDict.Add((ushort)PacketType.C_SetInitialLabo, MakePacket<C_SetInitialLaboPacket>);
            m_RunFunctionDict.Add((ushort)PacketType.C_SetInitialLabo, PacketHandler.Instance.C_SetInitialLaboPacketHandler);

            m_MakePacketDict.Add((ushort)PacketType.MonsterPurchasePacket, MakePacket<MonsterPurchasePacket>);
            m_RunFunctionDict.Add((ushort)PacketType.MonsterPurchasePacket, PacketHandler.Instance.MonsterPurchasePacketHandler);

            m_MakePacketDict.Add((ushort)PacketType.PurchaseAllowedPacket, MakePacket<PurchaseAllowedPacket>);
            m_RunFunctionDict.Add((ushort)PacketType.PurchaseAllowedPacket, PacketHandler.Instance.PurchaseAllowedPacketHandler);

            m_MakePacketDict.Add((ushort)PacketType.MovePacket, MakePacket<MovePacket>);
            m_RunFunctionDict.Add((ushort)PacketType.MovePacket, PacketHandler.Instance.MovePacketHandler);

            m_MakePacketDict.Add((ushort)PacketType.GameStart, MakePacket<GameStartPacket>);
            m_RunFunctionDict.Add((ushort)PacketType.GameStart, PacketHandler.Instance.GameStartPacketHandler);

            m_MakePacketDict.Add((ushort)PacketType.C_MonsterCreate, MakePacket<C_MonsterCreatePacket>);
            m_RunFunctionDict.Add((ushort)PacketType.C_MonsterCreate, PacketHandler.Instance.C_MonsterCreatePacketHandler);

            m_MakePacketDict.Add((ushort)PacketType.C_AttackDistance, MakePacket<C_AttackDistancePacket>);
            m_RunFunctionDict.Add((ushort)PacketType.C_AttackDistance, PacketHandler.Instance.C_AttackDistancePacketHandler);

            m_MakePacketDict.Add((ushort)PacketType.C_Hit, MakePacket<C_HitPacket>);
            m_RunFunctionDict.Add((ushort)PacketType.C_Hit, PacketHandler.Instance.C_HitPacketHandler);

            m_MakePacketDict.Add((ushort)PacketType.C_ChangeTarget, MakePacket<C_ChangeTargetPacket>);
            m_RunFunctionDict.Add((ushort)PacketType.C_ChangeTarget, PacketHandler.Instance.C_ChangeTargetPacketHandler);
        }

        public int OnRecvPacket(Session session, ArraySegment<byte> buffer)
        {
            // packet 번호에 따라 패킷 생성
            // 패킷 번호에 따라 맞는 함수 실행
            int count = 0;
            count += sizeof(ushort);
            ushort packetID = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);

            Func<ArraySegment<byte>, IPacket> func = null;

            
            if (m_MakePacketDict.TryGetValue(packetID, out func))
            {
                IPacket packet = func.Invoke(buffer);

                Action<Session, IPacket> action = null;

                if (m_RunFunctionDict.TryGetValue(packetID, out action))
                {
                    action.Invoke(session, packet);
                    return packet.PacketSize;
                }
            }

            return 0;
        }

        T MakePacket<T>(ArraySegment<byte> buffer) where T: IPacket, new()
        {
            T pkt = new T();
            pkt.Read(buffer);

            return pkt; 
        }
    }
}
