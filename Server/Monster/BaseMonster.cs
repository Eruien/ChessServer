using Server;
using System;
using System.Net;
using System.Numerics;

namespace ServerContent
{
    public class BaseMonster : BaseObject
    {
        // public, protected, private 순서
        // 참조타입, 구조체, 일반 타입
        // 거리 계산이나 이런것을 위하여
        public BaseObject Target { get; set; }
        public BaseObject TargetLabo { get; set; }
        public MonsterType MonsterType { get; set; } = MonsterType.None;
        public MonsterState MonsterState { get; set; } = MonsterState.None;
        public int MonsterId { get; set; } = 0;
 
        private Selector selector = new Selector();
        private float transportPacketTime = 0.01f;
        private float currentTime = 0.0f;

        // 생성자 소멸자
        public BaseMonster(BaseObject laboratory)
        {
            TargetLabo = laboratory;
            Target = TargetLabo;
        }

        // 기본 로직 Init, Frame, Render, Release
        public void Init()
        {
            SelfType = ObjectType.Monster;
            SelfTeam = Team.BlueTeam;
            // Position은 나중에 사용자 입력에 따라 별도 처리
            SetBlackBoardKey();
            MakeBehaviorTree();
        }

        public void Frame()
        {
            if (!IsDeath)
            {
                currentTime += (float)LTimer.m_SPF;

                if (Target == null)
                {
                    if (TargetLabo != null)
                    {
                        Target = TargetLabo;
                        blackBoard.m_TargetObject.Key = Target;
                    }
                }
                blackBoard.m_AttackDistance.Key = (float)ComputeAttackDistance();
                selector.Tick();

                if (currentTime >= transportPacketTime)
                {
                    currentTime = 0.0f;
                }
            }
        }

        // 생성 로직 Init에 들어가는 초기화 처리 되어야 하는 로직
        private void MakeBehaviorTree()
        {
            // HP관리
            Selector HPMgr = new Selector();
            selector.AddChild(HPMgr);

            Sequence<float, b_float> checkHPZero = new Sequence<float, b_float>(KeyQuery.IsLessThanOrEqualTo, 0.001f, blackBoard.m_HP);
            HPMgr.AddChild(checkHPZero);

            ServerContent.Action action = new ServerContent.Action(IsHPZero);
            checkHPZero.AddChild(action);

            // 공격 관리
            Selector attackMgr = new Selector();
            selector.AddChild(attackMgr);

            SetSequence<object, b_Object> checkTargetLive = new SetSequence<object, b_Object>(KeyQuery.IsSet, blackBoard.m_TargetObject);
            attackMgr.AddChild(checkTargetLive);

            Sequence<float, b_float> checkAttackRange = new Sequence<float, b_float>(KeyQuery.IsLessThanOrEqualTo, blackBoard.m_AttackRange.Key, blackBoard.m_AttackDistance);
            checkTargetLive.AddChild(checkAttackRange);

            Action attackAction = new Action(Attack);
            checkAttackRange.AddChild(attackAction);

            // 이동 관리
            Selector moveMgr = new Selector();
            selector.AddChild(moveMgr);

            SetSequence<object, b_Object> move = new SetSequence<object, b_Object>(KeyQuery.IsSet, blackBoard.m_TargetObject);
            moveMgr.AddChild(move);
            Action moveAction = new Action(MoveToPosition);

            move.AddChild(moveAction);
        }

        // 일반 함수
        public override void SetPosition(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
        }

        public void SetTarget(BaseObject obj)
        {
            Target = obj;
            blackBoard.m_TargetObject.Key = Target;
        }

        protected override void SetBlackBoardKey()
        {
            blackBoard.m_TargetObject.Key = TargetLabo;
            blackBoard.m_HP.Key = Managers.Data.monsterDict[this.GetType().Name].hp;
            blackBoard.m_AttackDistance.Key = Managers.Data.monsterDict[this.GetType().Name].attackDistance;
            blackBoard.m_AttackRange.Key = Managers.Data.monsterDict[this.GetType().Name].attackRange;
            blackBoard.m_AttackRangeCorrectionValue.Key = Managers.Data.monsterDict[this.GetType().Name].attackRangeCorrectionValue;
            blackBoard.m_DefaultAttackDamage.Key = Managers.Data.monsterDict[this.GetType().Name].defaultAttackDamage;
            blackBoard.m_MoveSpeed.Key = Managers.Data.monsterDict[this.GetType().Name].moveSpeed;
            blackBoard.m_ProjectTileSpeed.Key = Managers.Data.monsterDict[this.GetType().Name].projectTileSpeed;
        }

        private double ComputeAttackDistance()
        {
            if (Target == null) return blackBoard.m_AttackDistance.Key;
            Vector3 vec = Target.Position - Position;
            double dis = Math.Pow(vec.X * vec.X + vec.Z * vec.Z, 0.5f);

            return dis;
        }

        private void TransportPacket(System.Action action)
        {
            if (currentTime >= transportPacketTime)
            {
                action.Invoke();
            }
        }

        // TaskNode 모음
        private ReturnCode Attack()
        {
            MonsterState = MonsterState.Attack;
            S_MonsterStatePacket monsterStatePacket = new S_MonsterStatePacket();
            monsterStatePacket.monsterId = (ushort)MonsterId;
            monsterStatePacket.currentState = (ushort)MonsterState;

            TransportPacket(() => Program.g_GameRoom.BroadCast(monsterStatePacket.Write()));
            
            return ReturnCode.SUCCESS;
        }

        private ReturnCode MoveToPosition()
        {
            MonsterState = MonsterState.Move;
            Vector3 dir = Target.Position - Position;

            if (dir == Vector3.Zero)
            {
                dir = Vector3.Zero;
            }
            else
            {
                dir = Vector3.Normalize(dir);
            }

            Position += dir * blackBoard.m_MoveSpeed.Key * (float)LTimer.m_SPF;

            // 패킷 보내기
            MovePacket movePacket = new MovePacket();
            movePacket.monsterId = (ushort)MonsterId;
            movePacket.PosX = Position.X;
            movePacket.PosY = Position.Y;
            movePacket.PosZ = Position.Z;

            S_MonsterStatePacket monsterStatePacket = new S_MonsterStatePacket();
            monsterStatePacket.monsterId = (ushort)MonsterId;
            monsterStatePacket.currentState = (ushort)MonsterState;

            TransportPacket(() => Program.g_GameRoom.BroadCast(monsterStatePacket.Write()));
            TransportPacket(() => Program.g_GameRoom.BroadCast(movePacket.Write()));
            
            return ReturnCode.SUCCESS;
        }

        private ReturnCode IsHPZero()
        {
            if (blackBoard.m_HP.Key <= 0)
            {
                MonsterState = MonsterState.Death;
                S_MonsterStatePacket monsterStatePacket = new S_MonsterStatePacket();
                monsterStatePacket.monsterId = (ushort)MonsterId;
                monsterStatePacket.currentState = (ushort)MonsterState;

                Program.g_GameRoom.BroadCast(monsterStatePacket.Write());
               
                IsDeath = true;
                return ReturnCode.SUCCESS;
            }
            else
            {
                return ReturnCode.FAIL;
            }
        }
    }
}
