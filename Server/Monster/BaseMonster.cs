using System.Numerics;

namespace ServerContent
{
    public class BaseMonster : BaseObject
    {
        public BaseObject target;
        public BaseObject targetLabo;
        protected MonsterType monsterType = MonsterType.None;
        public MonsterState monsterState = MonsterState.None;
        private Selector selector;

        public void Init()
        {
            SelfType = ObjectType.Monster;
            selector = new Selector();
            SetBlackBoardKey();
        }

        protected void Start()
        {
            // HP관리
            Selector HPMgr = new Selector();
            selector.AddChild(HPMgr);

            Sequence<float, b_float> checkHPZero = new Sequence<float, b_float>(KeyQuery.IsLessThanOrEqualTo, 0.001f, blackBoard.m_HP);
            HPMgr.AddChild(checkHPZero);

            Action action = new Action(IsHPZero);
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

        protected void Frame()
        {
            selector.Tick();
        }

        // 일반 함수
        private double ComputeAttackDistance()
        {
            if (target == null) return blackBoard.m_AttackDistance.Key;
            Vector3 vec = target.position - position;
            double dis = Math.Pow(vec.X * vec.X + vec.Z * vec.Z, 0.5f);

            return dis;
        }

        protected override void SetBlackBoardKey()
        {
            target = targetLabo;
            blackBoard.m_TargetObject.Key = target;
            blackBoard.m_HP.Key = Managers.Data.monsterDict[this.GetType().Name].hp;
            blackBoard.m_AttackDistance.Key = Managers.Data.monsterDict[this.GetType().Name].attackDistance;
            blackBoard.m_AttackRange.Key = Managers.Data.monsterDict[this.GetType().Name].attackRange;
            blackBoard.m_AttackRangeCorrectionValue.Key = Managers.Data.monsterDict[this.GetType().Name].attackRangeCorrectionValue;
            blackBoard.m_DefaultAttackDamage.Key = Managers.Data.monsterDict[this.GetType().Name].defaultAttackDamage;
            blackBoard.m_MoveSpeed.Key = Managers.Data.monsterDict[this.GetType().Name].moveSpeed;
            blackBoard.m_ProjectTileSpeed.Key = Managers.Data.monsterDict[this.GetType().Name].projectTileSpeed;
        }

        // TaskNode 모음
        private ReturnCode Attack()
        {
            monsterState = MonsterState.Attack;
            return ReturnCode.SUCCESS;
        }

        private ReturnCode MoveToPosition()
        {
            monsterState = MonsterState.Move;
            // 이동하는 로직 구현

            return ReturnCode.SUCCESS;
        }

        private ReturnCode IsHPZero()
        {
            if (blackBoard.m_HP.Key <= 0)
            {
                monsterState = MonsterState.Death;
                return ReturnCode.SUCCESS;
            }
            else
            {
                return ReturnCode.FAIL;
            }
        }
    }
}
