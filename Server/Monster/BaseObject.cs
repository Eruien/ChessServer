using System.Numerics;

namespace ServerContent
{
    abstract public class BaseObject
    {
        public ObjectType SelfType { get; set; }

        public BlackBoard blackBoard = new BlackBoard();
        public bool IsDeath { get; set; } = false;

        public Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
        protected abstract void SetBlackBoardKey();
    }
}

 
