using Server;
using ServerCore;
using System;
using System.Numerics;

namespace ServerContent
{
    public class BaseMonster : BaseObject
    {
        // public, protected, private 순서
        // 참조타입, 구조체, 일반 타입
        // 거리 계산이나 이런것을 위하여
        public static System.Action m_SearchNearTarget;
        public BaseObject m_Target { get; set; }
        public BaseObject m_TargetLab { get; set; }
        public MonsterType m_MonsterType { get; set; } = MonsterType.None;
        public MonsterState m_MonsterState { get; set; } = MonsterState.None;
       
        private Selector m_Selector = new Selector();
        private float m_TransportPacketTime = 0.1f;
        private float m_CurrentTime = 0.0f;
        private float m_PushingSpeed = 3.0f;
        private float m_PushingSpan = 1.0f;

        // 생성자 소멸자
        public BaseMonster(BaseObject laboratory)
        {
            m_TargetLab = laboratory;
            m_Target = m_TargetLab;
            m_SearchNearTarget += () => SearchFristTarget();
        }

        // 기본 로직 Init, Frame, Render, Release
        public void Init()
        {
            m_SelfType = ObjectType.Monster;
            // Position은 나중에 사용자 입력에 따라 별도 처리
            SetBlackBoardKey();
            MakeBehaviorTree();
        }

        public override void Frame()
        {
            if (!m_IsDeath)
            {
                m_CurrentTime += (float)LTimer.m_SPF;

                if (m_Target != null)
                {
                    CollisionAvoidance();
                    m_Selector.Tick();
                }
              
                if (m_CurrentTime >= m_TransportPacketTime)
                {
                    m_CurrentTime = 0.0f;
                }
            }
        }

        // 생성 로직 Init에 들어가는 초기화 처리 되어야 하는 로직
        private void MakeBehaviorTree()
        {
            // HP관리
            // 공격 관리
            Selector attackMgr = new Selector();
            m_Selector.AddChild(attackMgr);

            SetSequence<object, b_Object> checkTargetLive = new SetSequence<object, b_Object>(KeyQuery.IsSet, m_BlackBoard.m_TargetObject);
            attackMgr.AddChild(checkTargetLive);

            Sequence<float, b_float> checkAttackRange = new Sequence<float, b_float>(KeyQuery.IsLessThanOrEqualTo, m_BlackBoard.m_AttackRange.Key, m_BlackBoard.m_AttackDistance);
            checkTargetLive.AddChild(checkAttackRange);

            Action attackAction = new Action(Attack);
            checkAttackRange.AddChild(attackAction);

            // 이동 관리
            Selector moveMgr = new Selector();
            m_Selector.AddChild(moveMgr);

            SetSequence<object, b_Object> move = new SetSequence<object, b_Object>(KeyQuery.IsSet, m_BlackBoard.m_TargetObject);
            moveMgr.AddChild(move);
            Action moveAction = new Action(MoveToPosition);

            move.AddChild(moveAction);
        }

        // 일반 함수
        public override void SetPosition(float x, float y, float z)
        {
            m_Position = new Vector3(x, y, z);
        }

        public void SetTarget(BaseObject obj)
        {
            m_Target = obj;
            m_BlackBoard.m_TargetObject.Key = m_Target;
        }

        protected override void SetBlackBoardKey()
        {
            // 클라에서 이름 정보 받아오면 이름 정보로 교체
            m_BlackBoard.m_TargetObject.Key = m_TargetLab;
            m_BlackBoard.m_HP.Key = Managers.Data.m_MonsterDict[m_Name].hp;
            m_BlackBoard.m_AttackDistance.Key = Managers.Data.m_MonsterDict[m_Name].attackDistance;
            m_BlackBoard.m_AttackRange.Key = Managers.Data.m_MonsterDict[m_Name].attackRange;
            m_BlackBoard.m_AttackRangeCorrectionValue.Key = Managers.Data.m_MonsterDict[m_Name].attackRangeCorrectionValue;
            m_BlackBoard.m_DefaultAttackDamage.Key = Managers.Data.m_MonsterDict[m_Name].defaultAttackDamage;
            m_BlackBoard.m_MoveSpeed.Key = Managers.Data.m_MonsterDict[m_Name].moveSpeed;
            m_BlackBoard.m_ProjectTileSpeed.Key = Managers.Data.m_MonsterDict[m_Name].projectTileSpeed;
        }

