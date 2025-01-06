using System.Numerics;

namespace ServerContent
{
    public class Labo : BaseObject
    {
        // 생성자 소멸자
      
        // 기본 로직 Init, Frame, Render, Release
        public void Init()
        {
            SetBlackBoardKey();
            SelfType = ObjectType.Machine;
        }

        public override void Frame()
        {
            IsHPZero();
        }

        // 일반 함수
        public override void SetPosition(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
        }

        protected override void SetBlackBoardKey()
        {
            blackBoard.m_HP.Key = 100.0f;
        }

        private void IsHPZero()
        {
            if (blackBoard.m_HP.Key <= 0)
            {
                
            }
        }
    }
}


