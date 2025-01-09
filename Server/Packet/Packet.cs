using ServerCore;
using System;
using System.Collections.Generic;

namespace Server
{
    enum PacketType
    {
        None,
        LaboratoryPacket,
        C_GameStartPacket,
        S_SetInitialDataPacket,
        S_LabListPacket,
        C_MonsterPurchasePacket,
        S_PurchaseAllowedPacket,
        C_MonsterCreatePacket,
        S_BroadcastMonsterCreatePacket,
        S_BroadcastMonsterStatePacket,
        S_BroadcastMovePacket,
        C_AttackDistancePacket,
        C_HitPacket,
        S_BroadcastHitPacket,
        C_ChangeTargetPacket,
        S_BroadcastChangeTargetPacket,
    }

    public interface IPacket
    {
        public ushort PacketSize { get; }
        public ushort PacketID { get; }
        void Read(ArraySegment<byte> segment);
        ArraySegment<byte> Write();
    }

    public class PacketHeader
    {
        protected ushort m_PacketSize = 0;
        protected ushort m_PacketID = 0;
    }

    public class C_GameStartPacket : PacketHeader, IPacket
    {
        public bool m_IsGameStart = false;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(bool) * 1; } }
        public ushort PacketID { get { return m_PacketID; } }

        public C_GameStartPacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.C_GameStartPacket;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.m_IsGameStart = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
            count += sizeof(bool);
        }

        public ArraySegment<byte> Write()
        {
            int count = 0;
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            Array.Copy(BitConverter.GetBytes(this.m_PacketSize), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_PacketID), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_IsGameStart), 0, segment.Array, segment.Offset + count, sizeof(bool));
            count += sizeof(bool);

            return SendBufferHelper.Close(count);
        }
    }

    public class S_SetInitialDataPacket : PacketHeader, IPacket
    {
        public ushort m_MyTeam = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) * 1; } }
        public ushort PacketID { get { return m_PacketID; } }

        public S_SetInitialDataPacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.S_SetInitialDataPacket;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.m_MyTeam = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
        }

        public ArraySegment<byte> Write()
        {
            int count = 0;
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            Array.Copy(BitConverter.GetBytes(this.m_PacketSize), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_PacketID), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_MyTeam), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            return SendBufferHelper.Close(count);
        }
    }

    public class S_LabListPacket : PacketHeader, IPacket
    {
        ushort m_LabCount = 0;
        public List<LaboratoryPacket> m_LabList = new List<LaboratoryPacket>();

        public ushort PacketSize
        {
            get
            {
                return (ushort)(sizeof(ushort) * 2 + sizeof(ushort) + m_LabList.Count * LaboratoryPacket.PacketSize);
            }
        }
        public ushort PacketID { get { return m_PacketID; } }

        public S_LabListPacket()
        {
            m_PacketID = (ushort)PacketType.S_LabListPacket;
        }

        public void Add(LaboratoryPacket lab)
        {
            m_LabList.Add(lab);
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.m_LabCount = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);

            for (int i = 0; i < m_LabCount; i++)
            {
                LaboratoryPacket lab = new LaboratoryPacket();
                lab.Read(segment, ref count);
                m_LabList.Add(lab);
            }
        }

        public ArraySegment<byte> Write()
        {
            int count = 0;
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);

            Array.Copy(BitConverter.GetBytes(PacketSize), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_PacketID), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(m_LabList.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            for (int i = 0; i < m_LabList.Count; i++)
            {
                m_LabList[i].Write(segment, ref count);
            }

            return SendBufferHelper.Close(count);
        }

        public class LaboratoryPacket : PacketHeader
        {
            public ushort m_LabId = 0;
            public ushort m_Team = 0;
            public float m_PosX = 0;
            public float m_PosY = 0;
            public float m_PosZ = 0;

            static public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) * 2 + sizeof(float) * 3; } }
            public LaboratoryPacket()
            {
                m_PacketSize = PacketSize;
                m_PacketID = (ushort)PacketType.LaboratoryPacket;
            }

            public void Read(ArraySegment<byte> segment, ref int count)
            {
                count += sizeof(ushort);
                count += sizeof(ushort);
                this.m_LabId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
                count += sizeof(ushort);
                this.m_Team = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
                count += sizeof(ushort);
                this.m_PosX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
                count += sizeof(float);
                this.m_PosY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
                count += sizeof(float);
                this.m_PosZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
                count += sizeof(float);
            }

            public void Write(ArraySegment<byte> segment, ref int count)
            {
                Array.Copy(BitConverter.GetBytes(this.m_PacketSize), 0, segment.Array, segment.Offset + count, sizeof(ushort));
                count += sizeof(ushort);
                Array.Copy(BitConverter.GetBytes(this.m_PacketID), 0, segment.Array, segment.Offset + count, sizeof(ushort));
                count += sizeof(ushort);
                Array.Copy(BitConverter.GetBytes(this.m_LabId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
                count += sizeof(ushort);
                Array.Copy(BitConverter.GetBytes(this.m_Team), 0, segment.Array, segment.Offset + count, sizeof(ushort));
                count += sizeof(ushort);
                Array.Copy(BitConverter.GetBytes(this.m_PosX), 0, segment.Array, segment.Offset + count, sizeof(float));
                count += sizeof(float);
                Array.Copy(BitConverter.GetBytes(this.m_PosY), 0, segment.Array, segment.Offset + count, sizeof(float));
                count += sizeof(float);
                Array.Copy(BitConverter.GetBytes(this.m_PosZ), 0, segment.Array, segment.Offset + count, sizeof(float));
                count += sizeof(float);
            }
        }
    }

    public class C_MonsterPurchasePacket : PacketHeader, IPacket
    {
        public int m_UserGameMoney = 0;
        public int m_MonsterPrice = 0;
        public float m_PosX = 0;
        public float m_PosY = 0;
        public float m_PosZ = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(int) * 2 + sizeof(float) * 3; } }
        public ushort PacketID { get { return m_PacketID; } }

        public C_MonsterPurchasePacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.C_MonsterPurchasePacket;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.m_UserGameMoney = BitConverter.ToInt32(segment.Array, segment.Offset + count);
            count += sizeof(int);
            this.m_MonsterPrice = BitConverter.ToInt32(segment.Array, segment.Offset + count);
            count += sizeof(int);
            this.m_PosX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.m_PosY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.m_PosZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
        }

        public ArraySegment<byte> Write()
        {
            int count = 0;
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            Array.Copy(BitConverter.GetBytes(this.m_PacketSize), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_PacketID), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_UserGameMoney), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);
            Array.Copy(BitConverter.GetBytes(this.m_MonsterPrice), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);
            Array.Copy(BitConverter.GetBytes(this.m_PosX), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.m_PosY), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.m_PosZ), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);

            return SendBufferHelper.Close(count);
        }
    }

    public class S_PurchaseAllowedPacket : PacketHeader, IPacket
    {
        public bool m_IsPurchase = false;
        public float m_PosX = 0;
        public float m_PosY = 0;
        public float m_PosZ = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(bool) * 1 + sizeof(float) * 3; } }
        public ushort PacketID { get { return m_PacketID; } }

        public S_PurchaseAllowedPacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.S_PurchaseAllowedPacket;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.m_IsPurchase = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
            count += sizeof(bool);
            this.m_PosX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.m_PosY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.m_PosZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
        }

        public ArraySegment<byte> Write()
        {
            int count = 0;
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            Array.Copy(BitConverter.GetBytes(this.m_PacketSize), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_PacketID), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_IsPurchase), 0, segment.Array, segment.Offset + count, sizeof(bool));
            count += sizeof(bool);
            Array.Copy(BitConverter.GetBytes(this.m_PosX), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.m_PosY), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.m_PosZ), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);

            return SendBufferHelper.Close(count);
        }
    }

    public class C_MonsterCreatePacket : PacketHeader, IPacket
    {
        public float m_PosX = 0;
        public float m_PosY = 0;
        public float m_PosZ = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(float) * 3; } }
        public ushort PacketID { get { return m_PacketID; } }

        public C_MonsterCreatePacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.C_MonsterCreatePacket;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.m_PosX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.m_PosY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.m_PosZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
        }

        public ArraySegment<byte> Write()
        {
            int count = 0;
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            Array.Copy(BitConverter.GetBytes(this.m_PacketSize), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_PacketID), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_PosX), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.m_PosY), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.m_PosZ), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);

            return SendBufferHelper.Close(count);
        }
    }

    public class S_BroadcastMonsterCreatePacket : PacketHeader, IPacket
    {
        public ushort m_MonsterId = 0;
        public ushort m_MonsterTeam = 0;
        public ushort m_TargetLabId = 0;
        public float m_PosX = 0;
        public float m_PosY = 0;
        public float m_PosZ = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) * 3 + sizeof(float) * 3; } }
        public ushort PacketID { get { return m_PacketID; } }

        public S_BroadcastMonsterCreatePacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.S_BroadcastMonsterCreatePacket;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.m_MonsterId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.m_MonsterTeam = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.m_TargetLabId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.m_PosX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.m_PosY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.m_PosZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
        }

        public ArraySegment<byte> Write()
        {
            int count = 0;
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            Array.Copy(BitConverter.GetBytes(this.m_PacketSize), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_PacketID), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_MonsterId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_MonsterTeam), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_TargetLabId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_PosX), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.m_PosY), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.m_PosZ), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);

            return SendBufferHelper.Close(count);
        }
    }

    public class S_BroadcastMonsterStatePacket : PacketHeader, IPacket
    {
        public ushort m_MonsterId = 0;
        public ushort m_CurrentState = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) * 2; } }
        public ushort PacketID { get { return m_PacketID; } }
        public S_BroadcastMonsterStatePacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.S_BroadcastMonsterStatePacket;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.m_MonsterId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.m_CurrentState = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
        }

        public ArraySegment<byte> Write()
        {
            int count = 0;
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            Array.Copy(BitConverter.GetBytes(this.m_PacketSize), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_PacketID), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_MonsterId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_CurrentState), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            return SendBufferHelper.Close(count);
        }
    }

    public class S_BroadcastMovePacket : PacketHeader, IPacket
    {
        public ushort m_MonsterId = 0;
        public float m_PosX = 0;
        public float m_PosY = 0;
        public float m_PosZ = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) * 1 + sizeof(float) * 3; } }
        public ushort PacketID { get { return m_PacketID; } }
        public S_BroadcastMovePacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.S_BroadcastMovePacket;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.m_MonsterId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.m_PosX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.m_PosY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.m_PosZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
        }

        public ArraySegment<byte> Write()
        {
            int count = 0;
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            Array.Copy(BitConverter.GetBytes(this.m_PacketSize), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_PacketID), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_MonsterId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_PosX), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.m_PosY), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.m_PosZ), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);

            return SendBufferHelper.Close(count);
        }
    }

    public class C_AttackDistancePacket : PacketHeader, IPacket
    {
        public ushort m_MonsterId = 0;
        public float m_AttackDistance = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) * 1 + sizeof(float) * 1; } }
        public ushort PacketID { get { return m_PacketID; } }
        public C_AttackDistancePacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.C_AttackDistancePacket;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.m_MonsterId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.m_AttackDistance = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
        }

        public ArraySegment<byte> Write()
        {
            int count = 0;
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            Array.Copy(BitConverter.GetBytes(this.m_PacketSize), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_PacketID), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_MonsterId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_AttackDistance), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);

            return SendBufferHelper.Close(count);
        }
    }

    public class C_HitPacket : PacketHeader, IPacket
    {
        public ushort m_MonsterId = 0;
        public ushort m_TargetObjectId = 0;
        public ushort m_AttackType = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) * 3; } }
        public ushort PacketID { get { return m_PacketID; } }
        public C_HitPacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.C_HitPacket;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.m_MonsterId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.m_TargetObjectId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.m_AttackType = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
        }

        public ArraySegment<byte> Write()
        {
            int count = 0;
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            Array.Copy(BitConverter.GetBytes(this.m_PacketSize), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_PacketID), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_MonsterId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_TargetObjectId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_AttackType), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            return SendBufferHelper.Close(count);
        }
    }

    public class S_BroadcastHitPacket : PacketHeader, IPacket
    {
        public ushort m_ObjectId = 0;
        public ushort m_ObjectHP = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) * 2; } }
        public ushort PacketID { get { return m_PacketID; } }
        public S_BroadcastHitPacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.S_BroadcastHitPacket;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.m_ObjectId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.m_ObjectHP = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
        }

        public ArraySegment<byte> Write()
        {
            int count = 0;
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            Array.Copy(BitConverter.GetBytes(this.m_PacketSize), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_PacketID), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_ObjectId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_ObjectHP), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            return SendBufferHelper.Close(count);
        }
    }

    public class C_ChangeTargetPacket : PacketHeader, IPacket
    {
        public ushort m_ObjectId = 0;
        public ushort m_TargetObjectId = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) * 2; } }
        public ushort PacketID { get { return m_PacketID; } }
        public C_ChangeTargetPacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.C_ChangeTargetPacket;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.m_ObjectId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.m_TargetObjectId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
        }

        public ArraySegment<byte> Write()
        {
            int count = 0;
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            Array.Copy(BitConverter.GetBytes(this.m_PacketSize), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_PacketID), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_ObjectId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_TargetObjectId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            return SendBufferHelper.Close(count);
        }
    }

    public class S_BroadcastChangeTargetPacket : PacketHeader, IPacket
    {
        public ushort m_ObjectId = 0;
        public ushort m_TargetObjectId = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) * 2; } }
        public ushort PacketID { get { return m_PacketID; } }
        public S_BroadcastChangeTargetPacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.S_BroadcastChangeTargetPacket;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.m_ObjectId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.m_TargetObjectId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
        }

        public ArraySegment<byte> Write()
        {
            int count = 0;
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            Array.Copy(BitConverter.GetBytes(this.m_PacketSize), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_PacketID), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_ObjectId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.m_TargetObjectId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            return SendBufferHelper.Close(count);
        }
    }
}
