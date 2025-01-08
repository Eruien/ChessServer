using System.Numerics;

namespace ServerContent
{
    public class Lab : BaseObject
    {
        // ������ �Ҹ���
      
        // �⺻ ���� Init, Frame, Render, Release
        public void Init()
        {
            SetBlackBoardKey();
            m_SelfType = ObjectType.Machine;
        }

        public override void Frame()
        {
            IsHPZero();
        }

        // �Ϲ� �Լ�
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
                //���� ����Ǵ� ���� �ֱ�
            }
        }
    }
}


