using System.Numerics;

namespace ServerContent
{
    public class Lab : BaseObject
    {
        // 생성자 소멸자
      
        // 기본 로직 Init, Frame, Render, Release
        public void Init()
        {
            SetBlackBoardKey();
            m_SelfType = ObjectType.Machine;
        }

        public override void Frame()
        {
            IsHPZero();
        }

        // 일반 함수
        public override void SetPosition(float x, float y, float z)
        {
            m_Position = new Vector3(x, y, z);
        }

        protected override void SetBlackBoardKey()
        {
            m_BlackBoard.m_HP.Key = 100.0f;
        }

        private void IsHPZero()
        {
            if (m_BlackBoard.m_HP.Key <= 0)
            {
                //게임 종료되는 로직 넣기
            }
        }
    }
}


