
namespace ServerContent
{
    public class Labo : BaseObject
    {
        protected override void SetBlackBoardKey()
        {
            blackBoard.m_HP.Key = 100.0f;
        }

        private void Awake()
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