        private void TransportPacket(System.Action action)
        {
            if (m_CurrentTime >= m_TransportPacketTime)
            {
                action.Invoke();
            }
        }

        private void CollisionAvoidance()
        {
            foreach (var obj in Managers.Object.m_ObjectDict)
            {
                Vector3 rDir = m_Position - obj.Value.m_Position;

                double dis = Math.Pow(rDir.X * rDir.X + rDir.Z * rDir.Z, 0.5f);

                if (rDir == Vector3.Zero)
                {
                    rDir = Vector3.Zero;
                }
                else
                {
                    rDir = Vector3.Normalize(rDir);
                }

                if (dis <= m_PushingSpan)
                {
                    m_Position += rDir * m_PushingSpeed * (float)LTimer.m_SPF;
                }
            }
        }

        public BaseObject SearchNearTarget()
        {
            BaseObject nearTarget = null;
            double minDistance = double.PositiveInfinity;
            
            foreach (var obj in Managers.Object.m_ObjectDict)
            {
                if (obj.Value.m_SelfTeam == m_SelfTeam) continue;
                if (obj.Value.m_IsDeath) continue;

                Vector3 rDir = obj.Value.m_Position - m_Position;
                double dis = Math.Pow(rDir.X * rDir.X + rDir.Z * rDir.Z, 0.5f);

                if (dis <= minDistance)
                {
                    minDistance = dis;
                    nearTarget = obj.Value;
                }
            }

            return nearTarget;
        }

        public void SearchFristTarget()
        {   
            S_BroadcastChangeTargetPacket broadcastChangeTarget = new S_BroadcastChangeTargetPacket();
            broadcastChangeTarget.m_ObjectId = (ushort)m_ObjectId;
            broadcastChangeTarget.m_TargetObjectId = (ushort)SearchNearTarget().m_ObjectId;

            TransportPacket(() => Program.g_GameRoom.BroadCast(broadcastChangeTarget.Write()));
        }

        // TaskNode 모음
        private ReturnCode Attack()
        {
            m_MonsterState = MonsterState.Attack;
            S_BroadcastMonsterStatePacket monsterStatePacket = new S_BroadcastMonsterStatePacket();
            monsterStatePacket.m_MonsterId = (ushort)m_ObjectId;
            monsterStatePacket.m_CurrentState = (ushort)m_MonsterState;

            TransportPacket(() => Program.g_GameRoom.BroadCast(monsterStatePacket.Write()));
            
            return ReturnCode.SUCCESS;
        }

        private ReturnCode MoveToPosition()
        {
            m_MonsterState = MonsterState.Move;
            Vector3 dir = m_Target.m_Position - m_Position;

            if (dir == Vector3.Zero)
            {
                dir = Vector3.Zero;
            }
            else
            {
                dir = Vector3.Normalize(dir);
            }

            m_Position += dir * m_BlackBoard.m_MoveSpeed.Key * (float)LTimer.m_SPF;

            // 패킷 보내기
            S_BroadcastMovePacket movePacket = new S_BroadcastMovePacket();
            movePacket.m_MonsterId = (ushort)m_ObjectId;
            movePacket.m_PosX = m_Position.X;
            movePacket.m_PosY = m_Position.Y;
            movePacket.m_PosZ = m_Position.Z;

            S_BroadcastMonsterStatePacket monsterStatePacket = new S_BroadcastMonsterStatePacket();
            monsterStatePacket.m_MonsterId = (ushort)m_ObjectId;
            monsterStatePacket.m_CurrentState = (ushort)m_MonsterState;

            TransportPacket(() => Program.g_GameRoom.BroadCast(monsterStatePacket.Write()));
            TransportPacket(() => Program.g_GameRoom.BroadCast(movePacket.Write()));
            
            return ReturnCode.SUCCESS;
        }
    }
}
