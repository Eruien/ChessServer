using ServerCore;

namespace Client
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
            m_MakePacketDict.Add((ushort)PacketType.PurchaseAllowedPacket, MakePacket<PurchaseAllowedPacket>);
            m_RunFunctionDict.Add((ushort)PacketType.PurchaseAllowedPacket, PacketHandler.Instance.PurchaseAllowedPacket);

            m_MakePacketDict.Add((ushort)PacketType.MovePacket, MakePacket<MovePacket>);
            m_RunFunctionDict.Add((ushort)PacketType.MovePacket, PacketHandler.Instance.MovePacketHandler);

            m_MakePacketDict.Add((ushort)PacketType.PlayerList, MakePacket<PlayerList>);
            m_RunFunctionDict.Add((ushort)PacketType.PlayerList, PacketHandler.Instance.PlayerListHandler);

            m_MakePacketDict.Add((ushort)PacketType.S_BroadcastEnterGame, MakePacket<S_BroadcastEnterGame>);
            m_RunFunctionDict.Add((ushort)PacketType.S_BroadcastEnterGame, PacketHandler.Instance.S_BroadcastEnterGameHandler);
        }

        public int OnRecvPacket(Session session, ArraySegment<byte> buffer, Action<Session, IPacket> onRecvCallback = null)
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

                if (onRecvCallback != null)
                    onRecvCallback.Invoke(session, packet);
                else
                    HandlePacket(packetID, session, packet);

                return packet.PacketSize;
            }

            return 0;
        }

        T MakePacket<T>(ArraySegment<byte> buffer) where T : IPacket, new()
        {
            T pkt = new T();
            pkt.Read(buffer);

            return pkt;
        }

        public void HandlePacket(ushort packetID, Session session, IPacket packet)
        {
            Action<Session, IPacket> action = null;

            if (m_RunFunctionDict.TryGetValue(packetID, out action))
            {
                action.Invoke(session, packet);
            }
        }
    }
}
