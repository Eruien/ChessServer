﻿using System;
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
            ClientSession clientSession = session as ClientSession;
            BaseObject labo = null;

            if (clientSession.GameRoom == null) return;
            int currentTeam = 0;

            if ((clientSession.SessionId % 2) == 0)
            {
                currentTeam = (ushort)Team.RedTeam;
            }
            else
            {
                currentTeam = (ushort)Team.BlueTeam;
            }
           
            for (int i = 1; i <= 2; i++)
            {
                if (Managers.Object.GetObject(i).SelfTeam != (Team)currentTeam)
                {
                    labo = Managers.Object.GetObject(i);
                    break;
                }
            }
           
            BaseMonster monster = new BaseMonster(labo);
            
            monster.Init();
            monster.SetPosition(monsterCreatePacket.PosX, monsterCreatePacket.PosY, monsterCreatePacket.PosZ);
            monster.MonsterId = Managers.Object.Register(monster);

            S_BroadcastMonsterCreatePacket broadcastMonsterPacket = new S_BroadcastMonsterCreatePacket();
            broadcastMonsterPacket.monsterTeam = (ushort)currentTeam; 
            broadcastMonsterPacket.objectId = (ushort)monster.MonsterId;
            broadcastMonsterPacket.PosX = monsterCreatePacket.PosX;
            broadcastMonsterPacket.PosY = monsterCreatePacket.PosY;
            broadcastMonsterPacket.PosZ = monsterCreatePacket.PosZ;
           
            Room room = clientSession.GameRoom;
            room.BroadCast(broadcastMonsterPacket.Write());
        }

        public void C_AttackDistancePacketHandler(Session session, IPacket packet)
        {
            C_AttackDistancePacket attackDistancePacket = packet as C_AttackDistancePacket;

            BaseObject monster = Managers.Object.GetObject(attackDistancePacket.objectId);

            if (monster != null)
            {
                monster.blackBoard.m_AttackDistance.Key = attackDistancePacket.attackDistance;
            }
        }

        public void C_HitPacketHandler(Session session, IPacket packet)
        {
            C_HitPacket hitPacket = packet as C_HitPacket;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.GameRoom == null) return;

            BaseObject monster = Managers.Object.GetObject(hitPacket.objectId);
            BaseObject targetObject =  Managers.Object.GetObject(hitPacket.targetMonsterId);
            targetObject.blackBoard.m_HP.Key -= monster.blackBoard.m_DefaultAttackDamage.Key;
            S_HitPacket hPacket = new S_HitPacket();
            hPacket.objectId = hitPacket.targetMonsterId;
            hPacket.objectHP = (ushort)targetObject.blackBoard.m_HP.Key;
            Room room = clientSession.GameRoom;
            room.BroadCast(hPacket.Write());
        }

        public void C_ChangeTargetPacketHandler(Session session, IPacket packet)
        {
            C_ChangeTargetPacket changeTargetPacket = packet as C_ChangeTargetPacket;
            ClientSession clientSession = session as ClientSession;

            if (clientSession.GameRoom == null) return;

            BaseObject obj = Managers.Object.GetObject(changeTargetPacket.objectId);
            BaseObject targetObject = Managers.Object.GetObject(changeTargetPacket.targetObjectId);
            obj.blackBoard.m_TargetObject.Key = targetObject;

            S_ChangeTargetPacket ServerChangeTarget = new S_ChangeTargetPacket();
            ServerChangeTarget.objectId = changeTargetPacket.objectId;
            ServerChangeTarget.targetObjectId = changeTargetPacket.targetObjectId;
            Room room = clientSession.GameRoom;
            room.BroadCast(ServerChangeTarget.Write());
        }
    }
}
