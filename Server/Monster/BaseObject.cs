using System.Numerics;

namespace ServerContent
{
    abstract public class BaseObject
    {
        public BlackBoard blackBoard = new BlackBoard();
        public ObjectType SelfType { get; set; } = ObjectType.None;
        public Team SelfTeam { get; set; } = Team.None;
        public Vector3 Position { get; set; } = new Vector3(0.0f, 0.0f, 0.0f);
        public bool IsDeath { get; set; } = false;
        
        public abstract void SetPosition(float x, float y, float z);
        protected abstract void SetBlackBoardKey();
    }
}

 
