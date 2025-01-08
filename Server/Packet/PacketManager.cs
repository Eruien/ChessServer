using ServerCore;

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
            m_MakePacketDict.Add((ushort)PacketType.C_GameStartPacket, MakePacket<C_GameStartPacket>);
            m_RunFunctionDict.Add((ushort)PacketType.C_GameStartPacket, PacketHandler.Instance.C_GameStartPacketHandler);

            m_MakePacketDict.Add((ushort)PacketType.C_MonsterPurchasePacket, MakePacket<C_MonsterPurchasePacket>);
            m_RunFunctionDict.Add((ushort)PacketType.C_MonsterPurchasePacket, PacketHandler.Instance.C_MonsterPurchasePacketHandler);

            m_MakePacketDict.Add((ushort)PacketType.C_MonsterCreatePacket, MakePacket<C_MonsterCreatePacket>);
            m_RunFunctionDict.Add((ushort)PacketType.C_MonsterCreatePacket, PacketHandler.Instance.C_MonsterCreatePacketHandler);

            m_MakePacketDict.Add((ushort)PacketType.C_AttackDistancePacket, MakePacket<C_AttackDistancePacket>);
            m_RunFunctionDict.Add((ushort)PacketType.C_AttackDistancePacket, PacketHandler.Instance.C_AttackDistancePacketHandler);

            m_MakePacketDict.Add((ushort)PacketType.C_HitPacket, MakePacket<C_HitPacket>);
            m_RunFunctionDict.Add((ushort)PacketType.C_HitPacket, PacketHandler.Instance.C_HitPacketHandler);

            m_MakePacketDict.Add((ushort)PacketType.C_ChangeTargetPacket, MakePacket<C_ChangeTargetPacket>);
            m_RunFunctionDict.Add((ushort)PacketType.C_ChangeTargetPacket, PacketHandler.Instance.C_ChangeTargetPacketHandler);
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
