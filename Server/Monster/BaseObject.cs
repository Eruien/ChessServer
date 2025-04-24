using Server;
using System.Numerics;

namespace ServerContent
{
    abstract public class BaseObject
    {
        public System.Action? m_TargetChangeObserver;

        public BlackBoard m_BlackBoard = new BlackBoard();
        public ObjectType m_SelfType { get; set; } = ObjectType.None;
        public Team m_SelfTeam { get; set; } = Team.None;
        public Vector3 m_Position { get; set; } = new Vector3(0.0f, 0.0f, 0.0f);
        public bool m_IsPosCorrect { get; set; } = false;
        public bool m_IsDeath { get; set; } = false;
        public int m_ObjectId { get; set; } = 0;
        public string m_Name { get; set; } = "";

        public abstract void Frame();
        public abstract void SetPosition(float x, float y, float z);
        public void ClearChangeObserverListener() { m_TargetChangeObserver = null; }
        protected abstract void SetBlackBoardKey();
    }
}

 
