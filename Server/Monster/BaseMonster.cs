using Server;
using System.Numerics;

namespace ServerContent
{
    public class BaseMonster : BaseObject
    {
        // public, protected, private 순서
        // 참조타입, 구조체, 일반 타입
        // 거리 계산이나 이런것을 위하여
        public BaseObject m_Target { get; set; }
        public BaseObject m_TargetLab { get; set; }
        public MonsterType m_MonsterType { get; set; } = MonsterType.None;
        public MonsterState m_MonsterState { get; set; } = MonsterState.None;
       
        private Selector m_Selector = new Selector();
        private float m_TransportPacketTime = 0.1f;
        private float m_CurrentTime = 0.0f;

        // 생성자 소멸자
        public BaseMonster(BaseObject laboratory)
        {
            m_TargetLab = laboratory;
            m_Target = m_TargetLab;
        }

        // 기본 로직 Init, Frame, Render, Release
        public void Init()
        {
            m_SelfType = ObjectType.Monster;
            m_SelfTeam = Team.BlueTeam;
            // Position은 나중에 사용자 입력에 따라 별도 처리
            SetBlackBoardKey();
            MakeBehaviorTree();
        }

        public override void Frame()
        {
            if (!m_IsDeath)
            {
                m_CurrentTime += (float)LTimer.m_SPF;

                m_Selector.Tick();

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
            Selector hpMgr = new Selector();
            m_Selector.AddChild(hpMgr);

            Sequence<float, b_float> checkHPZero = new Sequence<float, b_float>(KeyQuery.IsLessThanOrEqualTo, 0.001f, m_BlackBoard.m_HP);
            hpMgr.AddChild(checkHPZero);

            ServerContent.Action action = new ServerContent.Action(IsHPZero);
            checkHPZero.AddChild(action);

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
            m_BlackBoard.m_HP.Key = Managers.Data.m_MonsterDict[this.GetType().Name].m_HP;
            m_BlackBoard.m_AttackDistance.Key = Managers.Data.m_MonsterDict[this.GetType().Name].m_AttackDistance;
            m_BlackBoard.m_AttackRange.Key = Managers.Data.m_MonsterDict[this.GetType().Name].m_AttackRange;
            m_BlackBoard.m_AttackRangeCorrectionValue.Key = Managers.Data.m_MonsterDict[this.GetType().Name].m_AttackRangeCorrectionValue;
            m_BlackBoard.m_DefaultAttackDamage.Key = Managers.Data.m_MonsterDict[this.GetType().Name].m_DefaultAttackDamage;
            m_BlackBoard.m_MoveSpeed.Key = Managers.Data.m_MonsterDict[this.GetType().Name].m_MoveSpeed;
            m_BlackBoard.m_ProjectTileSpeed.Key = Managers.Data.m_MonsterDict[this.GetType().Name].m_ProjectTileSpeed;
        }

        private double ComputeAttackDistance()
        {
            if (m_Target == null) return m_BlackBoard.m_AttackDistance.Key;
            Vector3 vec = m_Target.m_Position - m_Position;
            double dis = Math.Pow(vec.X * vec.X + vec.Z * vec.Z, 0.5f);

            return dis;
        }

        private void TransportPacket(System.Action action)
        {
            if (m_CurrentTime >= m_TransportPacketTime)
            {
                action.Invoke();
            }
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

        private ReturnCode IsHPZero()
        {
            if (m_BlackBoard.m_HP.Key <= 0)
            {
                m_MonsterState = MonsterState.Death;
                S_BroadcastMonsterStatePacket monsterStatePacket = new S_BroadcastMonsterStatePacket();
                monsterStatePacket.m_MonsterId = (ushort)m_ObjectId;
                monsterStatePacket.m_CurrentState = (ushort)m_MonsterState;

                Program.g_GameRoom.BroadCast(monsterStatePacket.Write());
               
                m_IsDeath = true;
                return ReturnCode.SUCCESS;
            }
            else
            {
                return ReturnCode.FAIL;
            }
        }
    }
}
