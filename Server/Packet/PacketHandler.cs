using ServerContent;
using ServerCore;

namespace Server
{
    public class PacketHandler
    {
        static PacketHandler m_PacketHandler = new PacketHandler();
        public static PacketHandler Instance { get { return m_PacketHandler; } }

        public void C_GameStartPacketHandler(Session session, IPacket packet)
        {
            C_GameStartPacket gameStartPacket = packet as C_GameStartPacket;
            S_BroadcastGameStartPacket broadcastGameStartPacket = new S_BroadcastGameStartPacket();
            ClientSession clientSession = session as ClientSession;

            if (clientSession.m_GameRoom == null) return;

            if (clientSession.m_SessionTeam == Team.RedTeam)
            {
                Program.g_RedTeamGameStart = true;
            }
            else if (clientSession.m_SessionTeam == Team.BlueTeam)
            {
                Program.g_BlueTeamGameStart = true;
            }

            if (Program.g_RedTeamGameStart && Program.g_BlueTeamGameStart)
            {
                broadcastGameStartPacket.m_IsGameStart = true;
                Program.g_IsGameStart = true;
                BaseMonster.m_SearchNearTarget.Invoke();
            }

            Room room = clientSession.m_GameRoom;
            room.BroadCast(broadcastGameStartPacket.Write());
        }

        public void C_MonsterPurchasePacketHandler(Session session, IPacket packet)
        {
            C_MonsterPurchasePacket purchasePacket = packet as C_MonsterPurchasePacket;
            S_PurchaseAllowedPacket purchaseAllowedPacket = new S_PurchaseAllowedPacket();
            ClientSession clientSession = session as ClientSession;

            if (clientSession.m_GameRoom == null) return;

            purchaseAllowedPacket.m_StringSize = purchasePacket.m_StringSize;
            purchaseAllowedPacket.m_MonsterType = purchasePacket.m_MonsterType;

            if (purchasePacket.m_UserGameMoney >= purchasePacket.m_MonsterPrice)
            {
                purchasePacket.m_UserGameMoney -= purchasePacket.m_MonsterPrice;
                purchaseAllowedPacket.m_IsPurchase = true;
                purchaseAllowedPacket.m_PosX = purchasePacket.m_PosX;
                purchaseAllowedPacket.m_PosY = purchasePacket.m_PosY;
                purchaseAllowedPacket.m_PosZ = purchasePacket.m_PosZ;
            }
            else
            {
                purchaseAllowedPacket.m_IsPurchase = false;
            }

            purchaseAllowedPacket.m_UserGameMoney = purchasePacket.m_UserGameMoney;
            Room room = clientSession.m_GameRoom;
            room.Push(() => clientSession.Send(purchaseAllowedPacket.Write()));
        }

        public void C_MonsterCreatePacketHandler(Session session, IPacket packet)
        {
            C_MonsterCreatePacket monsterCreatePacket = packet as C_MonsterCreatePacket;
            ClientSession clientSession = session as ClientSession;
            Team otherTeam = Team.None;
            
            if (clientSession.m_GameRoom == null) return;

            if (clientSession.m_SessionTeam == Team.RedTeam)
            {
                otherTeam = Team.BlueTeam;
            }
            else if (clientSession.m_SessionTeam == Team.BlueTeam)
            {
                otherTeam = Team.RedTeam;
            }
           
            BaseMonster monster = new BaseMonster(Managers.Lab.GetTeamLab(otherTeam));
            monster.m_Name = monsterCreatePacket.m_MonsterType;
            monster.Init();
            monster.SetPosition(monsterCreatePacket.m_PosX, monsterCreatePacket.m_PosY, monsterCreatePacket.m_PosZ);
            monster.m_ObjectId = Managers.Object.Register(monster);
            monster.m_SelfTeam = clientSession.m_SessionTeam;
           
            S_BroadcastMonsterCreatePacket broadcastMonsterPacket = new S_BroadcastMonsterCreatePacket();
            broadcastMonsterPacket.m_StringSize = monsterCreatePacket.m_StringSize;
            broadcastMonsterPacket.m_MonsterType = monsterCreatePacket.m_MonsterType;
            broadcastMonsterPacket.m_MonsterTeam = (ushort)clientSession.m_SessionTeam;
            broadcastMonsterPacket.m_TargetLabId = (ushort)Managers.Lab.GetLabNumber(monster.m_TargetLab);
            broadcastMonsterPacket.m_MonsterId = (ushort)monster.m_ObjectId;
            broadcastMonsterPacket.m_PosX = monsterCreatePacket.m_PosX;
            broadcastMonsterPacket.m_PosY = monsterCreatePacket.m_PosY;
            broadcastMonsterPacket.m_PosZ = monsterCreatePacket.m_PosZ;
           
            Room room = clientSession.m_GameRoom;
            room.BroadCast(broadcastMonsterPacket.Write());
        }

