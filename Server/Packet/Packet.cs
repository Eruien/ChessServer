using ServerCore;
using System;
using System.Collections.Generic;

namespace Server
{
    enum PacketType
    {
        None,
        Laboratory,
        S_LabList,
        S_SetInitialData,
        MonsterPurchasePacket,
        PurchaseAllowedPacket,
        MovePacket,
        Player,
        PlayerList,
        S_BroadcastEnterGame,
        GameStart,
        C_MonsterCreate,
        S_BroadcastMonsterCreate,
        S_MonsterState,
        C_AttackDistance,
        C_Hit,
        S_Hit,
        C_ChangeTarget,
        S_ChangeTarget,
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

    public class S_SetInitialDataPacket : PacketHeader, IPacket
    {
        public ushort myTeam = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) * 1; } }
        public ushort PacketID { get { return m_PacketID; } }

        public S_SetInitialDataPacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.S_SetInitialData;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.myTeam = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
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
            Array.Copy(BitConverter.GetBytes(this.myTeam), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            return SendBufferHelper.Close(count);
        }
    }

    public class S_LabListPacket : PacketHeader, IPacket
    {
        ushort LabCount = 0;
        public List<Laboratory> LabList = new List<Laboratory>();

        public ushort PacketSize
        {
            get
            {
                return (ushort)(sizeof(ushort) * 2 + sizeof(ushort) + LabList.Count * Laboratory.PacketSize);
            }
        }
        public ushort PacketID { get { return m_PacketID; } }

        public S_LabListPacket()
        {
            m_PacketID = (ushort)PacketType.S_LabList;
        }

        public void Add(Laboratory lab)
        {
            LabList.Add(lab);
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.LabCount = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);

            for (int i = 0; i < LabCount; i++)
            {
                Laboratory lab = new Laboratory();
                lab.Read(segment, ref count);
                LabList.Add(lab);
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
            Array.Copy(BitConverter.GetBytes(LabList.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            for (int i = 0; i < LabList.Count; i++)
            {
                LabList[i].Write(segment, ref count);
            }

            return SendBufferHelper.Close(count);
        }

        public class Laboratory : PacketHeader
        {
            public ushort Id = 0;
            public ushort Team = 0;
            public float PosX = 0;
            public float PosY = 0;
            public float PosZ = 0;

            static public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) * 2 + sizeof(float) * 3; } }
            public Laboratory()
            {
                m_PacketSize = PacketSize;
                m_PacketID = (ushort)PacketType.Laboratory;
            }

            public void Read(ArraySegment<byte> segment, ref int count)
            {
                count += sizeof(ushort);
                count += sizeof(ushort);
                this.Id = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
                count += sizeof(ushort);
                this.Team = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
                count += sizeof(ushort);
                this.PosX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
                count += sizeof(float);
                this.PosY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
                count += sizeof(float);
                this.PosZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
                count += sizeof(float);
            }

            public void Write(ArraySegment<byte> segment, ref int count)
            {
                Array.Copy(BitConverter.GetBytes(this.m_PacketSize), 0, segment.Array, segment.Offset + count, sizeof(ushort));
                count += sizeof(ushort);
                Array.Copy(BitConverter.GetBytes(this.m_PacketID), 0, segment.Array, segment.Offset + count, sizeof(ushort));
                count += sizeof(ushort);
                Array.Copy(BitConverter.GetBytes(this.Id), 0, segment.Array, segment.Offset + count, sizeof(ushort));
                count += sizeof(ushort);
                Array.Copy(BitConverter.GetBytes(this.Team), 0, segment.Array, segment.Offset + count, sizeof(ushort));
                count += sizeof(ushort);
                Array.Copy(BitConverter.GetBytes(this.PosX), 0, segment.Array, segment.Offset + count, sizeof(float));
                count += sizeof(float);
                Array.Copy(BitConverter.GetBytes(this.PosY), 0, segment.Array, segment.Offset + count, sizeof(float));
                count += sizeof(float);
                Array.Copy(BitConverter.GetBytes(this.PosZ), 0, segment.Array, segment.Offset + count, sizeof(float));
                count += sizeof(float);
            }
        }
    }

    public class C_MonsterCreatePacket : PacketHeader, IPacket
    {
        public ushort monsterTeam = 0;
        public float PosX = 0;
        public float PosY = 0;
        public float PosZ = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) * 1 + sizeof(float) * 3; } }
        public ushort PacketID { get { return m_PacketID; } }

        public C_MonsterCreatePacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.C_MonsterCreate;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.monsterTeam = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.PosX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.PosY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.PosZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
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
            Array.Copy(BitConverter.GetBytes(this.monsterTeam), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.PosX), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.PosY), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.PosZ), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);

            return SendBufferHelper.Close(count);
        }
    }

    public class S_BroadcastMonsterCreatePacket : PacketHeader, IPacket
    {
        public ushort objectId = 0;
        public ushort monsterTeam = 0;
        public float PosX = 0;
        public float PosY = 0;
        public float PosZ = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) * 2 + sizeof(float) * 3; } }
        public ushort PacketID { get { return m_PacketID; } }

        public S_BroadcastMonsterCreatePacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.S_BroadcastMonsterCreate;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.objectId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.monsterTeam = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.PosX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.PosY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.PosZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
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
            Array.Copy(BitConverter.GetBytes(this.objectId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.monsterTeam), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.PosX), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.PosY), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.PosZ), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);

            return SendBufferHelper.Close(count);
        }
    }

    public class GameStartPacket : PacketHeader, IPacket
    {
        public bool IsGameStart = false;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(bool) * 1; } }
        public ushort PacketID { get { return m_PacketID; } }

        public GameStartPacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.GameStart;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.IsGameStart = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
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
            Array.Copy(BitConverter.GetBytes(this.IsGameStart), 0, segment.Array, segment.Offset + count, sizeof(bool));
            count += sizeof(bool);

            return SendBufferHelper.Close(count);
        }
    }

    public class MonsterPurchasePacket : PacketHeader, IPacket
    {
        public int userGameMoney = 0;
        public int monsterPrice = 0;
        public float PosX = 0;
        public float PosY = 0;
        public float PosZ = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(int) * 2 + sizeof(float) * 3; } }
        public ushort PacketID { get { return m_PacketID; } }

        public MonsterPurchasePacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.MonsterPurchasePacket;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.userGameMoney = BitConverter.ToInt32(segment.Array, segment.Offset + count);
            count += sizeof(int);
            this.monsterPrice = BitConverter.ToInt32(segment.Array, segment.Offset + count);
            count += sizeof(int);
            this.PosX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.PosY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.PosZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
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
            Array.Copy(BitConverter.GetBytes(this.userGameMoney), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);
            Array.Copy(BitConverter.GetBytes(this.monsterPrice), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);
            Array.Copy(BitConverter.GetBytes(this.PosX), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.PosY), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.PosZ), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);

            return SendBufferHelper.Close(count);
        }
    }

    public class PurchaseAllowedPacket : PacketHeader, IPacket
    {
        public bool IsPurchase = false;
        public float PosX = 0;
        public float PosY = 0;
        public float PosZ = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(bool) * 1 + sizeof(float) * 3; } }
        public ushort PacketID { get { return m_PacketID; } }

        public PurchaseAllowedPacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.PurchaseAllowedPacket;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.IsPurchase = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
            count += sizeof(bool);
            this.PosX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.PosY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.PosZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
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
            Array.Copy(BitConverter.GetBytes(this.IsPurchase), 0, segment.Array, segment.Offset + count, sizeof(bool));
            count += sizeof(bool);
            Array.Copy(BitConverter.GetBytes(this.PosX), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.PosY), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.PosZ), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);

            return SendBufferHelper.Close(count);
        }
    }

    public class MovePacket : PacketHeader, IPacket
    {
        public ushort objectId = 0;
        public float PosX = 0;
        public float PosY = 0;
        public float PosZ = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) * 1 + sizeof(float) * 3; } }
        public ushort PacketID { get { return m_PacketID; } }
        public MovePacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.MovePacket;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.objectId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.PosX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.PosY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.PosZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
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
            Array.Copy(BitConverter.GetBytes(this.objectId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.PosX), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.PosY), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.PosZ), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);

            return SendBufferHelper.Close(count);
        }
    }

    public class S_MonsterStatePacket : PacketHeader, IPacket
    {
        public ushort objectId = 0;
        public ushort currentState = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) * 2; } }
        public ushort PacketID { get { return m_PacketID; } }
        public S_MonsterStatePacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.S_MonsterState;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.objectId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.currentState = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
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
            Array.Copy(BitConverter.GetBytes(this.objectId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.currentState), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            return SendBufferHelper.Close(count);
        }
    }

    public class C_AttackDistancePacket : PacketHeader, IPacket
    {
        public ushort objectId = 0;
        public float attackDistance = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) * 1 + sizeof(float) * 1; } }
        public ushort PacketID { get { return m_PacketID; } }
        public C_AttackDistancePacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.C_AttackDistance;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.objectId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.attackDistance = BitConverter.ToSingle(segment.Array, segment.Offset + count);
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
            Array.Copy(BitConverter.GetBytes(this.objectId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.attackDistance), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);

            return SendBufferHelper.Close(count);
        }
    }

    public class C_HitPacket : PacketHeader, IPacket
    {
        public ushort objectId = 0;
        public ushort targetMonsterId = 0;
        public ushort attackType = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) * 3; } }
        public ushort PacketID { get { return m_PacketID; } }
        public C_HitPacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.C_Hit;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.objectId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.targetMonsterId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.attackType = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
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
            Array.Copy(BitConverter.GetBytes(this.objectId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.targetMonsterId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.attackType), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            return SendBufferHelper.Close(count);
        }
    }

    public class S_HitPacket : PacketHeader, IPacket
    {
        public ushort objectId = 0;
        public ushort objectHP = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) * 2; } }
        public ushort PacketID { get { return m_PacketID; } }
        public S_HitPacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.S_Hit;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.objectId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.objectHP = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
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
            Array.Copy(BitConverter.GetBytes(this.objectId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.objectHP), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            return SendBufferHelper.Close(count);
        }
    }

    public class C_ChangeTargetPacket : PacketHeader, IPacket
    {
        public ushort objectId = 0;
        public ushort targetObjectId = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) * 2; } }
        public ushort PacketID { get { return m_PacketID; } }
        public C_ChangeTargetPacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.C_ChangeTarget;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.objectId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.targetObjectId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
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
            Array.Copy(BitConverter.GetBytes(this.objectId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.targetObjectId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            return SendBufferHelper.Close(count);
        }
    }

    public class S_ChangeTargetPacket : PacketHeader, IPacket
    {
        public ushort objectId = 0;
        public ushort targetObjectId = 0;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) * 2; } }
        public ushort PacketID { get { return m_PacketID; } }
        public S_ChangeTargetPacket()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.S_ChangeTarget;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.objectId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.targetObjectId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
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
            Array.Copy(BitConverter.GetBytes(this.objectId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.targetObjectId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            return SendBufferHelper.Close(count);
        }
    }

    public class PlayerList : PacketHeader, IPacket
    {
        ushort PlayerCount = 0;

        public ushort PacketSize
        {
            get
            {
                return (ushort)(sizeof(ushort) * 2 + sizeof(ushort) + m_PlayerList.Count * Player.PacketSize);
            }
        }
        public ushort PacketID { get { return m_PacketID; } }

        public PlayerList()
        {
            m_PacketID = (ushort)PacketType.PlayerList;
        }

        public void Add(Player player)
        {
            m_PlayerList.Add(player);
        }

        List<Player> m_PlayerList = new List<Player>();

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.PlayerCount = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);

            for (int i = 0; i < PlayerCount; i++)
            {
                Player player = new Player();
                player.Read(segment, ref count);
                m_PlayerList.Add(player);
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
            Array.Copy(BitConverter.GetBytes(m_PlayerList.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            for (int i = 0; i < m_PlayerList.Count; i++)
            {
                m_PlayerList[i].Write(segment, ref count);
            }

            return SendBufferHelper.Close(count);
        }

        public class Player : PacketHeader
        {
            public ushort PlayerId = 0;
            public bool IsSelf = false;
            public float PosX = 0;
            public float PosY = 0;
            public float PosZ = 0;

            static public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) + sizeof(bool) + sizeof(float) * 3; } }
            public Player()
            {
                m_PacketSize = PacketSize;
                m_PacketID = (ushort)PacketType.Player;
            }

            public void Read(ArraySegment<byte> segment, ref int count)
            {
                count += sizeof(ushort);
                count += sizeof(ushort);
                this.PlayerId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
                count += sizeof(ushort);
                this.IsSelf = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
                count += sizeof(bool);
                this.PosX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
                count += sizeof(float);
                this.PosY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
                count += sizeof(float);
                this.PosZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
                count += sizeof(float);
            }

            public void Write(ArraySegment<byte> segment, ref int count)
            {
                Array.Copy(BitConverter.GetBytes(this.m_PacketSize), 0, segment.Array, segment.Offset + count, sizeof(ushort));
                count += sizeof(ushort);
                Array.Copy(BitConverter.GetBytes(this.m_PacketID), 0, segment.Array, segment.Offset + count, sizeof(ushort));
                count += sizeof(ushort);
                Array.Copy(BitConverter.GetBytes(this.PlayerId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
                count += sizeof(ushort);
                Array.Copy(BitConverter.GetBytes(this.IsSelf), 0, segment.Array, segment.Offset + count, sizeof(bool));
                count += sizeof(bool);
                Array.Copy(BitConverter.GetBytes(this.PosX), 0, segment.Array, segment.Offset + count, sizeof(float));
                count += sizeof(float);
                Array.Copy(BitConverter.GetBytes(this.PosY), 0, segment.Array, segment.Offset + count, sizeof(float));
                count += sizeof(float);
                Array.Copy(BitConverter.GetBytes(this.PosZ), 0, segment.Array, segment.Offset + count, sizeof(float));
                count += sizeof(float);
            }
        }
    }

    public class S_BroadcastEnterGame : PacketHeader, IPacket
    {
        public ushort playerId;
        public float posX;
        public float posY;
        public float posZ;

        public ushort PacketSize { get { return sizeof(ushort) * 2 + sizeof(ushort) + sizeof(float) * 3; } }
        public ushort PacketID { get { return m_PacketID; } }

        public S_BroadcastEnterGame()
        {
            m_PacketSize = PacketSize;
            m_PacketID = (ushort)PacketType.S_BroadcastEnterGame;
        }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.playerId = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
        }

        public ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            ushort count = 0;

            Array.Copy(BitConverter.GetBytes(this.m_PacketSize), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes((ushort)PacketType.S_BroadcastEnterGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);

            return SendBufferHelper.Close(count);
        }
    }
}
