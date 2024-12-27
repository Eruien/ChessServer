using System.Numerics;

namespace ServerContent
{
    public class Labo : BaseObject
    {
        // RedRaboPos = {1.33, 2.904, -21.87}
        protected override void SetBlackBoardKey()
        {
            blackBoard.m_HP.Key = 100.0f;
            position = new Vector3(1.33f, 2.904f, -21.87f);
        }

        public override void SetPosition(float x, float y, float z)
        {
            position = new Vector3(x, y, z);
        }

        public void Init()
        {
            SetBlackBoardKey();
            SelfType = ObjectType.Machine;
        }

        private void Update()
        {
            IsHPZero();
        }

        private void IsHPZero()
        {
            if (blackBoard.m_HP.Key <= 0)
            {
                //Destroy(gameObject);
            }
        }
    }
}