        public void C_SetPositionPacketHandler(Session session, IPacket packet)
        {
            C_SetPositionPacket positionPacket = packet as C_SetPositionPacket;
            S_BroadcastSetPositionPacket broadcastPositionPacket = new S_BroadcastSetPositionPacket();
            ClientSession clientSession = session as ClientSession;

            if (clientSession.m_GameRoom == null) return;

            BaseObject obj = Managers.Object.GetObject(positionPacket.m_MonsterId);
            obj.SetPosition(positionPacket.m_PosX, positionPacket.m_PosY, positionPacket.m_PosZ);

            broadcastPositionPacket.m_MonsterId = (ushort)obj.m_ObjectId;
            broadcastPositionPacket.m_PosX = obj.m_Position.X;
            broadcastPositionPacket.m_PosY = obj.m_Position.Y;
            broadcastPositionPacket.m_PosZ = obj.m_Position.Z;

            Room room = clientSession.m_GameRoom;
            room.BroadCast(broadcastPositionPacket.Write());
        }

        public void C_AttackDistancePacketHandler(Session session, IPacket packet)
        {
            C_AttackDistancePacket attackDistancePacket = packet as C_AttackDistancePacket;

            BaseObject monster = Managers.Object.GetObject(attackDistancePacket.m_MonsterId);

            if (monster != null)
            {
                monster.m_BlackBoard.m_AttackDistance.Key = attackDistancePacket.m_AttackDistance;
            }
        }

        public void C_HitPacketHandler(Session session, IPacket packet)
        {
            C_HitPacket hitPacket = packet as C_HitPacket;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.m_GameRoom == null) return;

            BaseObject monster = Managers.Object.GetObject(hitPacket.m_MonsterId);
            BaseObject targetObject =  Managers.Object.GetObject(hitPacket.m_TargetObjectId);
            targetObject.m_BlackBoard.m_HP.Key -= monster.m_BlackBoard.m_DefaultAttackDamage.Key;

            if (targetObject.m_BlackBoard.m_HP.Key <= 0)
            {
                S_BroadcastMonsterDeathPacket monsterDeathPacket = new S_BroadcastMonsterDeathPacket();
                monsterDeathPacket.m_MonsterId = (ushort)targetObject.m_ObjectId;

                targetObject.m_IsDeath = true;
                Room room = clientSession.m_GameRoom;
                room.BroadCast(monsterDeathPacket.Write());
            }
        }

        public void C_ChangeTargetPacketHandler(Session session, IPacket packet)
        {
            C_ChangeTargetPacket changeTargetPacket = packet as C_ChangeTargetPacket;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.m_GameRoom == null) return;

            BaseObject obj = Managers.Object.GetObject(changeTargetPacket.m_ObjectId);
            BaseMonster monster = obj as BaseMonster;
            BaseObject targetObject = Managers.Object.GetObject(monster.SearchNearTarget().m_ObjectId);
            obj.m_BlackBoard.m_TargetObject.Key = targetObject;
            monster.m_Target = targetObject;
            S_BroadcastChangeTargetPacket broadcastChangeTarget = new S_BroadcastChangeTargetPacket();
            broadcastChangeTarget.m_ObjectId = changeTargetPacket.m_ObjectId;
            broadcastChangeTarget.m_TargetObjectId = (ushort)targetObject.m_ObjectId;

            Room room = clientSession.m_GameRoom;
            room.BroadCast(broadcastChangeTarget.Write());
        }
    }
}
